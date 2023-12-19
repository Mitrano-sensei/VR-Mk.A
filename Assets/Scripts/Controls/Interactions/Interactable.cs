using System;
using UnityEngine;
using UnityEngine.Events;

/**
 * Objects that can be interacted with.
 */
public class Interactable : MonoBehaviour
{
    [SerializeField] private OnInteractionEvent _onInteraction = new OnInteractionEvent();

    protected LogManager _logger;

    protected void Start()
    {
        _logger = LogManager.Instance;

        _onInteraction.AddListener((InteractEvent e) => { _logger.Trace("Interacted with " + name + (e.InteractedWith != null ? " while holding " + e.InteractedWith.name : "")); });
    }

    /**
     * Manage the interaction of the object
     */
    public void Interact(InteractEvent interactEvent)
    {
        var dockable = GetComponent<Dockable>();

        if (dockable != null && dockable.DockedOn.Count == 0)
        { 
            _logger.Trace("Tried to interact with " + name + " while not docked");
            return;
        }

        _onInteraction.Invoke(interactEvent);        
    }
}

#region Events
/**
 * Event invoked when the player interacts with an interactable object. 
 * The player can hold a pickable item while doing it, if so it is passed as a parameter, else it is null.
 */
[Serializable] public class OnInteractionEvent : UnityEvent<InteractEvent> { }

public class InteractEvent
{
    public Pickable InteractedWith { get; set; }
    public InteractEvent(Pickable pickable)
    {
        InteractedWith = pickable;
    }
}
#endregion
