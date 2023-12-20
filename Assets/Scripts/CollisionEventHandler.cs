using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


// Send events when colliding

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class CollisionEventHandler : MonoBehaviour
{
    private Rigidbody _rb;
    [SerializeField]
    private OnCollision _onCollision;

    public OnCollision OnCollision { get => _onCollision; }

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        if (_onCollision == null)
        {
            _onCollision = new OnCollision();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnCollision.Invoke(new CollisionEvent(collision));
    }
}

#region Events

[Serializable] public class OnCollision : UnityEvent<CollisionEvent> { }


public class CollisionEvent
{
    private Collision _collision;

    public CollisionEvent(Collision collision)
    {
        _collision = collision;
    }

    public Collision Collision { get => _collision; }

    
}

#endregion