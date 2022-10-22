using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFC_Tile : MonoBehaviour
{
    [SerializeField] int id;
    [SerializeField] List<int> validTiles = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
    [SerializeField] int entrophy;
    [SerializeField] bool solved = false;

    [Header("Neighbours")]
    [SerializeField] GameObject northNeighbour;
    [SerializeField] GameObject westNeighbour;
    [SerializeField] GameObject southNeighbour;
    [SerializeField] GameObject eastNeighbour;

    private Vector2 mapSize;

    private void Awake()
    {
        entrophy = validTiles.Count;
    }

    public int GetEntrophy()
    {
        return entrophy;
    }

    public bool IsSolved()
    {
        return solved;
    }

    public void SetSolved()
    {
        solved = true;
        if (GetNorthNeighbour())
        {
            GetNorthNeighbour().GetComponent<WFC_Tile>().RemoveSouthNeighbour();
        }
        if (GetSouthNeighbour())
        {
            GetSouthNeighbour().GetComponent<WFC_Tile>().RemoveNorthNeighbour();
        }
        if (GetWestNeighbour())
        {
            GetWestNeighbour().GetComponent<WFC_Tile>().RemoveEastNeighbour();
        }
        if (GetEastNeighbour())
        {
            GetEastNeighbour().GetComponent<WFC_Tile>().RemoveWestNeighbour();
        }

        Destroy(gameObject);
    }

    public void SetID(int _id)
    {
        id = _id;
        gameObject.name = "WFC_" + id.ToString();
    }

    public int GetID()
    {
        return id;
    }

    public Vector3 GetPostition()
    {
        return gameObject.transform.position;
    }

    public void SetSize(Vector2 size)
    {
        mapSize = size;
    }

    public int SelectRandomTile()
    {
        if (entrophy == 1)
        {
            return validTiles[0];
        }
        else
        {
            //int rnd = Random.Range(0, validTiles.Count);
            //Debug.Log("Tile ID: " + validTiles[rnd].ToString());
            //return validTiles[rnd];

            return RoadData.GetWeightedRoadIndex(validTiles);
        }
    }

    public void Collapse(List<int> _list)
    {
        foreach (int item in _list)
        {
            validTiles.Remove(item);
        }

        entrophy = validTiles.Count;
    }

    #region ">> Neighbour Setters"
    public void SetNorthNeighbour(GameObject _object)
    {
        northNeighbour = _object;
    }
    public void SetWestNeighbour(GameObject _object)
    {
        westNeighbour = _object;
    }
    public void SetSouthNeighbour(GameObject _object)
    {
        southNeighbour = _object;
    }
    public void SetEastNeighbour(GameObject _object)
    {
        eastNeighbour = _object;
    }
    #endregion

    #region ">> Neighbour Removers"
    public void RemoveNorthNeighbour()
    {
        northNeighbour = null;
    }
    public void RemoveWestNeighbour()
    {
        westNeighbour = null;
    }
    public void RemoveSouthNeighbour()
    {
        southNeighbour = null;
    }
    public void RemoveEastNeighbour()
    {
        eastNeighbour = null;
    }
    #endregion

    #region ">> Neighbour Getters"
    public GameObject GetNorthNeighbour() 
    {
        return northNeighbour;
    }
    public GameObject GetWestNeighbour()
    {
        return westNeighbour;
    }
    public GameObject GetSouthNeighbour()
    {
        return southNeighbour;
    }
    public GameObject GetEastNeighbour()
    {
        return eastNeighbour;
    }
    #endregion
}
