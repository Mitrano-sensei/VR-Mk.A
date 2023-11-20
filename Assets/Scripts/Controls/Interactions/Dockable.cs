using DG.Tweening;
using System;
using System.Collections;
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
    [SerializeField] private Vector3 _correctRotation;
    [Description("The position of the main point that will be docked")]
    [SerializeField] private Vector3 _centerPosition = new (0, 0, 0);
    
    [Header("Events")]
    [SerializeField] private OnDocking _onDock = new OnDocking();
    [SerializeField] private OnEject _onEject = new OnEject();

    [Header("Constraints")]
    [SerializeField] private List<Vector2> _constraints = new();

    [Header("Misc")]
    [Description("The Time in Second the object becomes bouncy after an ejection.")]
    [SerializeField] private float _bouncyTimeInSeconds = 1f;

    private Rigidbody _rb;
    private List<Docker> _dockedOn = new List<Docker>();
    private LogManager _logger;

    public OnDocking OnDock { get => _onDock; }
    public List<Docker> DockedOn { get => _dockedOn; set => _dockedOn = value; }
    public Vector3 CorrectRotation { get => _correctRotation; private set => _correctRotation = value; }
    public Vector3 CenterPosition { get => _centerPosition; private set => _centerPosition = value; }
    public OnEject OnEject { get => _onEject; set => _onEject = value; }

    protected override void Start()
    {
        base.Start();
        _logger = LogManager.Instance;
        _rb = GetComponent<Rigidbody>();
        if (_rb == null) Debug.LogError("Rigidbody missing on " + gameObject.name);

        if (CorrectRotation == null) CorrectRotation = new Vector3(0, 0, 0);

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

        OnEject.AddListener(EjectHandler);
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

    /**
     * Docking when the item is constrained, i.e. when it takes more than 1 dockers to dock.
     */
    private void ConstrainedDockToObject(Docker docker)
    {
        if (!docker.IsAvailable || !docker.IsActive)
        {
            _logger.Trace("Trying to dock to an unavailable/unactive docker");
            return;
        }
        var dockManager = DockManager.Instance;
        if (!dockManager.IsDockable(new (docker.X, docker.Y, docker.Z), _constraints))
        {
            transform.SetParent(OriginParent);
            _logger.Trace("Trying to dock to an available/active docker with not enough room");
            return;
        }

        _rb.isKinematic = true;

        var dockerList = new List<Docker>{docker};
        foreach (var constraint in _constraints)
        {
            var d = dockManager.GetDocker(new Vector3(docker.X + constraint.x, docker.Y + constraint.y, docker.Z));
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

    /**
     * UnDocking when the item is constrained, i.e. when it takes more than 1 dockers to dock.
     */
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

    /**
     * Behavior when the object is ejected.
     */
    public void EjectHandler(EjectEvent ejectEvent)
    {
        if (DockedOn.Count == 0) return;
        if (_constraints.Count == 0)    SimpleUndockObject();
        else                            ConstrainedUndockObject();

        // Create a random direction in the demi sphere centered on floatingDirection
        var randomDirection = UnityEngine.Random.insideUnitSphere;
        randomDirection = (Vector3.Dot(randomDirection, ejectEvent.EjectDirection) < 0 ? -1 : 1) * randomDirection;

        // Push the dockable up a little bedore ejecting it
        _rb.transform.position += ejectEvent.EjectDirection * 0.2f;
        _rb.isKinematic = false;
        _rb.AddForce(randomDirection.normalized * 10, ForceMode.Impulse);

        // Make the material super bouncy for 3 seconds
        var originalMaterial = GetComponent<Collider>().material;
        var superBouncyMaterial = new PhysicMaterial
        {
            bounciness = 1f,
            dynamicFriction = originalMaterial.dynamicFriction,
            staticFriction = originalMaterial.staticFriction,
            frictionCombine = originalMaterial.frictionCombine,
            bounceCombine = PhysicMaterialCombine.Maximum
        };

        StartCoroutine(MakeSuperBouncy(originalMaterial, superBouncyMaterial, _bouncyTimeInSeconds));
    }

    /**
     * Makes the material super bouncy for a given time.
     */
    private IEnumerator MakeSuperBouncy(PhysicMaterial originalMaterial, PhysicMaterial superBouncyMaterial, float timeInSeconds)
    {
        GetComponent<Collider>().material = superBouncyMaterial;
        yield return new WaitForSeconds(timeInSeconds);
        GetComponent<Collider>().material = originalMaterial;
    }


    #region Movements

    private void DockMovement(Docker docker)
    {
        Sequence sequence = DOTween.Sequence()
                    .AppendCallback(() => transform.SetParent(docker.transform))
                    .Append(transform.DOLocalMove(CenterPosition, 1f).SetEase(Ease.InQuad))
                    .Join(transform.DOLocalRotate(CorrectRotation, 1f).SetEase(Ease.InQuad))
                    .OnComplete(() =>
                    {
                        if (!docker.IsAvailable) // To verify if we haven't undocked in the meantime
                        {
                            transform.SetParent(docker.transform); // To be sure
                            transform.localPosition = CenterPosition;
                            transform.localRotation = Quaternion.Euler(CorrectRotation);
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
/**
 * Event invoked when a dockable object is ejected from a docker.
 */
[Serializable] public class OnEject : UnityEvent<EjectEvent> { }

public class EjectEvent
{
    public Vector3 EjectDirection;

    public EjectEvent(Vector3 ejectDirection = new Vector3())
    {
        EjectDirection = ejectDirection;
    }
}
#endregion