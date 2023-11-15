using System;
using System.Collections;
using System.Collections.Generic;
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