using DG.Tweening;
using System.Collections.Generic;
using Unity.XR.PXR;
using Unity.XR.PXR.Input;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class PicoControlWatcher : AbstractControlWatcher
{
    [Header("Player")]
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _hand;

    private List<InputDevice> _devices;

    protected override void Awake()
    {
        base.Awake();
    }

    protected void Start()
    {
        if (_player == null) Debug.LogError("Player is null");
        if (_hand == null) Debug.LogError("Hand is null");

        // Pico XR
        _devices = new List<InputDevice>();
        RegisterDevices();

        // Events
        OnTeleportEvent.AddListener((Vector3 newPos) =>
        {
            _player.transform.position = newPos;
        });

        Sequence mover = null;
        OnGrabEvent.AddListener((Pickable pickable) =>
        {
            var dockable = pickable.GetComponent<Dockable>();

            pickable.transform.SetParent(_hand.transform);
            mover = DOTween.Sequence();
            mover = mover.Append(pickable.transform.DOLocalMove(Vector3.forward + (dockable != null ? dockable.CenterPosition * 0.15f : Vector3.zero), .5f).SetEase(Ease.InOutQuad));

            if (dockable != null)
            {
                var rotationOffset = new Vector3(-90, 0, 0); // Because Correct rotation is for the item to dock, not to be picked. 
                mover.Join(pickable.transform.DOLocalRotate(-dockable.CorrectRotation + rotationOffset, .5f).SetEase(Ease.InOutQuad));
            }

            Helpers.MoveUntilDie(pickable.transform, _hand, mover);
        });

        OnReleaseEvent.AddListener((ReleasedEvent releasedEvent) =>
        {
            mover?.Kill();
            mover = null;       // Double check x)

            if (releasedEvent.GetDocker() != null && releasedEvent.GetDocker().IsActive && releasedEvent.GetDocker().IsAvailable) return;     // Here we let base OnDock event handle the docking.

            var releasedObject = releasedEvent.ReleasedObject;
            var rb = releasedObject.GetComponent<Rigidbody>();
            releasedObject.transform.SetParent(releasedObject.OriginParent != null ? releasedObject.OriginParent : _cockpitEnvironment.transform);
        });
    }

    protected void Update()
    {
        bool triggerButtonState = false;
        bool menuButtonState = false;
        bool arrowButtonState = false;
        
        foreach (var device in _devices)
        {
            device.TryGetFeatureValue(CommonUsages.triggerButton, out triggerButtonState);
            device.TryGetFeatureValue(CommonUsages.menuButton, out menuButtonState);
            device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out arrowButtonState);
        }

        if (arrowButtonState)
        {
            // Raycast from hand, if hit, teleport to hit point.
            RaycastHit hit;
            if (Helpers.HitBehindGrabbedObjectFromHand(_hand, GrabbedObject?.gameObject, out hit))
            {
                // If the object is a teleporter, invoke the event.
                if (hit.collider.gameObject.GetComponent<TeleportationArea>())
                {
                    OnTeleportEvent.Invoke(hit.point);
                }
            }
        }





    }


    private void DeviceConnected(InputDevice device)
    {
        bool discardedValue;

        if (device.TryGetFeatureValue(CommonUsages.menuButton, out discardedValue))
        {
            _devices.Add(device);
        }
    }

    private void DeviceDisconnected(InputDevice device)
    {
        if (_devices.Contains(device))
        {
            _devices.Remove(device);
        }
    }

    private void RegisterDevices()
    {
        var allDevices = new List<InputDevice>();
        InputDevices.GetDevices(allDevices);
        foreach (var device in allDevices)
        {
            DeviceConnected(device);
        }

        InputDevices.deviceDisconnected += DeviceDisconnected;
        InputDevices.deviceConnected += DeviceConnected;
    }

}
