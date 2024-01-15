using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RotatingLight : MonoBehaviour
{
    // Danger -> Lights getting crazy when the mech is hit

    private List<Light> _lights;
    [SerializeField] private GameManager _gameManager;

    [SerializeField] float _dangerDuration = 5; // In seconds

    [SerializeField] private Color _normalColor = new Color(200, 150, 50); // Color when everything is fine
    [SerializeField] private Color _dangerColor = new Color(225, 25, 25); // Color when taking hits
    [SerializeField] private float _normalIntensity = 2;
    [SerializeField] private float _dangerIntensity = 4;
    [SerializeField] private float _rotationSpeed = 50;
    [SerializeField] private float _dangerSpeedMultiplier = 1.75f;

    private bool _isDanger = false;
    private float _dangerStartTime;

    // Start is called before the first frame update
    void Start()
    {
        if(_gameManager == null)
        {
            _gameManager = GameManager.Instance;
        }

        _lights = new List<Light>();
        foreach(Transform t in transform)
        {
            Light newLight = t.GetComponent<Light>();
            if (newLight != null)
            {
                _lights.Add(newLight);
                newLight.color = _normalColor;
                newLight.intensity = _normalIntensity;
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        _gameManager.OnHealthChange.AddListener((OnHealthChangeEvent e) =>
        { 
            if (e.Amount < 0)
            {
                _isDanger = true;
                _dangerStartTime = Time.time;
            }
        });
        RotateLights();
        if (_isDanger)
        {
            Danger();
        }
    }

    void RotateLights()
    {
        foreach(Light light in _lights)
        {

            light.transform.Rotate(0, _rotationSpeed * Time.deltaTime * (_isDanger ? _dangerSpeedMultiplier : 1), 0);
        }
    }

    void Danger()
    {
        float t = Time.time - _dangerStartTime;
        foreach(Light light in _lights)
        {
            light.intensity = _dangerIntensity;
            light.color = _dangerColor;
            if (t > _dangerDuration)
            { 
                _isDanger = false;
                light.intensity = _normalIntensity;
                light.color = _normalColor;
            }
        }
    }
}
