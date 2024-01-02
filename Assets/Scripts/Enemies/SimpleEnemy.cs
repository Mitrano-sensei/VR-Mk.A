using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(EnemyHealth), typeof(FacePlayer), typeof(MechRocketLauncher))]
public class SimpleEnemy : MonoBehaviour
{
    private MechRocketLauncher _rocketLauncher;

    public void Start()
    {
        _rocketLauncher = GetComponent<MechRocketLauncher>();
    }

    public void Update()
    {
        
    }

    public void DebugFire()
    {
        _rocketLauncher.Fire();
    }

    public enum EnemyState
    {
        Idle,
        Charging,
        Attacking,
        Dead // TODO : Remove this ? 
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