using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;
using static UnityEngine.GraphicsBuffer;

/**
 * Controls watcher for the FPS controls.
 **/
public class FPSControlsWatcher : AbstractControlWatcher
{
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _cockpitEnvironment;

    private LogManager _logger;

    protected override void Awake()
    {
        base.Awake();
        _logger = LogManager.Instance;
    }

    public void Start()
    {
        OnTeleportEvent.AddListener((Vector3 position) =>
        {
            _logger.Trace("Teleportation to " + position);
        });

        OnGrabEvent.AddListener((Pickable pickable) =>
        {
            _logger.Trace("Grabbing " + pickable.name);
            this.GrabbedObject = pickable;
        });

        OnReleaseEvent.AddListener((ReleasedEvent releasedEvent) =>
        {
            var docker = releasedEvent.GetDocker();

            if (docker != null)
            {
                _logger.Trace("Releasing on " + docker.name);
            }
            else
            {
                _logger.Trace("Releasing");
            }
        });

        OnInteractEvent.AddListener((Interactable interactable) =>
        {
            _logger.Trace("Interacting with " + interactable.name);
        });

        OnTeleportEvent.AddListener((Vector3 newPos) =>
        {
            _player.transform.position = newPos;
        });

        Sequence mover = null;

        OnGrabEvent.AddListener((Pickable pickable) =>
        {
            pickable.transform.SetParent(Camera.main.transform);
            mover = DOTween.Sequence();
            mover = mover.Append(pickable.transform.DOLocalMove(Vector3.forward, .5f).SetEase(Ease.InOutQuad));
            MoveUntilDie(pickable.transform, Camera.main.gameObject, mover);
            //pickable.GetComponent<Rigidbody>().isKinematic = true;
        });

        OnReleaseEvent.AddListener((ReleasedEvent releasedEvent) =>
        {
            mover?.Kill();
            mover = null;

            if (releasedEvent.GetDocker() != null) return;     // Here we let base OnDock event handle the docking.

            var releasedObject = releasedEvent.ReleasedObject;
            var rb = releasedObject.GetComponent<Rigidbody>();
            releasedObject.transform.SetParent(releasedObject.OriginParent ?  releasedObject.OriginParent : _cockpitEnvironment.transform);
        });
    }

    private void Update()
    {
        // Calls Teleport event if the player presses the right mouse button.
        if (Input.GetMouseButtonDown(1))
        {
            _logger.Trace("Right button");

            // Raycast from main camera to next object.
            RaycastHit hit;
            if (HitBehindGrabbedObject(GrabbedObject?.gameObject, out hit))
            {
                // If the object is a teleporter, invoke the event.
                if (hit.collider.gameObject.GetComponent<TeleportationArea>())
                {
                    OnTeleportEvent.Invoke(hit.point);
                }
            }
        }

        // Calls Grab Event when the player presses E and is looking at a pickable object.
        if (Input.GetKeyDown(KeyCode.E) && this.GrabbedObject == null)
        {
            _logger.Trace("E Button");

            // Raycast from main camera to next object.
            RaycastHit hit;
            if (HitBehindGrabbedObject(GrabbedObject?.gameObject, out hit))
            {
                // If the object is a pickable, invoke the event.
                if (hit.collider.gameObject.GetComponent<Pickable>())
                {
                    OnGrabEvent.Invoke(hit.collider.gameObject.GetComponent<Pickable>());
                }
            }
        }
        else if (this.GrabbedObject != null && !Input.GetKey(KeyCode.E))
        {
            _logger.Trace("Releasing Button");

            // Raycast from main camera to next object.
            RaycastHit hit;
            HitBehindGrabbedObject(GrabbedObject?.gameObject, out hit);
            var docker = hit.collider?.gameObject?.GetComponent<Docker>(); 
            OnReleaseEvent.Invoke(docker == null ? new ReleasedEvent(GrabbedObject) : new ReleasedEvent(GrabbedObject, docker));
        }

        // Calls Interact Event when the player presses left click and is looking at an interactable object.
        if (Input.GetMouseButtonDown(0))
        {
            _logger.Trace("Left Mouse Button");

            // Raycast from main camera to next object.
            RaycastHit hit;
            if (HitBehindGrabbedObject(GrabbedObject?.gameObject, out hit))
            {
                Interactable interactable = hit.collider.gameObject.GetComponent<Interactable>();
                // If the object is an interactable, invoke the event.
                if (interactable != null)
                {
                    OnInteractEvent.Invoke(interactable);
                }
            }
        }
    }

    /**
     * Cast a ray from the camera to the back of the grabbed object.
     */
    private bool HitBehindGrabbedObject(GameObject grabbedObject, out RaycastHit hit)
    {

        if (!grabbedObject) return Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit); ;

        int oldLayer = grabbedObject.layer;
        grabbedObject.layer = 2;
        bool b = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, ~(1 << 2));
        grabbedObject.layer = oldLayer;
        
        return b;
    }

    internal IEnumerator MoveUntilDie(Transform myTransform, GameObject target, Sequence tweener)
    {
        while (tweener != null)
        {
            tweener.Restart();
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    
}
