using System.Collections.Generic;
using UnityEngine;

public class RoadClass : MonoBehaviour
{
    [Header("Base information")]
    [SerializeField] string _name;
    [SerializeField] int id;
    [SerializeField] Vector2 mapSize;

    [Header("Valid Neighbours")]
    [SerializeField] List<int> north = new List<int> { };
    [SerializeField] List<int> west = new List<int> { };
    [SerializeField] List<int> south = new List<int> { };
    [SerializeField] List<int> east = new List<int> { };

    [Header("Procedural components")]
    [SerializeField] private PerlinGenerator perlinGenerator;
    [SerializeField] uint maxBuildingHeight = 5;

    private BuildingManager buildingManager;

    void Awake()
    {
        buildingManager = FindObjectOfType<BuildingManager>();

        BuildingPlot[] buildingPlots = gameObject.GetComponentsInChildren<BuildingPlot>();
        foreach (BuildingPlot _buildingPlot in buildingPlots)
        {
            int x = Mathf.RoundToInt(GetPlotCoordinate(_buildingPlot).x);
            int y = Mathf.RoundToInt(GetPlotCoordinate(_buildingPlot).y);
            buildingManager.AddPlot(x, y, _buildingPlot.gameObject);
        }
    }

    void Start()
    {
        gameObject.name = _name + " (" + gameObject.transform.position.x + ":" + gameObject.transform.position.z + ")";
        perlinGenerator = FindObjectOfType<PerlinGenerator>();
    }

    private Vector2 GetPlotCoordinate(BuildingPlot plot)
    {
        float x = gameObject.transform.position.z * 2 + 0.5f;
        float y = gameObject.transform.position.x * 2 + 0.5f;

        return new Vector2(x + plot.gameObject.transform.localPosition.z * 2,
                           y + plot.gameObject.transform.localPosition.x * 2);
    }

    public void SetMapSize (Vector2 _mapSize)
    {
        mapSize = _mapSize;
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
    #endregion
}
