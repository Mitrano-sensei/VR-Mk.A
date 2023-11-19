using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class DockManager : Singleton<DockManager>
{
    [Header("Docks")]
    [SerializeField] private Docker _dockPrefab;
    [SerializeField] private List<Vector3> firstActiveDocksIndex;
    [SerializeField] private List<DockElement> _dockElements;

    private Dictionary<Vector3, Docker> _docks;

    public void Start()
    {
        InitializeMainBoard();   
    }


    /**
     * Intantiate every docks and save them in a 2D array.
     * Initialize each docks to inActive (except the first ones).
     */
    private void InitializeMainBoard()
    {
        _docks = new();

        foreach (var dockElement in _dockElements)
        {
            var dock = Instantiate(_dockPrefab, dockElement.DockOrigin.position, Quaternion.identity, dockElement.DockOrigin);
            dock.X = (int)dockElement.Position.x;
            dock.Y = (int)dockElement.Position.y;
            dock.Z = (int)dockElement.Position.z;
            dock.IsActive = firstActiveDocksIndex.Contains(new(dock.X, dock.Y, dock.Z));

            _docks.Add(dockElement.Position, dock);
        }
    }

    private bool IsDockable(Vector3 position)
    {
        if (position.x < 0) return false;
        if (!_docks.ContainsKey(position)) return false;

        var dock = _docks[position];
        return dock != null && dock.IsActive;
    }

    public bool IsDockable(Vector3 position, List<Vector2> constraints)
    {
        if (!IsDockable(position)) return false;

        foreach(var constraint in constraints)
        {
            var constraint3d = new Vector3(constraint.x, constraint.y, 0);      // Adding the 0 because position already hase a Z constraint.
            if (!IsDockable(position + constraint3d)) return false;
        }

        return true;
    }

    public IEnumerable<Docker> GetNeighbours(Docker docker)
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0) continue;
                var neighbourPosition = new Vector3(docker.X + i, docker.Y + j, docker.Z);

                if (IsDockable(neighbourPosition))
                {
                    yield return _docks[neighbourPosition];
                }
            }
        }
    }
    public bool IsBuyable(Docker dock)
    {
        return !dock.IsActive && GetNeighbours(dock).ToList().FindAll(d => d.IsActive).Count > 0;
    }

    public List<Docker> GetBuyableDocks()
    {
        return _docks.ToList().FindAll(keyValue => IsBuyable(keyValue.Value)).Select(keyValue => keyValue.Value).ToList();
    }

    public List<Docker> GetActiveDocks(Boolean active = true)
    {
        return _docks.ToList().FindAll(keyValue => keyValue.Value.IsActive == active).Select(keyValue => keyValue.Value).ToList();
    }

    public List<Docker> GetAvailableActiveDocks()
    {
        return GetActiveDocks().FindAll(dock => dock.IsAvailable);
    }

    public Docker GetDocker(Vector3 position)
    {
        return _docks.ToList().Find(keyValue => keyValue.Value.X == position.x && keyValue.Value.Y == position.y && keyValue.Value.Z == position.z).Value;
    }

}

[Serializable]
public class DockElement
{
    [Description(   "Position of the dock on the motherboard. \n" +
                    "Two docks are neigbours if their (X,Y) position are close (X = X'+-1 || Y = Y'+-1) AND their Z position are the same (Z == Z'). \n" +
                    "Two neighbour docks should be separated by .15m and should be aligned.")]
    public Vector3 Position;
    [Description(   "Origin of the dock. \n" +
                    "The dock will be instantiated at this position.")]
    public Transform DockOrigin;
}
