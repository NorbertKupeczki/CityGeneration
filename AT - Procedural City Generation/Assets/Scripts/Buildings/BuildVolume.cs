using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class BuildVolume : MonoBehaviour
{
    [Header("Basics")]
    [SerializeField] int _id;
    [SerializeField] List<int> _validBlocks = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26 };
    [SerializeField] int _entrophy;
    [SerializeField] bool _solved = false;
    [SerializeField] int _blockMinHeight;

    [Header("Neighbours")]
    [SerializeField] GameObject _north;
    [SerializeField] GameObject _west;
    [SerializeField] GameObject _south;
    [SerializeField] GameObject _east;
    [SerializeField] GameObject _up;
    [SerializeField] GameObject _down;

    [Header("Misc.")]
    [SerializeField] BuildingManager _buildingManager;
    [SerializeField] int _level = 0;

    private void Awake()
    {
        _buildingManager = FindObjectOfType<BuildingManager>();
        _entrophy = _validBlocks.Count;
        _level = Mathf.RoundToInt(transform.position.y * 4);
    }
    public int GetEntrophy()
    {
        return _entrophy;
    }

    public bool IsSolved()
    {
        return _solved;
    }

    public int GetLevel()
    {
        return _level;
    }

    public void SetSolved()
    {
        _solved = true;
        if (GetNorth())
        {
            GetNorth().GetComponent<BuildVolume>().RemoveSouth();
        }
        if (GetSouth())
        {
            GetSouth().GetComponent<BuildVolume>().RemoveNorth();
        }
        if (GetWest())
        {
            GetWest().GetComponent<BuildVolume>().RemoveEast();
        }
        if (GetEast())
        {
            GetEast().GetComponent<BuildVolume>().RemoveWest();
        }
        if (GetUp())
        {
            GetUp().GetComponent<BuildVolume>().RemoveUp();
        }
        if (GetDown())
        {
            GetDown().GetComponent<BuildVolume>().RemoveDown();
        }

        Destroy(gameObject);
    }

    public void SetID(int id)
    {
        _id = id;
        gameObject.name = "BuildVolume_" + _id.ToString();
    }

    public int GetID()
    {
        return _id;
    }

    public Vector3 GetPostition()
    {
        return gameObject.transform.position;
    }

    public void SetMinHeight(int value)
    {
        _blockMinHeight = value;
    }

    public bool HasNeighbour(BuildingsData.Direction3D direction)
    {
        switch (direction)
        {
            case BuildingsData.Direction3D.NORTH:
                return GetNorth() != null;
            case BuildingsData.Direction3D.WEST:
                return GetWest() != null;
            case BuildingsData.Direction3D.SOUTH:
                return GetSouth() != null;
            case BuildingsData.Direction3D.EAST:
                return GetEast() != null;
            case BuildingsData.Direction3D.UP:
                return GetUp() != null;
            case BuildingsData.Direction3D.DOWN:
                return GetDown() != null;
            default:
                return false;
        }
    }

    public int SelectRandomBlock()
    {
        if (_entrophy == 1)
        {
            return _validBlocks[0];
        }
        else
        {
            // TODO: May need to check for block 26 and don't allow that under special circumstances
            int rnd = Random.Range(0, _validBlocks.Count - 1);
            return _validBlocks[rnd];
            //return BuildingsData.GetWeightedBlockIndex(_validBlocks);
        }
    }
    
    public void Collapse(List<int> invalidBlocks)
    {
        foreach (int item in invalidBlocks)
        {
            _validBlocks.Remove(item);
        }

        _entrophy = _validBlocks.Count;
    }
    
    public void Propogate()
    {
        PropogateToDirection(BuildingsData.Direction3D.NORTH);
        PropogateToDirection(BuildingsData.Direction3D.EAST);
        PropogateToDirection(BuildingsData.Direction3D.SOUTH);
        PropogateToDirection(BuildingsData.Direction3D.WEST);
        PropogateToDirection(BuildingsData.Direction3D.UP);
        PropogateToDirection(BuildingsData.Direction3D.DOWN);
    }

    public void PropogateToDirection(BuildingsData.Direction3D direction)
    {
        if (HasNeighbour(direction))
        {
            List<int> validToPropogate = new List<int>() { };

            foreach (int tile in _validBlocks)
            {
                List<int> validBlockIDs = _buildingManager.GetValidBlocksByID(tile, direction);
                for (int i = 0; i < validBlockIDs.Count; ++i)
                {
                    if (!validToPropogate.Contains(validBlockIDs[i]))
                    {
                        validToPropogate.Add(validBlockIDs[i]);
                    }
                }
            }

            List<int> invalidToPropogate = new List<int>() { };
            invalidToPropogate.AddRange(_buildingManager.GetInvalidBlocks(validToPropogate));

            if (invalidToPropogate.Count > 0)
            {
                switch (direction)
                {
                    case BuildingsData.Direction3D.NORTH:
                        GetNorth().GetComponent<BuildVolume>().Collapse(invalidToPropogate);
                        //roadManager.ShortlistTile(GetNorthNeighbour());
                        break;
                    case BuildingsData.Direction3D.WEST:
                        GetWest().GetComponent<BuildVolume>().Collapse(invalidToPropogate);
                        //roadManager.ShortlistTile(GetWestNeighbour());
                        break;
                    case BuildingsData.Direction3D.SOUTH:
                        GetSouth().GetComponent<BuildVolume>().Collapse(invalidToPropogate);
                        //roadManager.ShortlistTile(GetSouthNeighbour());
                        break;
                    case BuildingsData.Direction3D.EAST:
                        GetEast().GetComponent<BuildVolume>().Collapse(invalidToPropogate);
                        //roadManager.ShortlistTile(GetEastNeighbour());
                        break;
                    case BuildingsData.Direction3D.UP:
                        GetUp().GetComponent<BuildVolume>().Collapse(invalidToPropogate);
                        //roadManager.ShortlistTile(GetEastNeighbour());
                        break;
                    case BuildingsData.Direction3D.DOWN:
                        GetDown().GetComponent<BuildVolume>().Collapse(invalidToPropogate);
                        //roadManager.ShortlistTile(GetEastNeighbour());
                        break;
                    default:
                        break;
                }
            }
        }
    }

    #region ">> Neighbour Setters"
    public void SetNorth(GameObject _object)
    {
        _north = _object;
    }
    public void SetWest(GameObject _object)
    {
        _west = _object;
    }
    public void SetSouth(GameObject _object)
    {
        _south = _object;
    }
    public void SetEast(GameObject _object)
    {
        _east = _object;
    }
    public void SetUp(GameObject _object)
    {
        _up = _object;
    }
    public void SetDown(GameObject _object)
    {
        _down = _object;
    }
    #endregion

    #region ">> Neighbour Removers"
    public void RemoveNorth()
    {
        _north = null;
    }
    public void RemoveWest()
    {
        _west = null;
    }
    public void RemoveSouth()
    {
        _south = null;
    }
    public void RemoveEast()
    {
        _east = null;
    }
    public void RemoveUp()
    {
        _up = null;
    }
    public void RemoveDown()
    {
        _down = null;
    }
    #endregion

    #region ">> Neighbour Getters"
    public GameObject GetNorth()
    {
        return _north;
    }
    public GameObject GetWest()
    {
        return _west;
    }
    public GameObject GetSouth()
    {
        return _south;
    }
    public GameObject GetEast()
    {
        return _east;
    }
    public GameObject GetUp()
    {
        return _up;
    }
    public GameObject GetDown()
    {
        return _down;
    }
    #endregion

    public bool RemoveInvalidBlocks()
    {
        int startingEntrophy = _entrophy;

        if (_north == null)
        {
            List<int> toRemove = new List<int> { 0, 4, 5, 6, 7, 8, 11, 12, 14, 17, 18, 19, 20, 23, 24 };
            foreach (int id in toRemove)
            {
                _validBlocks.Remove(id);
            }
        }
        if (_west == null)
        {
            List<int> toRemove = new List<int> { 0, 2, 3, 5, 7, 8, 10, 12, 16, 18, 20, 21, 22, 23, 24 };
            foreach (int id in toRemove)
            {
                _validBlocks.Remove(id);
            }
        }
        if (_south == null)
        {
            List<int> toRemove = new List<int> { 0, 1, 2, 3, 4, 5, 9, 10, 13, 17, 18, 19, 20, 21, 22 };
            foreach (int id in toRemove)
            {
                _validBlocks.Remove(id);
            }
        }
        if (_east == null)
        {
            List<int> toRemove = new List<int> { 0, 1, 2, 4, 6, 7, 9, 11, 15, 17, 19, 21, 22, 23, 24 };
            foreach (int id in toRemove)
            {
                _validBlocks.Remove(id);
            }
        }
        if (_level < _blockMinHeight)
        {
            _validBlocks.Remove(26);
        }

        _entrophy = _validBlocks.Count;
        return startingEntrophy == _entrophy;
    }
}
