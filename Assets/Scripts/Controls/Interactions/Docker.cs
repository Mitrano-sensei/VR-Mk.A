using System;
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
    private bool _isAvailable = true;

    public bool IsActive { get => _isActive; set => _isActive = value; }
    public bool IsAvailable { get => _isAvailable; set => _isAvailable = value; }
    public int X { get => _x; set => _x = value; }
    public int Y { get => _y; set => _y = value; }
    public OnDock OnDock { get => _onDock; set => _onDock = value; }

    private void Start()
    {
        OnDock.AddListener(OnDockHandler);
        OnDock.AddListener(OnUndockHandler);
    }

    private void Update()
    {
        if (!IsActive)
            GetComponent<Renderer>().material.color = Color.gray;
        else
        {
            if (IsAvailable)
                GetComponent<Renderer>().material.color = Color.green;
            else
                GetComponent<Renderer>().material.color = Color.red;
        }
    }

    private void OnDockHandler(OnDockEvent onDockEvent)
    {
        if (onDockEvent.Type == OnDockEvent.DockType.UNDOCK)
            return;
        
        IsAvailable = false;
    }

    private void OnUndockHandler(OnDockEvent onDockEvent)
    {
        if (onDockEvent.Type == OnDockEvent.DockType.DOCK)
            return;
        
        IsAvailable = true;
        var dockable = onDockEvent.Dockable;
        dockable.transform.SetParent(dockable.OriginParent);
    }
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
