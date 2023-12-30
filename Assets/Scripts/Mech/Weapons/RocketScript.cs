using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RocketScript : MonoBehaviour
{
    private float _speed;
    private float _lifeTime;
    private int _damage;
    private float _explosionRadius;
    private float _explosionForce;

    private LogManager _logger;

    public float Speed { get => _speed; set => _speed = value; }
    public float LifeTime { get => _lifeTime; set => _lifeTime = value; }
    public int Damage { get => _damage; set => _damage = value; }
    public float ExplosionRadius { get => _explosionRadius; set => _explosionRadius = value; }
    public float ExplosionForce { get => _explosionForce; set => _explosionForce = value; }

    private void Start()
    {
        _logger = LogManager.Instance;
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var collide = collision.collider;
        if (collide.GetComponent<MechMovements>() != null || collide.GetComponent<RocketScript>() != null ) { return; } // Can't collide with the mech that fired it, TODO : Add a tag to the mech and check for that instead ?

        _logger.Trace("Rocket collided with " + collision.gameObject.name);

        var colliders = Physics.OverlapSphere(transform.position, ExplosionRadius);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.GetComponent<MechMovements>()) { continue; } // Can't collide with the mech that fired it, TODO : Add a tag to the mech and check for that instead ?

            Debug.Log("Exploding on " + collider.gameObject.name);
            var rb = collider.gameObject.GetComponent<Rigidbody>();
            if (rb != null && !rb.isKinematic)
            {
                rb.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius);
            }

            var damageable = collider.GetComponent<Damageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(Damage);
            }
        }

        Destroy(gameObject);
    }
}
