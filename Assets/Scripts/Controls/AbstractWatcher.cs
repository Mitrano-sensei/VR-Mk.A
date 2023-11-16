using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/**
 * A watcher that defines a common interface for all control watchers.
 * That is to say different interactions that can be performed by the player.
 * 
 * A Watcher extending this class should listen to player inputs and invoke events accordingly.
 */
public abstract class AbstractControlWatcher : Singleton<AbstractControlWatcher>
{
    private Pickable grabbedObject = null;

    [SerializeField] private OnInteractEvent _onInteractEvent = new OnInteractEvent();
    [SerializeField] private OnTeleportEvent _onTeleportEvent = new OnTeleportEvent();
    [SerializeField] private OnGrabEvent _onGrabEvent = new OnGrabEvent();
    [SerializeField] private OnReleaseEvent _onReleaseEvent = new OnReleaseEvent();

    public OnInteractEvent OnInteractEvent { get => _onInteractEvent; }
    public OnTeleportEvent OnTeleportEvent { get => _onTeleportEvent; }
    public OnGrabEvent OnGrabEvent { get => _onGrabEvent; }
    public OnReleaseEvent OnReleaseEvent { get => _onReleaseEvent; }
    protected Pickable GrabbedObject { get => grabbedObject; set => grabbedObject = value; }

    protected override void Awake()
    {
        base.Awake();
        GrabbedObject = null;

        OnGrabEvent.AddListener(HandleBaseGrab);

        OnReleaseEvent.AddListener(HandleBaseRelease);

        OnInteractEvent.AddListener(HandleBaseInteract);
    }

    /**
     * Handle base grab
     */
    private void HandleBaseGrab(Pickable pickable)
    {
        GrabbedObject = pickable;
        pickable.OnPick.Invoke();
    }

    /**
     * Handle base release
     */
    private void HandleBaseRelease(ReleasedEvent releasedEvent) {
        GrabbedObject.OnUnPick.Invoke();

        var dockable = GrabbedObject.GetComponent<Dockable>();
        var docker = releasedEvent.GetDocker();
        if (dockable != null && docker != null)
        {
            dockable.OnDock.Invoke(docker);
        }

        GrabbedObject = null;
    }

    /**
     * Handle base interact
     */
    private void HandleBaseInteract(Interactable target)
    {
        if (GetComponent<Dockable>() != null && GetComponent<Dockable>().DockedOn == null) return; // TODO : On Fail Interaction Event ? 

        target.OnInteraction.Invoke(GrabbedObject);
    }
}

#region Events
/**
 * Event invoked when the player interacts with an interactable object.
 * Interactable object is passed as a parameter.
 */
[Serializable] public class OnInteractEvent : UnityEvent<Interactable> {}

/**
 * Event invoked when the player teleports to a location.
 * Location is passed as a parameter as a Vector3.
 */
[Serializable] public class OnTeleportEvent : UnityEvent<Vector3> {}

/**
 * Event invoked when the player grabs a pickable object.
 * Pickable object is passed as a parameter.
 */

[Serializable] public class OnGrabEvent : UnityEvent<Pickable> {}
/**
 * Event invoked when the player releases a pickable object.
 * Docker object can be passed as a parameter of ReleasedEvent.
 */
[Serializable] public class OnReleaseEvent : UnityEvent<ReleasedEvent> {}

/**
 * Event describing a release.
 */
public class ReleasedEvent
{
    private Docker _releasedOn = null;
    private Pickable _releasedObject = null;

    public Docker ReleasedOn { get => _releasedOn; set => _releasedOn = value; }
    public Pickable ReleasedObject { get => _releasedObject; set => _releasedObject = value; }

    public ReleasedEvent(Pickable releasedObject, Docker releasedOn)
    {
        _releasedOn = releasedOn;
        ReleasedObject = releasedObject;
    }

    public ReleasedEvent(Pickable releasedObject) {
        _releasedObject = releasedObject;
    }

    public Docker GetDocker()
    {
        if (_releasedOn == null) return null;

        return _releasedOn.GetComponent<Docker>() ? _releasedOn.GetComponent<Docker>() : null;
    }
}
#endregion