using System;
using UnityEngine;
using UnityEngine.Events;

/**
 * Objects that can be picked up.
 */
public class Pickable : MonoBehaviour
{
    [SerializeField] private OnPickEvent _onPick = new OnPickEvent();
    [SerializeField] private OnUnPickEvent _onUnPick = new OnUnPickEvent();

    private Transform _originParent;

    public OnPickEvent OnPick { get => _onPick; }
    public OnUnPickEvent OnUnPick { get => _onUnPick; }
    public Transform OriginParent { get => _originParent; }

    protected virtual void Start()
    {
        _originParent = transform.parent;
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();

        OnPick.AddListener(() =>
        {
            rb.isKinematic = true;
        });

        OnUnPick.AddListener(() => {
            rb.isKinematic = false;
        });
    }
}

#region Events
/**
 * Event invoked when a pickable object is picked up.
 */
[Serializable] public class OnPickEvent : UnityEvent { }
/**
 * Event invoked when a pickable object is released.
 */
[Serializable] public class OnUnPickEvent : UnityEvent { }
#endregion
