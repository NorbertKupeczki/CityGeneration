using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFC_Tile : MonoBehaviour
{
    [Header("Basics")]
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
    private RoadNetworkManager roadManager;

    private void Awake()
    {
        roadManager = FindObjectOfType<RoadNetworkManager>();
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

    public void Collapse(List<int> invalidTiles)
    {
        foreach (int item in invalidTiles)
        {
            validTiles.Remove(item);
        }

        entrophy = validTiles.Count;
    }

    public void Propogate()
    {
        PropogateToDirection(BuildingsData.Direction.NORTH);
        PropogateToDirection(BuildingsData.Direction.EAST);
        PropogateToDirection(BuildingsData.Direction.SOUTH);
        PropogateToDirection(BuildingsData.Direction.WEST);
    }

    public void PropogateToDirection(BuildingsData.Direction direction)
    {
        if (HasNeighbour(direction))
        {
            List<int> validToPropogate = new List<int>() { };

            foreach (int tile in validTiles)
            {
                List<int> validTileIDs = roadManager.GetValidTilesByID(tile, direction);
                for (int i = 0; i < validTileIDs.Count; ++i)
                {
                    if (!validToPropogate.Contains(validTileIDs[i]))
                    {
                        validToPropogate.Add(validTileIDs[i]);
                    }
                }
            }

            List<int> invalidToPropogate = new List<int>() { };
            invalidToPropogate.AddRange(roadManager.GetInvalidTiles(validToPropogate));

            if (invalidToPropogate.Count > 0)
            {
                switch (direction)
                {
                    case BuildingsData.Direction.NORTH:
                        GetNorthNeighbour().GetComponent<WFC_Tile>().Collapse(invalidToPropogate);
                        roadManager.ShortlistTile(GetNorthNeighbour());
                        break;
                    case BuildingsData.Direction.WEST:
                        GetWestNeighbour().GetComponent<WFC_Tile>().Collapse(invalidToPropogate);
                        roadManager.ShortlistTile(GetWestNeighbour());
                        break;
                    case BuildingsData.Direction.SOUTH:
                        GetSouthNeighbour().GetComponent<WFC_Tile>().Collapse(invalidToPropogate);
                        roadManager.ShortlistTile(GetSouthNeighbour());
                        break;
                    case BuildingsData.Direction.EAST:
                        GetEastNeighbour().GetComponent<WFC_Tile>().Collapse(invalidToPropogate);
                        roadManager.ShortlistTile(GetEastNeighbour());
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public bool HasNeighbour(BuildingsData.Direction direction)
    {
        switch (direction)
        {
            case BuildingsData.Direction.NORTH:
                return GetNorthNeighbour() != null;
            case BuildingsData.Direction.WEST:
                return GetWestNeighbour() != null;
            case BuildingsData.Direction.SOUTH:
                return GetSouthNeighbour() != null;
            case BuildingsData.Direction.EAST:
                return GetEastNeighbour() != null;
            default:
                return false;
        }
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
