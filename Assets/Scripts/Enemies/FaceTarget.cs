using UnityEngine;

public class FaceTarget : MonoBehaviour
{
    private Vector3 _target;
    public Vector3 Target{ get => _target; set => _target = value; }

    public float RotationSpeed { get; set; }

    public bool IsActive { get; set; } = true;

    public void Start()
    {
    }

    public void Update()
    {
        if (_target != null) 
            RotateToTarget();
    }

    /**
     * Rotate the enemy to face the player, but only on the Y axis, only when the player is within a certain distance, and with a certain speed.
     */
    private void RotateToTarget()
    {
        if (_target == null)
            return;

        Vector3 direction = _target - transform.position;
        direction.y = 0;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * RotationSpeed);
    }

}
