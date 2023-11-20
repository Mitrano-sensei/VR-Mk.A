using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityCalculator : MonoBehaviour
{
    private Queue<Vector3> _lastPositions = new();
    [SerializeField]
    private int _bufferSize = 5;

    public Vector3 Velocity { get => ComputeVelocity(); }
    public Queue<Vector3> LastPositions { get => _lastPositions; set => _lastPositions = value; }

    void FixedUpdate()
    {
        LastPositions.Enqueue(gameObject.transform.position);

        

        if(LastPositions.Count > _bufferSize)
        {
            LastPositions.Dequeue();
        }
    }

    private Vector3 ComputeVelocity()
    {
        var list = LastPositions.ToArray();
        Vector3 mean = new();

        for (int i = 0; i < list.Length - 1; i++)
        {
            mean += (list[i + 1] - list[i]) / Time.fixedDeltaTime;
        }
        mean /= list.Length - 1;

        return mean;
    }
}
