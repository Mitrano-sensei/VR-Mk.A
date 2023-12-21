using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/**
 * Objects that can be interacted with.
 */
public class Interactable : MonoBehaviour
{
    [SerializeField] private OnInteractionEvent _onInteraction = new OnInteractionEvent();
    [SerializeField] private OnInteractionFailedEvent _onInteractionFailed = new OnInteractionFailedEvent();
    
    public delegate bool Condition();
    private List<Condition> _conditions = new List<Condition>();

    protected LogManager _logger;

    protected void Start()
    {
        _logger = LogManager.Instance;

        _onInteraction.AddListener((InteractEvent e) => { _logger.Trace("Interacted with " + name + (e.InteractedWith != null ? " while holding " + e.InteractedWith.name : "")); });
        _onInteractionFailed.AddListener((InteractEvent e) => { _logger.Trace("Failed to interact with " + name + (e.InteractedWith != null ? " while holding " + e.InteractedWith.name : "") + "because all conditions are not met."); });
    }

    /**
     * Manage the interaction of the object
     */
    public void Interact(InteractEvent interactEvent)
    {
        if (CheckConditions())
        {
            _onInteractionFailed.Invoke(interactEvent);
            return;
        }

        _onInteraction.Invoke(interactEvent);        
    }

    /**
     * Adds a condition to the interaction.
     * If the condition is not met, the interaction will fail.
     * 
     * TODO : Add more information, like the name of the condition, or the object that failed the condition.
     */
    public void AddCondition(Condition condition)
    {
        _conditions.Add(condition);
    }

    /**
     *  Check if all conditions are met before interacting.
     *  If no conditions are set, the interaction is always possible.
     *  To add a condition use AddCondition(() => { return myCondition; });
     */
    private bool CheckConditions()
    {
        var countCondition = _conditions.Count > 0;                     
        var checkConditionsList = _conditions.TrueForAll(c => c());

        // Can't get this to work yet, may need it later though
        // if (!checkConditionsList) 
        //     _logger.Trace("Condition from " + _conditions.Find(c => !c()).GetInvocationList()[0] + " is not met.");

        return countCondition && !checkConditionsList;
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

[Serializable] public class OnInteractionFailedEvent : UnityEvent<InteractEvent> { }
#endregion
