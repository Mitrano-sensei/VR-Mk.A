using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

public class MechRocketLauncher : MonoBehaviour
{
    [Header("Rocket launcher params")]
    [SerializeField] private GameObject _rocketPrefab;
    [SerializeField] private Transform _rocketSpawnPoint;
    [SerializeField] private bool _isAlly = true;

    [Header("Rocket Stats")]
    [SerializeField] private float _rocketSpeed = 10f;
    [SerializeField] private float _rocketLifeTime = 5f;
    [Tooltip("Damage dealt by the rocket to enemies, note that it does not apply to player mech")]
    [SerializeField] private int _rocketDamage = 10;
    [SerializeField] private float _rocketExplosionRadius = 5f;
    [SerializeField] private float _rocketExplosionForce = 10f;

    [Header("Events")]
    [Tooltip("Event called when the rocket is fired")]
    [SerializeField] private UnityEvent _onFire = new UnityEvent();
    [Tooltip("Event called when the rocket explodes")]
    [SerializeField] private UnityEvent _onExplode = new UnityEvent();

    private LogManager _logger;

    private void Start()
    {
        
        _logger = LogManager.Instance;
        _onFire.AddListener(() =>  _logger.Trace("Firing rocket") );
    }

    public void Fire()
    {
        _onFire?.Invoke();

        var rocket = Instantiate(_rocketPrefab, _rocketSpawnPoint.position, _rocketSpawnPoint.rotation);
        var rocketScript = rocket.GetComponent<RocketScript>();
        rocketScript.Speed = _rocketSpeed;
        rocketScript.LifeTime = _rocketLifeTime;
        rocketScript.Damage = _rocketDamage;
        rocketScript.ExplosionRadius = _rocketExplosionRadius;
        rocketScript.ExplosionForce= _rocketExplosionForce;
        rocketScript.OnExplode = _onExplode;
        rocketScript.IsAlly = _isAlly;
        rocketScript.FiredBy = gameObject;
    }   

}
