using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RocketScript : MonoBehaviour
{
    private float _speed = 10f;
    private float _lifeTime = 5f;
    private float _damage = 10f;
    private float _explosionRadius = 5f;
    private float _explosionForce = 10f;

    private LogManager _logger;

    private void Start()
    {
        _logger = LogManager.Instance;
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var collide = collision.collider;
        if (collide.GetComponent<MechMovements>() != null || collide.GetComponent<RocketScript>() != null ) { return; } // Can't collide with the mech that fired it, TODO : Add a tag to the mech and check for that instead ?


        _logger.Trace("Rocket collided with " + collision.gameObject.name);

        var colliders = Physics.OverlapSphere(transform.position, _explosionRadius);
        foreach (var collider in colliders)
        {
            if (collider.GetComponent<MechMovements>()) { continue; } // Can't collide with the mech that fired it, TODO : Add a tag to the mech and check for that instead ?

            Debug.Log("Exploding on " + collider.gameObject.name);
            var rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(_explosionForce, transform.position, _explosionRadius);
            }

            /*
            // TODO : Later
            var health = collider.GetComponent<HealthScript>();
            if (health != null)
            {
                health.TakeDamage(_damage);
            }
            */
        }

        Destroy(gameObject);
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public void SetLifeTime(float lifeTime)
    {
        _lifeTime = lifeTime;
    }

    public void SetDamage(float damage)
    {
        _damage = damage;
    }

    public void SetExplosionRadius(float explosionRadius)
    {
        _explosionRadius = explosionRadius;
    }

    public void SetExplosionForce(float explosionForce)
    {
        _explosionForce = explosionForce;
    }
}
