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

    private OnInteractEvent _onInteractEvent = new OnInteractEvent();
    private OnTeleportEvent _onTeleportEvent = new OnTeleportEvent();
    private OnGrabEvent _onGrabEvent = new OnGrabEvent();
    private OnReleaseEvent _onReleaseEvent = new OnReleaseEvent();

    public OnInteractEvent OnInteractEvent { get => _onInteractEvent; }
    public OnTeleportEvent OnTeleportEvent { get => _onTeleportEvent; }
    public OnGrabEvent OnGrabEvent { get => _onGrabEvent; }
    public OnReleaseEvent OnReleaseEvent { get => _onReleaseEvent; }
    protected Pickable GrabbedObject { get => grabbedObject; set => grabbedObject = value; }

    protected override void Awake()
    {
        base.Awake();
        GrabbedObject = null;

        OnGrabEvent.AddListener((Pickable pickable) =>
        {
            GrabbedObject = pickable;
            pickable.OnPick.Invoke();
        });

        OnReleaseEvent.AddListener((Docker docker) =>
        {
            GrabbedObject.OnUnPick.Invoke();
            GrabbedObject = null;

            var dockable = GrabbedObject.GetComponent<Dockable>();
            if (dockable != null)
            {
                dockable.OnDock.Invoke(docker);
            }
        });

        OnInteractEvent.AddListener((Interactable target) =>
        {
            target.OnInteraction.Invoke(GrabbedObject);
        });
    }
}

#region Events
/**
 * Event invoked when the player interacts with an interactable object.
 * Interactable object is passed as a parameter.
 */
public class OnInteractEvent : UnityEvent<Interactable> {}
/**
 * Event invoked when the player teleports to a location.
 * Location is passed as a parameter as a Vector3.
 */
public class OnTeleportEvent : UnityEvent<Vector3> {}
/**
 * Event invoked when the player grabs a pickable object.
 * Pickable object is passed as a parameter.
 */
public class OnGrabEvent : UnityEvent<Pickable> {}
/**
 * Event invoked when the player releases a pickable object.
 * Docker is where the object is released is passed as a parameter, null if it is not a docker.
 */
public class OnReleaseEvent : UnityEvent<Docker> {}

#endregion