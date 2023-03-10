using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

public class BuildingManager : MonoBehaviour
{
    [Header("Building Prefabs")]
    [SerializeField] List<GameObject> _testBuildings = new List<GameObject>();
    [SerializeField] GameObject _largePlot;

    [Header("Construction Management")]
    [SerializeField] int _plotsWidth;
    [SerializeField] int _plotsHeight;
    [SerializeField] List<GameObject> _allPlots;
    [SerializeField] List<GameObject> _largePlots;
    [SerializeField] int _totalBuildingPlots = 0;

    [Header("Zone Rules")]
    [SerializeField] float _industrialZoneRate = 0.15f;
    [SerializeField] float _commercialZoneRate = 0.33f;
    [SerializeField] int _numberOfParks = 2;

    private int jobsRunning = 1;
    private CityManager _cityManager;

    public Action GenerationComplete;

    private void Awake()
    {
        _cityManager = FindObjectOfType<CityManager>();
    }

    private void Start()
    {
        StartCoroutine(CheckBuildVolumesReady());
    }

    public bool AreJobsRunning()
    {
        return jobsRunning != 0;
    }

    public GameObject GetTestBlock(int id)
    {
        return _testBuildings[id];
    }

    public void SetPlotsSize (int width, int height)
    {
        _plotsWidth = width;
        _plotsHeight = height;

        _allPlots = new List<GameObject>(width * height);
        for (int i = 0; i < _plotsWidth * _plotsHeight; i++)
        {
            _allPlots.Add(null);
        }
    }

    private int CoordsToPlot(int x, int y)
    {
        return y * _plotsWidth + x;
    }

    public void AddPlot(int x, int y, GameObject plot)
    {
        _allPlots[CoordsToPlot(x, y)] = plot;
    }
    
    private void RemovePlot(int ID)
    {
        _allPlots[ID] = null;
    }

    private GameObject NewLargePlot()
    {
        GameObject newLargePlot = Instantiate(_largePlot);
        _largePlots.Add(newLargePlot);

        newLargePlot.GetComponent<LargePlot>().SetCentreOfCity(_plotsWidth, _plotsHeight);

        return newLargePlot;
    }

    public void CreateLargePlots()
    {
        for (int y = 0; y < _plotsHeight; ++y)
        {
            for (int x = 0; x < _plotsWidth; ++x)
            {
                if (_allPlots[CoordsToPlot(x, y)] != null &&_allPlots[CoordsToPlot(x, y)].GetComponent<BuildingPlot>().IsNotLinked())
                {
                    LargePlot newLargePlot = NewLargePlot().GetComponent<LargePlot>();

                    newLargePlot.AddBuildingPlot(_allPlots[CoordsToPlot(x, y)]);
                    newLargePlot.transform.SetParent(gameObject.transform);
                    _cityManager.AddZoneToHierarchy(newLargePlot.gameObject);
                    FindNeighbours(x, y, newLargePlot);
                    _totalBuildingPlots += newLargePlot.SetPlotsToComplete();
                }
            }
        }

        SortLargePlotsByDistance();
        Debug.Log("Zones done");

        // Start the Wave Function Collapse on each zone
    }

