using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

/**
 * Objects where dockable objects can dock to.
 */
public class Docker : MonoBehaviour
{
    [SerializeField] private OnDock _onDock = new OnDock();
    private int _x;
    private int _y;
    private bool _isActive = true;

    public bool IsActive { get => _isActive; set => _isActive = value; }
    public int X { get => _x; set => _x = value; }
    public int Y { get => _y; set => _y = value; }
    public OnDock OnDock { get => _onDock; set => _onDock = value; }
}

#region Events
public class OnDockEvent { 
    public enum DockType
    {
        DOCK,
        UNDOCK
    }

    public Dockable Dockable { get; set; }
    public DockType Type { get; set; }
}

[Serializable] public class OnDock : UnityEvent<OnDockEvent> { }
#endregion
