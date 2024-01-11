using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Damageable))]
public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _currentHealth;

    [Header("Events")]
    [SerializeField] private bool _destroyOnDeath = true;
    [SerializeField] private OnDeath _onDeath = new OnDeath();
    public OnDeath OnDeath { get { return _onDeath; } }

    private Damageable _damageable;
    private LogManager _logger;

    public void Start()
    {
        _damageable = GetComponent<Damageable>();
        _currentHealth = _maxHealth;
        _logger = LogManager.Instance;

        OnDeath.AddListener(deathEvent =>
        {
            _logger.Trace(gameObject.name + " has died.");
        });

        _damageable.OnDamageTaken.AddListener(damageEvent => {
            _currentHealth -= damageEvent.Damage;
            if ( _currentHealth <= 0)
            {
                OnDeath.Invoke(new DeathEvent());

                if (_destroyOnDeath)
                {
                    Destroy(gameObject);
                }
            }
        });
    }
}

#region Events

[Serializable] public class OnDeath : UnityEvent<DeathEvent> { }

public class DeathEvent
{
    public DeathEvent()
    {
    }
}

#endregion