    private void FindNeighbours(int _x, int _y, LargePlot largePlot)
    {
        GameObject plot = _allPlots[CoordsToPlot(_x, _y)];
        plot.GetComponent<BuildingPlot>().SetLinkingState(BuildingsData.PlotLinking.IN_PROGRESS);

        //Find north neighbour
        if (_y - 1 >= 0 && _allPlots[CoordsToPlot(_x, _y - 1)] != null)
        {
            LinkPlots(plot, _allPlots[CoordsToPlot(_x, _y - 1)], BuildingsData.Direction.NORTH, largePlot);
            RecursivePropogation(_x, _y - 1, largePlot);
        }
        //Find west neighbour
        if (_x - 1 >= 0 && _allPlots[CoordsToPlot(_x - 1, _y)] != null)
        {
            LinkPlots(plot, _allPlots[CoordsToPlot(_x - 1, _y)], BuildingsData.Direction.WEST, largePlot);
            RecursivePropogation(_x - 1, _y, largePlot);
        }
        //Find south neighbour
        if (_y + 1 < _plotsHeight && _allPlots[CoordsToPlot(_x, _y + 1)] != null)
        {
            LinkPlots(plot, _allPlots[CoordsToPlot(_x, _y + 1)], BuildingsData.Direction.SOUTH, largePlot);
            RecursivePropogation(_x, _y + 1, largePlot);
        }
        //Find east neighbour
        if (_x + 1 < _plotsWidth && _allPlots[CoordsToPlot(_x + 1, _y)] != null)
        {
            LinkPlots(plot, _allPlots[CoordsToPlot(_x + 1, _y)], BuildingsData.Direction.EAST, largePlot);
            RecursivePropogation(_x + 1, _y, largePlot);
        }
    }

    private void LinkPlots (GameObject plotA, GameObject plotB, BuildingsData.Direction direction, LargePlot largePlot)
    {
        switch (direction)
        {
            case BuildingsData.Direction.NORTH:
                if (!plotA.GetComponent<BuildingPlot>().HasNeighbour(BuildingsData.Direction.NORTH))
                {
                    plotA.GetComponent<BuildingPlot>().SetNorth(plotB);
                    plotB.GetComponent<BuildingPlot>().SetSouth(plotA);
                    AddPlotToLargePlot(plotB, largePlot);
                }
                break;
            case BuildingsData.Direction.WEST:
                if (!plotA.GetComponent<BuildingPlot>().HasNeighbour(BuildingsData.Direction.WEST))
                {
                    plotA.GetComponent<BuildingPlot>().SetWest(plotB);
                    plotB.GetComponent<BuildingPlot>().SetEast(plotA);
                    AddPlotToLargePlot(plotB, largePlot);
                }
                break;
            case BuildingsData.Direction.SOUTH:
                if (!plotA.GetComponent<BuildingPlot>().HasNeighbour(BuildingsData.Direction.SOUTH))
                {
                    plotA.GetComponent<BuildingPlot>().SetSouth(plotB);
                    plotB.GetComponent<BuildingPlot>().SetNorth(plotA);
                    AddPlotToLargePlot(plotB, largePlot);
                }
                break;
            case BuildingsData.Direction.EAST:
                if (!plotA.GetComponent<BuildingPlot>().HasNeighbour(BuildingsData.Direction.EAST))
                {
                    plotA.GetComponent<BuildingPlot>().SetEast(plotB);
                    plotB.GetComponent<BuildingPlot>().SetWest(plotA);
                    AddPlotToLargePlot(plotB, largePlot);
                }
                break;
            default:
                break;
        }
    }

    private void RecursivePropogation(int x, int y, LargePlot largePlot)
    {
        if (_allPlots[CoordsToPlot(x, y)].GetComponent<BuildingPlot>().IsNotLinked())
        {
            FindNeighbours(x, y, largePlot);
        }
    }

    private void AddPlotToLargePlot(GameObject plot, LargePlot largePlot)
    {
        if (!largePlot.CheckPlotInList(plot))
        {
            largePlot.AddBuildingPlot(plot);
        }
    }

    private void SortLargePlotsByDistance()
    {
        
        _largePlots.Sort(delegate (GameObject a, GameObject b) {
            return (a.GetComponent<LargePlot>().GetDistanceFromCentre()).CompareTo(b.GetComponent<LargePlot>().GetDistanceFromCentre());
        });
        
    }

