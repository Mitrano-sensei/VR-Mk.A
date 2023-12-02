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

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        TurnOn();
    }

    public void TurnOn()
    {
        Renderer.material = _onMaterial;
    }

    public void TurnOff()
    {
        Renderer.material = _offMaterial;
    }
}
