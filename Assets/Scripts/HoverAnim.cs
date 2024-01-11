using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Hoverable))]
public class HoverAnim : MonoBehaviour
{
    private Hoverable _hoverable;
    [SerializeField] private ParticleSystem _particlesPrefab;
    private ParticleSystem _particles;

    // Start is called before the first frame update
    void Start()
    {
        _hoverable = GetComponent<Hoverable>();
        _particles = Instantiate(_particlesPrefab, transform.position, Quaternion.identity, transform);
        _particles.Stop();
        _hoverable.OnHoverEnter.AddListener((OnHoverEnterEvent) => _particles.Play());
        _hoverable.OnHoverExit.AddListener((OnHoverExitEvent) => _particles.Stop());
    }
}