    public void CreateZones()
    {
        int numberOfParks = _numberOfParks;
        int totalCommercialPlots = 0;
        int totalIndustrialPlots = 0;
        int counter = 0;
        jobsRunning = _largePlots.Count;

        for (int i = (_largePlots.Count - 1); totalIndustrialPlots < _totalBuildingPlots * _industrialZoneRate; --i)
        {
            LargePlot largePlot = _largePlots[i].GetComponent<LargePlot>();
            largePlot.ApplyPlotType(BuildingsData.PlotType.INDUSTRIAL);
            totalIndustrialPlots += largePlot.GetLandSize();
            // Create build volume
            StartCoroutine(largePlot.CreateBuildVolume(VolumeReady));
        }

        for (int i = 0; totalCommercialPlots < _totalBuildingPlots * _commercialZoneRate; ++i)
        {
            LargePlot largePlot = _largePlots[i].GetComponent<LargePlot>();
            largePlot.ApplyPlotType(BuildingsData.PlotType.COMMERCIAL);
            totalCommercialPlots += largePlot.GetLandSize();
            // Create build volume
            StartCoroutine(largePlot.CreateBuildVolume(VolumeReady));
        }

        foreach (GameObject _largePlot in _largePlots)
        {
            LargePlot largePlot = _largePlot.GetComponent<LargePlot>();
            if (largePlot.IsUndefinedPlotType())
            {
                switch (counter % 2)
                {
                    case 0:
                        largePlot.ApplyPlotType(BuildingsData.PlotType.RESIDENTIAL);
                        break;
                    case 1:
                        if (numberOfParks > 0)
                        {
                            largePlot.ApplyPlotType(BuildingsData.PlotType.PARK);
                            --numberOfParks;
                        }
                        else
                        {
                            largePlot.ApplyPlotType(BuildingsData.PlotType.RESIDENTIAL);
                        }
                        break;
                    default:
                        break;
                }
                // Create build volume
                StartCoroutine(largePlot.CreateBuildVolume(VolumeReady));
            }
            ++counter;
        }
    }

    private void VolumeReady()
    {
        --jobsRunning;
    }

    private void ConstructionReady()
    {
        --jobsRunning;
    }

    public List<int> GetInvalidBlocks(List<int> validBlocks)
    {
        List<int> invalidBlocks = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27 };

        foreach (int block in validBlocks)
        {
            invalidBlocks.Remove(block);
        }

        return invalidBlocks;
    }
    public List<int> GetValidBlocksByID(int id, BuildingsData.Direction3D direction)
    {
        switch (direction)
        {
            case BuildingsData.Direction3D.NORTH:
                return _testBuildings[id].GetComponent<BuildingClass>().GetNorth();
            case BuildingsData.Direction3D.WEST:
                return _testBuildings[id].GetComponent<BuildingClass>().GetWest();
            case BuildingsData.Direction3D.SOUTH:
                return _testBuildings[id].GetComponent<BuildingClass>().GetSouth();
            case BuildingsData.Direction3D.EAST:
                return _testBuildings[id].GetComponent<BuildingClass>().GetEast();
            case BuildingsData.Direction3D.UP:
                return _testBuildings[id].GetComponent<BuildingClass>().GetAbove();
            case BuildingsData.Direction3D.DOWN:
                return _testBuildings[id].GetComponent<BuildingClass>().GetBelow();
            default:
                return new List<int>() { };
        }
    }

    IEnumerator CheckBuildVolumesReady()
    {
        while (AreJobsRunning())
        {
            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("Build Volumes Ready");
        StartGeneratingBuildings();
        yield break;
    }

    IEnumerator CheckConstructionReady()
    {
        while (AreJobsRunning())
        {
            yield return new WaitForSeconds(0.1f);
        }

        GenerationComplete?.Invoke();

        Debug.Log("Building Construction Complete");
        yield break;
    }

    private void StartGeneratingBuildings()
    {
        foreach (GameObject zone in _largePlots)
        {
            zone.GetComponent<LargePlot>().StartBuildingGeneration(ConstructionReady);
            ++jobsRunning;
        }

        StartCoroutine(CheckConstructionReady());
    }
}
