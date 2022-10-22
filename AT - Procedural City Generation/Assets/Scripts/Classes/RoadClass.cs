using Palmmedia.ReportGenerator.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadClass : MonoBehaviour
{
    [Header("Base information")]
    [SerializeField] string _name;
    [SerializeField] int id;
    [SerializeField] int rotation;
    [SerializeField] Vector2 mapSize;

    [Header("Components")]
    [SerializeField] GameObject meshComponent;

    [Header("Valid Neighbours")]
    [SerializeField] List<int> north = new List<int> { };
    [SerializeField] List<int> west = new List<int> { };
    [SerializeField] List<int> south = new List<int> { };
    [SerializeField] List<int> east = new List<int> { };

    [Header("Procedural components")]
    [SerializeField] private PerlinGenerator perlinGenerator;
    [SerializeField] uint maxBuildingHeight = 5;

    private void Awake()
    {
        gameObject.name = _name + " (" + gameObject.transform.position.x + ":" + gameObject.transform.position.z + ")";
        perlinGenerator = FindObjectOfType<PerlinGenerator>();
        meshComponent.transform.rotation = Quaternion.Euler( 0, 90 * rotation, 0);
        GenerateBuildings();
    }

    private void GenerateBuildings()
    {
        BuildingPlot[] buidingPlots = gameObject.GetComponentsInChildren<BuildingPlot>();
        foreach(BuildingPlot buidingPlot in buidingPlots)
        {
            int x = Mathf.RoundToInt(GetPlotCoordinate(buidingPlot).x);
            int y = Mathf.RoundToInt(GetPlotCoordinate(buidingPlot).y);
            buidingPlot.Build(ConvertToLevels(perlinGenerator.GetNoiseValue(x, y)));
        }
    }

    private int ConvertToLevels(float noiseValue)
    {
        return Mathf.FloorToInt(noiseValue * maxBuildingHeight) + 1;
    }

    private Vector2 GetPlotCoordinate(BuildingPlot plot)
    {
        int x = Mathf.FloorToInt(gameObject.transform.position.z);
        int y = Mathf.FloorToInt(gameObject.transform.position.x);
        int width = (int)mapSize.x;

        int centre = (y * width * 9) + width * 3 + x * 3 + 1;

        return new Vector2(centre + plot.gameObject.transform.position.z * 3,
                           centre + plot.gameObject.transform.position.x * 3);
    }

    public void SetMapSize (Vector2 _mapSize)
    {
        mapSize = _mapSize;
    }

    void Start()
    {
        
        
    }
    
    void Update()
    {
        
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
