using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
 * Objects that can be picked up.
 */
public class Pickable : MonoBehaviour
{
    private OnPickEvent _onPick = new OnPickEvent();
    private OnUnPickEvent _onUnPick = new OnUnPickEvent();

    public OnPickEvent OnPick { get => _onPick; }
    public OnUnPickEvent OnUnPick { get => _onUnPick; }
}

#region Events
/**
 * Event invoked when a pickable object is picked up.
 */
public class OnPickEvent : UnityEvent { }
/**
 * Event invoked when a pickable object is released.
 */
public class OnUnPickEvent : UnityEvent { }
#endregion
