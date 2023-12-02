using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    [Header("Player health")]
    [SerializeField] private int _maxHealth = 20;
    private int currentHealth;

    [Header("Events")]
    [SerializeField] private OnHealthChange _onHealthChange = new OnHealthChange();
    private OnHealthChangeDone _onHealthChangeDone = new OnHealthChangeDone();

    protected LogManager _logger;

    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public int MaxHealth { get => _maxHealth; }
    public OnHealthChange OnHealthChange { get => _onHealthChange; set => _onHealthChange = value; }
    public OnHealthChangeDone OnHealthChangeDone { get => _onHealthChangeDone; set => _onHealthChangeDone = value; }

    // Start is called before the first frame update
    void Start()
    {
        _logger = LogManager.Instance;
        CurrentHealth = MaxHealth;

        OnHealthChange.AddListener(OnHealthChangeHandler);
    }

    private void OnHealthChangeHandler(OnHealthChangeEvent onHealthChangeEvent)
    {
        CurrentHealth = Math.Clamp(CurrentHealth + onHealthChangeEvent.Amount, 0, MaxHealth);
        _logger.Trace("Health remaining: " + CurrentHealth);
        if (CurrentHealth <= 0)
        {
            _logger.Trace("You are DED. GAME OVER.");
        }
    }

    public void GainHealth(int amount=1)
    {
        OnHealthChange?.Invoke(new OnHealthChangeEvent(amount));
        OnHealthChangeDone?.Invoke(new OnHealthChangeDoneEvent());
    }
}

#region Events

[Serializable] public class OnHealthChange : UnityEvent<OnHealthChangeEvent> { }

public class OnHealthChangeEvent
{
    private int _amount;

    public OnHealthChangeEvent(int amount)
    {
        Amount = amount;
    }

    public int Amount { get => _amount; set => _amount = value; }
}

[Serializable] public class OnHealthChangeDone : UnityEvent<OnHealthChangeDoneEvent> { }

public class OnHealthChangeDoneEvent
{

}
#endregion

