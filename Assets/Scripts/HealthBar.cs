using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private GameManager _gm;
    private HealthBarUnit[] _units;

    [SerializeField] private HealthBarUnit _unit_prefab;
    [SerializeField] private float _span = 2f; // C'est dur les noms
    

    void Start()
    {
        _gm = GameManager.Instance;
        _gm.OnHealthChangeDone.AddListener(PaintHealthBarUnitsListener);

        _units = new HealthBarUnit[_gm.MaxHealth];
        float interval = _span / _units.Length;
        for (int i=0; i < _units.Length; i++)
        {
            _units[i] = Instantiate(
                _unit_prefab,
                transform.position - new Vector3(-2 * interval * i + _span - interval, 0, 0),
                transform.rotation,
                transform);
        }
        PaintHealthBarUnits();
    }
    
    /**
     * Turn On every unit that should be on (i.e. every unit that is less than or equal to the current health) and turns other off.
     */
    void PaintHealthBarUnits()
    {
        for (int i=0; i < _units.Length; i++)
        {
            if (i+1 <= _gm.CurrentHealth)
            {
                _units[i].TurnOn();
            }
            else
            {
                _units[i].TurnOff();
            }
        }
    }

    void PaintHealthBarUnitsListener(OnHealthChangeDoneEvent onHealthChangeDoneEvent) => PaintHealthBarUnits();
}
