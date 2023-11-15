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

    protected override void Start()
    {
        base.Start();

        if (_correctRotation == null) _correctRotation = new UnityEngine.Vector3(0, 0, 0);

        OnDock.AddListener((Docker docker) =>
        {
            transform.SetParent(docker.transform);
            transform.DOMove(docker.transform.position, 1f).SetEase(Ease.InFlash);
            transform.DORotate(_correctRotation, 1f).SetEase(Ease.InElastic);
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