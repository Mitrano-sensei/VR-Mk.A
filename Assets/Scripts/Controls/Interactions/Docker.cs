using System;
using System.ComponentModel;
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
    [Description("The material that will be applied to the docker when it is highlighted for purchase")]
    [SerializeField] private Material _highlightBuyMaterial = null;
    [Description("The material that will be applied to the docker when it is active, i.e. when it is bought and available")]
    [SerializeField] private Material _activeMaterial;
    [Description("The material that will be applied to the docker when it is inactive, i.e. when it is not bought yet")]
    [SerializeField] private Material _inactiveMaterial;


    private Dockable _dockedObject = null;
    private bool _isActive = true;
    private bool _isAvailable = true;

    public bool IsActive { get => _isActive; set => _isActive = value; }
    public bool IsAvailable { get => _isAvailable; set => _isAvailable = value; }
    public Dockable DockedObject { get => _dockedObject; set => _dockedObject = value; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
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

        transform.localRotation = Quaternion.identity;
    }

    /**
     * Make the docker glow the right color.
     */
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

    /**
     * Basic handler.
     * When a dockable object is docked to this docker.
     */
    private void OnDockHandler(OnDockEvent onDockEvent)
    {
        if (onDockEvent.Type == OnDockEvent.DockType.UNDOCK)
            return;
        
        IsAvailable = false;
        DockedObject = onDockEvent.Dockable;
    }

    /**
     * Basic handler.
     * When a dockable object is undocked from this docker.
     */
    private void OnUndockHandler(OnDockEvent onDockEvent)
    {
        if (onDockEvent.Type == OnDockEvent.DockType.DOCK)
            return;

        DockedObject = null;
        IsAvailable = true;
        var dockable = onDockEvent.Dockable;
        dockable.transform.SetParent(dockable.OriginParent);
    }

    /**
     * Eject the docked object.
     */
    public void Eject()
    {
        if (DockedObject == null) return;

        var dockable = DockedObject;
        var floatingDirection = GetComponent<FloatMovement>().Direction.normalized;
        
        OnDock.Invoke(new OnDockEvent(dockable, OnDockEvent.DockType.UNDOCK));
        dockable.OnEject.Invoke(new EjectEvent(floatingDirection));

        _logger.Trace("Ejecting " + dockable.name + " from " + name);
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

    public OnDockEvent(Dockable dockable, DockType type)
    {
        Dockable = dockable;
        Type = type;
    }

    public OnDockEvent()
    {
        Dockable = null;
    }
}

/**
 * Event that is invoked when a dockable object is docked or undocked to this docker.
 */
[Serializable] public class OnDock : UnityEvent<OnDockEvent> {}
#endregion
