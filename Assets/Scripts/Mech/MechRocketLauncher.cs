using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechRocketLauncher : MonoBehaviour
{
    [SerializeField] private GameObject _rocketPrefab;
    [SerializeField] private Transform _rocketSpawnPoint;
    [SerializeField] private float _rocketSpeed = 10f;
    [SerializeField] private float _rocketLifeTime = 5f;
    [SerializeField] private float _rocketDamage = 10f;
    [SerializeField] private float _rocketExplosionRadius = 5f;
    [SerializeField] private float _rocketExplosionForce = 10f;

    private LogManager _logger;

    private void Start()
    {
        _logger = LogManager.Instance;
    }

    public void Fire()
    {
        _logger.Trace("Firing rocket");

        var rocket = Instantiate(_rocketPrefab, _rocketSpawnPoint.position, _rocketSpawnPoint.rotation);
        var rocketScript = rocket.GetComponent<RocketScript>();
        rocketScript.SetSpeed(_rocketSpeed);
        rocketScript.SetLifeTime(_rocketLifeTime);
        rocketScript.SetDamage(_rocketDamage);
        rocketScript.SetExplosionRadius(_rocketExplosionRadius);
        rocketScript.SetExplosionForce(_rocketExplosionForce);
    }

}
