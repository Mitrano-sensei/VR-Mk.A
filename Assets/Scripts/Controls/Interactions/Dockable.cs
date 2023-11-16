using DG.Tweening;
using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.Events;

/**
 * Objects that can be docked to a docker.
 */
public class Dockable : Pickable
{
    [SerializeField] private UnityEngine.Vector3 _correctRotation;

    [SerializeField] private OnDocking _onDock = new OnDocking();
    public OnDocking OnDock { get => _onDock; }

    private Rigidbody _rb;

    protected override void Start()
    {
        base.Start();
        _rb = GetComponent<Rigidbody>();
        if (_rb == null) Debug.LogError("Rigidbody missing on " + gameObject.name);

        if (_correctRotation == null) _correctRotation = new UnityEngine.Vector3(0, 0, 0);

        OnDock.AddListener(DockToObject);
    }

    private void DockToObject(Docker docker) {
        _rb.isKinematic = true;
        transform.SetParent(docker.transform);
        Sequence sequence = DOTween.Sequence()
                    .Append(transform.DOMove(docker.transform.position, 1f).SetEase(Ease.InQuad))
                    .Join(transform.DORotate(_correctRotation, 1f).SetEase(Ease.InQuad))
                    .OnComplete(() =>
                    {
                        transform.SetParent(docker.transform); // To be sure
                        transform.localPosition = new();
                    });

        var onDockEvent = new OnDockEvent();
        onDockEvent.Type = OnDockEvent.DockType.DOCK;
        onDockEvent.Dockable = this;

        // TODO : Manage Undocking

        docker.OnDock.Invoke(onDockEvent);
    }

    private void UndockObject(Docker docker)
    {
        var onUndockEvent = new OnDockEvent();
        onUndockEvent.Type = OnDockEvent.DockType.UNDOCK;
        onUndockEvent.Dockable = this;
        docker.OnDock.Invoke(onUndockEvent);
    }

}

#region Events
/**
 * Event invoked when a dockable object is docked to a docker.
 * The docker is passed as a parameter.
 */
[Serializable] public class OnDocking : UnityEvent<Docker> { }
#endregion