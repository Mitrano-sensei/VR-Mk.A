using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class HealthBarUnit : MonoBehaviour
{
    private Renderer _renderer;

    [SerializeField] private Material _onMaterial;
    [SerializeField] private Material _offMaterial;

    public Renderer Renderer { get => _renderer; }

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        TurnOn();
    }

    /**
     * Turn on the unit.
     */
    public void TurnOn()
    {
        Renderer.material = _onMaterial;
    }

    /**
     * Turn off the unit.
     */
    public void TurnOff()
    {
        Renderer.material = _offMaterial;
    }
}
