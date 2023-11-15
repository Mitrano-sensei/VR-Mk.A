using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.Events;

/**
 * Objects that can be docked to a docker.
 */
public class Dockable : Pickable
{
    private OnDockEvent _onDock = new OnDockEvent();
    public OnDockEvent OnDock { get => _onDock; }

    private void Awake()
    {
        OnDock.AddListener((Docker docker) =>
        {
            // TODO : DOTWeen
            GetComponent<Rigidbody>().isKinematic = true;
            transform.SetParent(docker.transform);
            transform.localPosition = new UnityEngine.Vector3(0, 0, 0);
            transform.rotation = UnityEngine.Quaternion.identity;
            // TODO : Do rotation as it should be
        });
    }

}

#region Events
/**
 * Event invoked when a dockable object is docked to a docker.
 * The docker is passed as a parameter.
 */
[Serializable] public class OnDockEvent : UnityEvent<Docker> { }
#endregion