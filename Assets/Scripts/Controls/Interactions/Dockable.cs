using DG.Tweening;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

/**
 * Objects that can be docked to a docker.
 */
public class Dockable : Pickable
{
    [Header("Position")]
    [Description("Rotation so that the object is correctly docked")]
    [SerializeField] private UnityEngine.Vector3 _correctRotation;
    [Description("The position of the main point that will be docked")]
    [SerializeField] private Vector3 _centerPosition = new (0, 0, 0);
    
    [Header("Events")]
    [SerializeField] private OnDocking _onDock = new OnDocking();

    [Header("Constraints")]
    [SerializeField] private List<UnityEngine.Vector2> _constraints = new();

    private Rigidbody _rb;
    private List<Docker> _dockedOn = new List<Docker>();
    private LogManager _logger;

    public OnDocking OnDock { get => _onDock; }
    public List<Docker> DockedOn { get => _dockedOn; set => _dockedOn = value; }

    protected override void Start()
    {
        base.Start();
        _logger = LogManager.Instance;
        _rb = GetComponent<Rigidbody>();
        if (_rb == null) Debug.LogError("Rigidbody missing on " + gameObject.name);

        if (_correctRotation == null) _correctRotation = new UnityEngine.Vector3(0, 0, 0);

        if (_constraints.Count == 0)
        {
            OnDock.AddListener(SimpleDockToObject);
            OnPick.AddListener(SimpleUndockObject);
        }
        else
        {
            OnDock.AddListener(ConstrainedDockToObject);
            OnPick.AddListener(ConstrainedUndockObject);
        }
    }

    /**
     * Dock a simple object to a docker.
     * A simple object is an object that has no constraints.
     */
    private void SimpleDockToObject(Docker docker) {
        if (!docker.IsAvailable || !docker.IsActive)
            return;

        _logger.Trace("Docking " + gameObject.name + " to " + docker.name);

        DockedOn.Add(docker);
        _rb.isKinematic = true;
        transform.SetParent(docker.transform);

        DockMovement(docker);

        var onDockEvent = new OnDockEvent();
        onDockEvent.Type = OnDockEvent.DockType.DOCK;
        onDockEvent.Dockable = this;

        docker.OnDock.Invoke(onDockEvent);
    }

    private void ConstrainedDockToObject(Docker docker)
    {
        if (!docker.IsAvailable || !docker.IsActive)
        {
            _logger.Trace("Trying to dock to an unavailable/unactive docker");
            return;
        }
        var dockManager = DockManager.Instance;
        if (!dockManager.IsDockable(new Vector2(docker.X, docker.Y), _constraints))
        {
            transform.SetParent(OriginParent);
            _logger.Trace("Trying to dock to an available/active docker with not enough room");
            return;
        }

        _rb.isKinematic = true;

        var dockerList = new List<Docker>{docker};
        foreach (var constraint in _constraints)
        {
            var d = dockManager.GetDocker(new Vector2(docker.X + constraint.x, docker.Y + constraint.y));
            dockerList.Add(d);
        }

        foreach (Docker d in dockerList)
        {
            DockedOn.Add(d);

            var onDockEvent = new OnDockEvent();
            onDockEvent.Type = OnDockEvent.DockType.DOCK;
            onDockEvent.Dockable = this;
            if (d!=docker) onDockEvent.IsSecondary = true;

            d.OnDock.Invoke(onDockEvent);
        }

        DockMovement(docker);
    }

    /**
    * UnDock a simple object from a docker.
    * A simple object is an object that has no constraints.
    */
    private void SimpleUndockObject()
    {
        if (DockedOn.Count == 0) return;

        _logger.Trace("Undocking " + gameObject.name + " from " + DockedOn[0].name);

        var onUndockEvent = new OnDockEvent();
        onUndockEvent.Type = OnDockEvent.DockType.UNDOCK;
        onUndockEvent.Dockable = this;
        DockedOn[0].OnDock.Invoke(onUndockEvent);

        DockedOn.Clear();
    }

    private void ConstrainedUndockObject()
    {
        if (DockedOn.Count == 0) return;
        foreach(var docker in DockedOn)
        {
            _logger.Trace("Undocking " + gameObject.name + " from " + docker);

            var onUndockEvent = new OnDockEvent();
            onUndockEvent.Type = OnDockEvent.DockType.UNDOCK;
            onUndockEvent.Dockable = this;
            if (docker != DockedOn[0]) onUndockEvent.IsSecondary = true;
            docker.OnDock.Invoke(onUndockEvent);
        }

        DockedOn.Clear();
    }

    #region Movements

    private void DockMovement(Docker docker)
    {
        Sequence sequence = DOTween.Sequence()
                    .Append(transform.DOMove(docker.transform.position - _centerPosition * 0.15f, 1f).SetEase(Ease.InQuad))     // May be a + instead of a -
                    .Join(transform.DORotate(_correctRotation, 1f).SetEase(Ease.InQuad))
                    .OnComplete(() =>
                    {
                        if (!docker.IsAvailable) // To verify if we haven't undocked in the meantime
                        {
                            transform.SetParent(docker.transform); // To be sure
                            transform.localPosition = -_centerPosition;
                        }
                    });
    }

    #endregion

}

#region Events
/**
 * Event invoked when a dockable object is docked to a docker.
 * The docker is passed as a parameter.
 */
[Serializable] public class OnDocking : UnityEvent<Docker> { }
#endregion