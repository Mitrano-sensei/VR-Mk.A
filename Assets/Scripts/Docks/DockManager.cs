using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class DockManager : Singleton<DockManager>
{
    [Header("Structure of Mainboard")]
    [SerializeField] private int _nbDockX = 15;
    [SerializeField] private int _nbDockY = 6;

    [Description("Position of dock (0,0) in the scene. \nPlease note that X, Y and Z should be respectivly set to right, up and to the player")]
    [SerializeField] private Transform _docksOrigin;

    [Header("Docks")]
    [SerializeField] private Docker _dockPrefab;
    [SerializeField] private Transform _dockParent;
    [SerializeField] private List<Vector2> firstActiveDocksIndex;

    private List<List<Docker>> _docks;

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
        var left = new Vector3(_docksOrigin.position.x, 0, 0);
        var up = new Vector3(0, _docksOrigin.position.y, 0);
        var toPlayer = new Vector3(0, 0, _docksOrigin.position.z);

        _docks = new List<List<Docker>>();

        for (int i = 0; i < _nbDockX; i++)
        {
            _docks.Add(new List<Docker>());
            for (int j = 0; j < _nbDockY; j++)
            {
                var dock = Instantiate(_dockPrefab, _docksOrigin.position + i * left + j * up, Quaternion.identity, _dockParent != null ? _dockParent : transform);
                dock.X = i;
                dock.Y = j;
                dock.IsActive = firstActiveDocksIndex.Contains(new Vector2(i, j));
                _docks[i].Add(dock);
            }
        }
    }

    public bool IsDockable(Vector2 position)
    {
        if (position.x < 0 || position.x >= _nbDockX) return false;
        if (position.y < 0 || position.y >= _nbDockY) return false;

        return _docks[(int)position.x][(int)position.y].IsActive;
    }

    public bool IsDockable(Vector2 position, List<Vector2> constraints)
    {
        if (!IsDockable(position)) return false;

        foreach(var constraint in constraints)
        {
            if (!IsDockable(position + constraint)) return false;
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
                if (IsDockable(new Vector2(docker.X + i, docker.Y + j)))
                {
                    // TODO : Verifier que ce truc là fonctionne
                    // FIXME : Ce truc là va probablement poser probleme
                    // TODO : Enlever ça dès qu'on sait que ça marche pas
                    yield return _docks[docker.X + i][docker.Y + j];
                }
            }
        }
    }
    public bool IsBuyable(Docker dock)
    {
        return !dock.IsActive && GetNeighbours(dock).ToList().FindAll(d => d.IsActive).Count > 0;
    }

    private List<Docker> GetBuyableDocks()
    {
        return _docks.SelectMany(i => i).ToList().FindAll(dock => IsBuyable(dock));
    }

    

}
