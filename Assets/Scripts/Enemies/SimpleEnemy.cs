using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(EnemyHealth), typeof(FaceTarget), typeof(MechRocketLauncher))]
public class SimpleEnemy : MonoBehaviour
{
    private MechRocketLauncher _rocketLauncher;
    private EnemyState _state = EnemyState.Wandering;
    private EnemyHealth _health;
    private FaceTarget _faceTarget;

    private GameManager _gameManager;
    private LogManager _logger;

    [Header("Events")]
    public UnityEvent<EnemyState> OnStateChanged = new();

    [Header("Wandering Stats")]

    [Tooltip("The speed at which the enemy will rotate towards their target")]
    [SerializeField] private float _wanderRotationSpeed = 3f;
    [Tooltip("The angle at which the enemy will start moving towards their target")]
    [SerializeField] private float _wanderAngleToMove = 5f;
    [Tooltip("The speed at which the enemy will move towards their target")]
    [SerializeField] private float _wanderMoveSpeed = 1f;
    [Tooltip("The distance at which the enemy will consider it has reached their target")]
    [SerializeField] private float _wanderDistanceToTarget = .7f;
    [Tooltip("The distance at which the enemy will start to aggro the player and start locking")]
    [SerializeField] private float _wanderAggroRange = 10f;

    [Header("Wandering Paths")]
    [SerializeField] private Transform[] _path;

    [Header("Locking Stats")]
    [Tooltip("The speed at which the enemy will rotate towards the player")]
    [SerializeField] private float _lockRotationSpeed = 3f;
    [Tooltip("The angle at which the enemy will start charging")]
    [SerializeField] private float _lockAngle = 5f;


    public void Start()
    {
        _rocketLauncher = GetComponent<MechRocketLauncher>();
        _health = GetComponent<EnemyHealth>();
        _faceTarget = GetComponent<FaceTarget>();

        _gameManager = GameManager.Instance;
        _logger = LogManager.Instance;

        _health.OnDeath.AddListener(deathEvent =>
        {
            ChangeState(EnemyState.Dead);
        });
    }

    public void Update()
    {
        switch (_state)
        {
            case EnemyState.Wandering:
                Wander();
                if (WatchForPlayer()) ChangeState(EnemyState.Locking);
                break;
            case EnemyState.Locking:
                Lock();
                break;
            case EnemyState.Charging:
                break;
            case EnemyState.Attacking:
                break;
            case EnemyState.Dead:
                break;
            default:
                Debug.LogError("Unknown State :/");
                break;
        }
    }

    private int _currentPathIndex = 0;
    /**
     * Wanders in his path (in circle).
     * The enemy will rotate towards the next point in the path, and then move towards it.
     * The movement is really simple, won't take into account obstacles.
     */
    private void Wander()
    {
        if (_path == null || _path.Length == 0)
            return;

        // Verify if we're close enough to the next point in the path
        if (Vector3.Distance(transform.position, _path[_currentPathIndex].position.With(y:transform.position.y)) < _wanderDistanceToTarget)
        {
            _logger.Trace("Wander - Reached !");
            // If we are, increment the index
            _currentPathIndex++;
            // If we've reached the end of the path, loop back to the beginning
            if (_currentPathIndex >= _path.Length)
                _currentPathIndex = 0;
        }

        var nextPosition = _path[_currentPathIndex].position.With(y:transform.position.y);

        // Verify if the enemy is facing the target
        var direction = nextPosition - transform.position;
        direction.y = 0;
        var angle = Vector3.Angle(transform.forward, direction);
        
        if (angle < _wanderAngleToMove)
        {
            _logger.Trace("Wander - Walking");
            // Walks towards the target
            transform.position = Vector3.MoveTowards(transform.position, nextPosition, _wanderMoveSpeed * Time.deltaTime);
        }
        else
        {
            // Set the target to the next point in the path
            _faceTarget.RotationSpeed = _wanderRotationSpeed;
            _faceTarget.Target = nextPosition;
            _logger.Trace("Wander - Rotating");
        }
    }

    private Vector3? _lockTargetPosition;
    /**
     * Rotates to the player until it's facing him, then starts charging.
     */
    private void Lock()
    {
        if (_lockTargetPosition == null)
            _lockTargetPosition = _gameManager.Player.transform.position;

        var lockTargetPosition = _lockTargetPosition ?? _gameManager.Player.transform.position; // So Vector3 is not nullable

        if (Vector3.Angle(transform.forward, lockTargetPosition - transform.position) < _lockAngle)
        {
            // If the enemy is facing the player, start charging
            ChangeState(EnemyState.Charging);

            // Prepare next lock
            _lockTargetPosition = null;
        }
        else
        {
            _logger.Trace("Lock - Turning to player");
            // Rotates to the player until it's facing him
            _faceTarget.RotationSpeed = _lockRotationSpeed;
            _faceTarget.Target = lockTargetPosition;
        }
    }

    public bool WatchForPlayer()
    {
        if (_state == EnemyState.Wandering)
        {
              // Verify if the player is within the aggro range
            var player = _gameManager.Player;
            if (player == null)
            {
                Debug.LogError("Player is null");
                return false;
            }

            var distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < _wanderAggroRange)
            {
                ChangeState(EnemyState.Locking);
                return true;
            }
        }

        return false;
    }

    public void ChangeState(EnemyState newState)
    {
        _state = newState;
        OnStateChanged.Invoke(_state);
    }

    public void DebugFire()
    {
        _rocketLauncher.Fire();
    }

    public void OnDrawGizmosSelected()
    {
        if (_state == EnemyState.Wandering)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _wanderAggroRange);
        }
    }

    public enum EnemyState
    {
        Wandering,
        Locking,
        Charging,
        Attacking,
        Dead
    }
}


#region Events
[Serializable] public class OnAttack : UnityEvent<OnAttackEvent> { }
public class OnAttackEvent
{

}

[Serializable] public class OnCharge : UnityEvent<OnChargeEvent> { }
public class OnChargeEvent
{

}

#endregion