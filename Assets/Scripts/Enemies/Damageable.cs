using System;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    [SerializeField]
    private OnDamageTaken _onDamageTaken = new OnDamageTaken();
    public OnDamageTaken OnDamageTaken { get { return _onDamageTaken; } }

    private LogManager _logger;

    public void Start()
    {
        _logger = LogManager.Instance;

        OnDamageTaken.AddListener((damageEvent) =>
        {
            _logger.Trace(damageEvent.Damage + " damage taken by " + gameObject.name);
        });
    }

    public void TakeDamage(int damage)
    {
        OnDamageTaken.Invoke(new DamageEvent(damage));
    }
}

#region Events

[Serializable]
public class OnDamageTaken : UnityEvent<DamageEvent> { }
public class DamageEvent
{
    public int Damage { get; }

    public DamageEvent(int damage)
    {
        Damage = damage;
    }
}
#endregion