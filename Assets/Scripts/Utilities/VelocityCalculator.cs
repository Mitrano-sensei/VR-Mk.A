using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

/**
 * Compute the average speed of the object over the last _bufferSize frames.
 */
public class VelocityCalculator : MonoBehaviour
{
    [Header("Parameters")]
    [Tooltip("Number of frames used to compute velocity")]
    [SerializeField] private int _bufferSize = 5;

    public Vector3 Velocity { get => ComputeVelocity(); }
    public Queue<Vector3> LastPositions { get => _lastPositions; set => _lastPositions = value; }
    public Queue<float> LastDeltaTime { get => _lastDeltaTime; set => _lastDeltaTime = value; }

    private Queue<Vector3> _lastPositions = new();
    private Queue<float> _lastDeltaTime = new();

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

    /**
     * Compute the velocity over the last frames using their respective deltaTimes.
     * Used in the Velocity getter.
     */
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
