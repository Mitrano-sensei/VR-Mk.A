using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

/**
 * Controls watcher for the FPS controls.
 **/
public class FPSControlsWatcher : AbstractControlWatcher
{
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _cockpitEnvironment;

    public void Start()
    {
        OnTeleportEvent.AddListener((Vector3 position) =>
        {
            Debug.Log("Teleportation to " + position);
        });

        OnGrabEvent.AddListener((Pickable pickable) =>
        {
            Debug.Log("Grabbing " + pickable.name);
            this.GrabbedObject = pickable;
        });

        OnReleaseEvent.AddListener((ReleasedEvent releasedEvent) =>
        {
            var docker = releasedEvent.GetDocker();

            if (docker != null)
            {
                Debug.Log("Releasing on " + docker.name);
            }
            else
            {
                Debug.Log("Releasing");
            }
        });

        OnInteractEvent.AddListener((Interactable interactable) =>
        {
            Debug.Log("Interacting with " + interactable.name);
        });

        OnTeleportEvent.AddListener((Vector3 newPos) =>
        {
            _player.transform.position = newPos;
        });

        OnGrabEvent.AddListener((Pickable pickable) =>
        {
            pickable.transform.SetParent(Camera.main.transform);
            pickable.transform.position = Camera.main.transform.position + Camera.main.transform.forward;
            pickable.GetComponent<Rigidbody>().isKinematic = true;
        });

        OnReleaseEvent.AddListener((ReleasedEvent releasedEvent) =>
        {
            if (releasedEvent.GetDocker() == null)
            {
                var releasedObject = releasedEvent.ReleasedObject;
                var rb = releasedObject.GetComponent<Rigidbody>();
                releasedObject.transform.SetParent(_cockpitEnvironment.transform);
                rb.isKinematic = false;
                return;
            }

            // Here we know we are releasing on a docker.

            
        });
    }

    private void Update()
    {
        // Calls Teleport event if the player presses the right mouse button.
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right button");

            // Raycast from main camera to next object.
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
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
            Debug.Log("E Button");

            // Raycast from main camera to next object.
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
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
            Debug.Log("Releasing Button");

            // Raycast from main camera to next object.
            RaycastHit hit;
            Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit);
            var docker = hit.collider?.gameObject?.GetComponent<Docker>(); 
            OnReleaseEvent.Invoke(docker == null ? new ReleasedEvent(GrabbedObject) : new ReleasedEvent(GrabbedObject, docker));
        }

        // Calls Interact Event when the player presses left click and is looking at an interactable object.
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Left Mouse Button");

            // Raycast from main camera to next object.
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
            {
                // If the object is an interactable, invoke the event.
                if (hit.collider.gameObject.GetComponent<Interactable>())
                {
                    OnInteractEvent.Invoke(hit.collider.gameObject.GetComponent<Interactable>());
                }
            }
        }
    }
}
