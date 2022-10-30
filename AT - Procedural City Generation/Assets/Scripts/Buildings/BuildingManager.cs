using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [Header("Building Prefabs")]
    [SerializeField] GameObject park;
    [SerializeField] List<GameObject> _residentialSmall = new List<GameObject>();
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

    public GameObject GetBuildingBlock(BuildingsData.BuildingLevel level)
    {
        return _residentialSmall[(int)level];
    }

    public GameObject GetParkTile ()
    {
        return park;
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
                    FindNeighbours(x, y, newLargePlot);
                    _totalBuildingPlots += newLargePlot.SetPlotsToComplete();
                }
            }
        }

        SortLargePlotsByDistance();
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

        for (int i = (_largePlots.Count - 1); totalIndustrialPlots < _totalBuildingPlots * _industrialZoneRate; --i)
        {
            _largePlots[i].GetComponent<LargePlot>().ApplyPlotType(BuildingsData.PlotType.INDUSTRIAL);
            totalIndustrialPlots += _largePlots[i].GetComponent<LargePlot>().GetLandSize();
        }

        for (int i = 0; totalCommercialPlots < _totalBuildingPlots * _commercialZoneRate; ++i)
        {
            _largePlots[i].GetComponent<LargePlot>().ApplyPlotType(BuildingsData.PlotType.COMMERCIAL);
            totalCommercialPlots += _largePlots[i].GetComponent<LargePlot>().GetLandSize();
        }

        foreach (GameObject _largePlot in _largePlots)
        {            
            LargePlot plot = _largePlot.GetComponent<LargePlot>();
            if (plot.IsUndefinedPlotType())
            {
                switch (counter % 2)
                {
                    case 0:
                        plot.ApplyPlotType(BuildingsData.PlotType.RESIDENTIAL);
                        break;
                    case 1:
                        if (numberOfParks > 0)
                        {
                            plot.ApplyPlotType(BuildingsData.PlotType.PARK);
                            --numberOfParks;
                        }
                        else
                        {
                            plot.ApplyPlotType(BuildingsData.PlotType.RESIDENTIAL);
                        }
                        break;
                    default:
                        break;
                }
            }
            ++counter;
        }
    }
}
