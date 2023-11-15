using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
 * Objects that can be interacted with.
 * */
public class Interactable : MonoBehaviour
{
    [SerializeField] private OnInteractionEvent _onInteraction = new OnInteractionEvent();
    public OnInteractionEvent OnInteraction { get => _onInteraction; }
}

#region Events
/**
 * Event invoked when the player interacts with an interactable object. 
 * The player can hold a pickable item while doing it, if so it is passed as a parameter, else it is null.
 */
[Serializable] public class OnInteractionEvent : UnityEvent<Pickable> { }
#endregion
