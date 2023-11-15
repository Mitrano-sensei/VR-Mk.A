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

    [SerializeField] private OnDockEvent _onDock = new OnDockEvent();
    public OnDockEvent OnDock { get => _onDock; }

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

    }

}

#region Events
/**
 * Event invoked when a dockable object is docked to a docker.
 * The docker is passed as a parameter.
 */
[Serializable] public class OnDockEvent : UnityEvent<Docker> { }
#endregion