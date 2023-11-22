using Palmmedia.ReportGenerator.Core.Logging;
using System;
using UnityEngine;
using UnityEngine.Events;

/**
 * Objects that can be interacted with.
 */
public class Interactable : MonoBehaviour
{
    [SerializeField] private OnInteractionEvent _onInteraction = new OnInteractionEvent();
    public OnInteractionEvent OnInteraction { get => _onInteraction; }

    protected LogManager _logger;

    protected void Start()
    {
        _logger = LogManager.Instance;

        OnInteraction.AddListener((InteractEvent e) => { _logger.Trace("Interacted with " + name + (e.InteractedWith != null ? " while holding " + e.InteractedWith.name : "")); });
        OnInteraction.AddListener(HandleBaseInteraction);
    }

    public void HandleBaseInteraction(InteractEvent interactEvent)
    {
        var usable = interactEvent.InteractedWith?.GetComponent<UsableItem>();
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
