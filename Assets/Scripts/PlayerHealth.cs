using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private OnHealthLossEvent _onHealthLoss = new OnHealthLossEvent();
    [SerializeField] private OnHealthGainEvent _onHealthGain = new OnHealthGainEvent();

    public OnHealthLossEvent OnHealthLoss { get => _onHealthLoss; }
    public OnHealthGainEvent OnHealthGain { get => _onHealthGain; }
}

#region Events

[SerializeField] public class OnHealthLossEvent : UnityEvent<int> { }
[SerializeField] public class OnHealthGainEvent : UnityEvent<int> { }
#endregion
