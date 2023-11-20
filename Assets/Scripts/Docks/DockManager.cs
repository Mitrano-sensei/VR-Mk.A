using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DockManager : Singleton<DockManager>
{
    [Header("Docks")]
    [SerializeField] private Docker _dockPrefab;
    [SerializeField] private List<Vector3> _firstActiveDocksIndex;
    [SerializeField] private List<DockElement> _dockElements;

    private Dictionary<Vector3, Docker> _docks;

    public void Start()
    {
        InitializeMainBoard();   
    }


    /**
     * Intantiate every docks and save them in a Dictionnary with their position as key.
     * Initialize each docks to inactive (except those whose positions are in firstActiveDocksIndex).
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
            dock.IsActive = _firstActiveDocksIndex.Contains(new(dock.X, dock.Y, dock.Z));

            _docks.Add(dockElement.Position, dock);
        }
    }

    /*
     * Returns true if the given position is dockable.
     */
    private bool IsDockable(Vector3 position)
    {
        if (position.x < 0) return false;
        if (!_docks.ContainsKey(position)) return false;

        var dock = _docks[position];
        return dock != null && dock.IsActive;
    }

    /**
     * Returns true if the given position is dockable and if all the constraints are dockable.
     */
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

    /**
     * Returns a list of neighbour dockers.
     */
    public IEnumerable<Docker> GetNeighbours(Docker docker)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
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
    /**
     * Returns true if the given docker is buyable.
     */
    public bool IsBuyable(Docker dock)
    {
        return !dock.IsActive && GetNeighbours(dock).ToList().FindAll(d => d.IsActive).Count > 0;
    }

    /**
     * Returns a list of buyable dockers.
     */
    public List<Docker> GetBuyableDocks()
    {
        return _docks.ToList().FindAll(keyValue => IsBuyable(keyValue.Value)).Select(keyValue => keyValue.Value).ToList();
    }

    /**
     * Returns a list of active dockers.
     */
    private List<Docker> GetActiveDockers(Boolean active = true)
    {
        return _docks.ToList().FindAll(keyValue => keyValue.Value.IsActive == active).Select(keyValue => keyValue.Value).ToList();
    }

    /**
     * Returns a list of available dockers. A docker is available if it is active and not docked.
     */
    public List<Docker> GetAvailableActiveDockers()
    {
        return GetActiveDockers().FindAll(dock => dock.IsAvailable);
    }

    /**
     * Returns the docker at the given position.
     */
    public Docker GetDocker(Vector3 position)
    {
        return _docks.ToList().Find(keyValue => keyValue.Value.X == position.x && keyValue.Value.Y == position.y && keyValue.Value.Z == position.z).Value;
    }

    /**
     * Returns a list of used dockers.
     */
    private List<Docker> GetUsedDockers()
    {
        return _docks.ToList().FindAll(keyValue => keyValue.Value.DockedObject != null).Select(keyValue => keyValue.Value).ToList();
    }

    /**
     * Returns a random used docker.
     */
    public Docker GetRandomUsedDocker()
    {
        var usedDockers = GetUsedDockers();
        if (usedDockers.Count == 0) return null;
        return usedDockers[UnityEngine.Random.Range(0, usedDockers.Count)];
    }

    /**
     * Calls Eject() on a random docker.
     */
    public void EjectRandomDocker()
    {
        GetRandomUsedDocker()?.Eject();
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
