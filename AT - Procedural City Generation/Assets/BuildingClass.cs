using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingClass : MonoBehaviour
{
    [Header("Base information")]
    [SerializeField] string _name;
    [SerializeField] int id;

    [Header("Valid Neighbours")]
    [SerializeField] List<int> north = new List<int> { };
    [SerializeField] List<int> west = new List<int> { };
    [SerializeField] List<int> south = new List<int> { };
    [SerializeField] List<int> east = new List<int> { };
    [SerializeField] List<int> above = new List<int> { };
    [SerializeField] List<int> below = new List<int> { };

    private Renderer renderer;
    private int _level;
    
    private void Awake()
    {
        renderer = GetComponentInChildren<Renderer>();
        _level = Mathf.RoundToInt(transform.position.y * 4);
    }

    public void SetColour(Color colour)
    {
        if(renderer != null)
        {
            renderer.material.color = colour;
        }
    }

    public GameObject CreateBuildingBlock(BuildingsData.PlotType type, BuildingsPrefabManager bpm, int idOfBlockBelow, bool LastLevel)
    {
        if (renderer != null)
        {
            Destroy(renderer.gameObject);
        }
        
        if ((LastLevel  && idOfBlockBelow != 26) || (id == 26 && idOfBlockBelow != 26))
        {
            GameObject block = Instantiate(bpm.GetBuilding(type, idOfBlockBelow), transform.position, Quaternion.identity);
            block.GetComponent<BuildingLevelSelector>().SelectLevel(BuildingsData.BuildingLevel.TOP);
            return block;
        }
        else if (id < 26)
        {
            BuildingsData.BuildingLevel bLevel = _level == 0 ? BuildingsData.BuildingLevel.BASE : BuildingsData.BuildingLevel.MID;

            GameObject block = Instantiate(bpm.GetBuilding(type, id), transform.position, Quaternion.identity);
            block.GetComponent<BuildingLevelSelector>().SelectLevel(bLevel);
            return block;
        }
        return null;
    }

    #region ">> Valid tile accessor functions"
    public List<int> GetNorth()
    {
        return north;
    }
    public List<int> GetWest()
    {
        return west;
    }
    public List<int> GetSouth()
    {
        return south;
    }
    public List<int> GetEast()
    {
        return east;
    }
    public List<int> GetAbove()
    {
        return above;
    }
    public List<int> GetBelow()
    {
        return below;
    }
    #endregion
}
