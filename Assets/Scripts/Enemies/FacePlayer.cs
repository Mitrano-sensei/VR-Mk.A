using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    private Transform player;

    [SerializeField] private float _rotationSpeed = 1f;
    [SerializeField] private float _aggroDistance = 10f;

    public bool IsActive { get; set; } = true;

    public void Start()
    {
        if (player == null)
        {
            player = GameManager.Instance.Player;
        }
    }

    public void Update()
    {
        if (IsActive)
            RotateToPlayer();
    }

    /**
     * Rotate the enemy to face the player, but only on the Y axis, only when the player is within a certain distance, and with a certain speed.
     */
    private void RotateToPlayer()
    {
        if (player == null)
        {
            Debug.LogError("Player is null for SimpleEnemy");
            return;
        }

        Vector3 direction = player.position - transform.position;
        direction.y = 0;

        if (direction.magnitude < _aggroDistance)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _rotationSpeed);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _aggroDistance);
    }

}
