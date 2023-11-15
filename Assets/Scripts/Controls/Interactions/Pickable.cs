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

    public OnPickEvent OnPick { get => _onPick; }
    public OnUnPickEvent OnUnPick { get => _onUnPick; }

    protected void Start()
    {
        Debug.Log("test " + gameObject.name);

        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if (!rb) Debug.Log(":(((((");

        OnPick.AddListener(() =>
        {
            Debug.Log("1");
            rb.isKinematic = true;
        });

        OnUnPick.AddListener(() => {
            Debug.Log("2 " + (rb.isKinematic? "oui" : "non"));
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
