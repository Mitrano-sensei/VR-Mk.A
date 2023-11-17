using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
 * Objects that can be docked to a docker.
 */
public class Dockable : Pickable
{
    [Header("Position")]
    [SerializeField] private UnityEngine.Vector3 _correctRotation;
    
    [Header("Events")]
    [SerializeField] private OnDocking _onDock = new OnDocking();

    [Header("Constraints")]
    [SerializeField] private List<UnityEngine.Vector2> _constraints = new();

    private Rigidbody _rb;
    private Docker _dockedOn;
    private LogManager _logger;

    public OnDocking OnDock { get => _onDock; }
    public Docker DockedOn { get => _dockedOn; set => _dockedOn = value; }

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
    }

    /**
     * Dock a simple object to a docker.
     * A simple object is an object that has no constraints.
     */
    private void SimpleDockToObject(Docker docker) {
        if (!docker.IsAvailable || !docker.IsActive)
            return;

        _logger.Trace("Docking " + gameObject.name + " to " + docker.name);

        DockedOn = docker;
        _rb.isKinematic = true;
        transform.SetParent(docker.transform);
        Sequence sequence = DOTween.Sequence()
                    .Append(transform.DOMove(docker.transform.position, 1f).SetEase(Ease.InQuad))
                    .Join(transform.DORotate(_correctRotation, 1f).SetEase(Ease.InQuad))
                    .OnComplete(() =>
                    {
                        if (!docker.IsAvailable) // To verify if we haven't undocked in the meantime
                        {
                            transform.SetParent(docker.transform); // To be sure
                            transform.localPosition = new();
                        }
                    });

        var onDockEvent = new OnDockEvent();
        onDockEvent.Type = OnDockEvent.DockType.DOCK;
        onDockEvent.Dockable = this;

        docker.OnDock.Invoke(onDockEvent);
    }

    /**
     * UnDock a simple object from a docker.
     * A simple object is an object that has no constraints.
     */
    private void SimpleUndockObject()
    {
        if (DockedOn == null) return;

        _logger.Trace("Undocking " + gameObject.name + " from " + DockedOn.name);

        var onUndockEvent = new OnDockEvent();
        onUndockEvent.Type = OnDockEvent.DockType.UNDOCK;
        onUndockEvent.Dockable = this;
        DockedOn.OnDock.Invoke(onUndockEvent);

        DockedOn = null;
    }

}

#region Events
/**
 * Event invoked when a dockable object is docked to a docker.
 * The docker is passed as a parameter.
 */
[Serializable] public class OnDocking : UnityEvent<Docker> { }
#endregion