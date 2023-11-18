using DG.Tweening;
using Palmmedia.ReportGenerator.Core.Logging;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/**
 * Objects where dockable objects can dock to.
 */
public class Docker : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private OnDock _onDock = new OnDock();

    [Header("Materials")]
    [SerializeField] private Material _highlightBuyMaterial = null;
    [SerializeField] private Material _activeMaterial;
    [SerializeField] private Material _inactiveMaterial;

    private int _x;
    private int _y;
    private bool _isActive = true;
    private bool _isAvailable = true;

    public bool IsActive { get => _isActive; set => _isActive = value; }
    public bool IsAvailable { get => _isAvailable; set => _isAvailable = value; }
    public int X { get => _x; set => _x = value; }
    public int Y { get => _y; set => _y = value; }
    public OnDock OnDock { get => _onDock; set => _onDock = value; }

    private LogManager _logger;

    private void Awake()
    {
    }

    private void Start()
    {
        _logger = LogManager.Instance;

        if (_activeMaterial == null)
            _logger.Error("Active material is not set for " + gameObject.name);

        if (_inactiveMaterial == null)
            _logger.Error("Inactive material is not set for " + gameObject.name);

        SetActiveShader();

        OnDock.AddListener(OnDockHandler);
        OnDock.AddListener(OnUndockHandler);

        OnDock.AddListener(e => SetActiveShader());
    }

    private void SetActiveShader()
    {
        var renderer = GetComponent<MeshRenderer>();
        if (!IsAvailable)
        {
            // Make the dock invisible 
            renderer.enabled = false;
            return;
        }
        renderer.enabled = true;

        if (IsActive)
            renderer.material = _activeMaterial;
        else
            renderer.material = _inactiveMaterial;

        renderer.material.RandomizeTilingAndSpeed();
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
    public bool IsSecondary { get; set; } = false;
    public Dockable Dockable { get; set; }
    public DockType Type { get; set; }
}

[Serializable] public class OnDock : UnityEvent<OnDockEvent> {
    
}
#endregion
