using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityCalculator : MonoBehaviour
{
    private Queue<Vector3> _lastPositions = new();
    private Queue<float> _lastDeltaTime = new();
    [SerializeField]
    private int _bufferSize = 5;

    public Vector3 Velocity { get => ComputeVelocity(); }
    public Queue<Vector3> LastPositions { get => _lastPositions; set => _lastPositions = value; }
    public Queue<float> LastDeltaTime { get => _lastDeltaTime; set => _lastDeltaTime = value; }

    void Update()
    {
        LastPositions.Enqueue(gameObject.transform.position);
        LastDeltaTime.Enqueue(Time.deltaTime);

        if(LastPositions.Count > _bufferSize)
        {
            LastPositions.Dequeue();
        }
        if(LastDeltaTime.Count > _bufferSize)
        {
            LastDeltaTime.Dequeue();
        }
    }

    private Vector3 ComputeVelocity()
    {
        var positionList = LastPositions.ToArray();
        var timeList = LastDeltaTime.ToArray();
        Vector3 mean = new();

        for (int i = 0; i < positionList.Length - 1; i++)
        {
            mean += (positionList[i + 1] - positionList[i]) / timeList[i+1];
        }
        mean /= positionList.Length - 1;

        return mean;
    }
}
