using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Transform _cameraPosition;

    void Update()
    {
        transform.position = _cameraPosition.position;
    }
}
