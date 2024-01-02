using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class RocketScript : MonoBehaviour
{
    private float _speed;
    private float _lifeTime;
    private int _damage;
    private float _explosionRadius;
    private float _explosionForce;
    private bool _isAlly = true;

    private GameObject _firedBy;

    private LogManager _logger;

    public float Speed { get => _speed; set => _speed = value; }
    public float LifeTime { get => _lifeTime; set => _lifeTime = value; }
    public int Damage { get => _damage; set => _damage = value; }
    public float ExplosionRadius { get => _explosionRadius; set => _explosionRadius = value; }
    public float ExplosionForce { get => _explosionForce; set => _explosionForce = value; }

    public UnityEvent OnExplode { get; set; }
    public bool IsAlly { get => _isAlly; set => _isAlly = value; }
    public GameObject FiredBy { get => _firedBy; set => _firedBy = value; }

    private void Start()
    {
        _logger = LogManager.Instance;
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
        LifeTime -= Time.deltaTime;
        if (LifeTime <= 0)
        {
            OnExplode?.Invoke();
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        var collide = collision.collider;
        if ((_isAlly && collide.GetComponent<MechMovements>() != null) || collide.GetComponent<RocketScript>() != null ) { return; } // Can't collide with the mech that fired it, TODO : Add a tag to the mech and check for that instead ?
        if (collide.gameObject == FiredBy) { return; } // Can't collide with the mech that fired it, TODO : Add a tag to the mech and check for that instead ?
        Debug.Log("Firedby " + FiredBy + ", Collision " + collide.gameObject);

        OnExplode?.Invoke();
        _logger.Trace("Rocket collided with " + collision.gameObject.name);

        var colliders = Physics.OverlapSphere(transform.position, ExplosionRadius);
        foreach (var collider in colliders)
        {
            if (_isAlly && collider.gameObject.GetComponent<MechMovements>()) { continue; } // Can't collide with the mech that fired it, TODO : Add a tag to the mech and check for that instead ?
            // Note that enemies can still be damaged by other enemies' rockets

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
