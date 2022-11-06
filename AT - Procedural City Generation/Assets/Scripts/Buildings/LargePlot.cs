using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargePlot : MonoBehaviour
{
    [SerializeField] BuildingsData.PlotType _plotType = BuildingsData.PlotType.UNDEFINED;
    [SerializeField] int _landSize = 0;
    [SerializeField] Vector2 _centreOfLand;
    [SerializeField] List<GameObject> _buildingPlots;
    [SerializeField] int _maxBuildingHeight;
    [SerializeField] List<List<GameObject>> _buildVolumes = new List<List<GameObject>> { };

    [Header("District Position")]
    [SerializeField] private Vector2 _centreOfCity;
    [SerializeField] private float _distanceFromCentre;

    private BuildingManager _buildingManager;
    private BuildingsPrefabManager _bpm;
    private Color _colour;

    private void Awake()
    {
        _buildingManager = FindObjectOfType<BuildingManager>();
        _bpm = FindObjectOfType<BuildingsPrefabManager>();
    }

    public void AddBuildingPlot(GameObject _newPlot)
    {
        _buildingPlots.Add(_newPlot);
        if(_landSize == 0)
        {
            _centreOfLand = new Vector2(_newPlot.transform.position.z, _newPlot.transform.position.x);
        }
        else
        {
            _centreOfLand = RecalculateCentre(_newPlot);
        }
        _landSize += 1;
    }

    public int SetPlotsToComplete()
    {
        foreach (GameObject _plot in _buildingPlots)
        {
            _landSize = _buildingPlots.Count;

            _plot.GetComponent<BuildingPlot>().SetLinkingState(BuildingsData.PlotLinking.COMPLETED);
        }

        gameObject.transform.position = new Vector3(_centreOfLand.y, 0.0f, _centreOfLand.x);
        _distanceFromCentre = GetDistanceFromCentreOfCity();
        _maxBuildingHeight = CalculateMaxBuildingHeight();
        return GetLandSize();
    }

    public bool CheckPlotInList(GameObject plot)
    {
        return _buildingPlots.Contains(plot);
    }

    public void ApplyPlotType(BuildingsData.PlotType plotType)
    {
        _plotType = plotType;

        foreach (GameObject _plot in _buildingPlots)
        {
            _plot.GetComponent<BuildingPlot>().SetPlotType(_plotType);
            //_plot.GetComponent<BuildingPlot>().Build(_maxBuildingHeight);
        }

        switch (plotType)
        {
            case BuildingsData.PlotType.UNDEFINED:
                break;
            case BuildingsData.PlotType.RESIDENTIAL:
                _colour = Color.green ;
                break;
            case BuildingsData.PlotType.COMMERCIAL:
                _colour = Color.blue;
                break;
            case BuildingsData.PlotType.INDUSTRIAL:
                _colour = Color.yellow;
                break;
            case BuildingsData.PlotType.PARK:
                break;
            default:
                break;
        }
    }

    public bool IsUndefinedPlotType()
    {
        return _plotType == BuildingsData.PlotType.UNDEFINED;
    }

    public float GetDistanceFromCentre()
    {
        return _distanceFromCentre;
    }

    public int GetLandSize()
    {
        return _landSize;
    }

    public void SetCentreOfCity(int cityPlotsWidth, int cityPlotsHeight)
    {
        _centreOfCity = new Vector2(cityPlotsWidth * 0.25f, cityPlotsHeight * 0.25f);
    }

    private Vector2 RecalculateCentre(GameObject _newPlot)
    {
        Vector2 weightedCentre = _centreOfLand * (_landSize);
        return new Vector2(weightedCentre.x + _newPlot.transform.position.z,
                           weightedCentre.y + _newPlot.transform.position.x) / (_landSize + 1);
    }

    private float GetDistanceFromCentreOfCity()
    {
        Vector2 distanceVector = _centreOfCity - _centreOfLand;
        return distanceVector.magnitude;
    }

    public IEnumerator CreateBuildVolume(Action action)
    {
        foreach (GameObject plot in _buildingPlots)
        {
            plot.GetComponent<BuildingPlot>().CreateBuildVolume(_maxBuildingHeight);
            yield return null;
        }

        foreach (GameObject plot in _buildingPlots)
        {
            plot.GetComponent<BuildingPlot>().CreateLinks();
        }
        action();
    }

    private int CalculateMaxBuildingHeight()
    {
        int minHeight = BuildingsData.MIN_BUILDING_HEIGHT;
        int maxHeight = BuildingsData.MAX_BUILDING_HEIGHT;
        float maxDistance = _centreOfCity.magnitude;
        float distance = (_centreOfCity - _centreOfLand).magnitude < 1.0f ? 1.0f : (_centreOfCity - _centreOfLand).magnitude;

        float ratio = (maxDistance / (distance * 0.5f)) * ((maxHeight - minHeight) / maxDistance) + minHeight;
        return Mathf.FloorToInt(ratio);
    }

    public void StartBuildingGeneration(Action construtionReadyFunction)
    {
        AddAllVolumesToList();
        SortVolumesByEntrophy();
        StartCoroutine(GenerateBuildings(construtionReadyFunction));
    }

    public void AddAllVolumesToList()
    {
        foreach (GameObject plot in _buildingPlots)
        {
            BuildingPlot plotScript = plot.GetComponent<BuildingPlot>();
            for(int i = 0; i < _maxBuildingHeight; ++i)
            {
                _buildVolumes.Add(new List<GameObject> { });
            }

            foreach (GameObject volume in plotScript.GetBuildVolumes())
            {
                BuildVolume volumeScript = volume.GetComponent<BuildVolume>();

                _buildVolumes[volumeScript.GetLevel()].Add(volume);
                volumeScript.RemoveInvalidBlocks();
            }
        }
    }

    private void SortVolumesByEntrophy()
    {
        for (int i = 0; i < _buildVolumes.Count; ++i)
        {
            SortLevelByEntrophy(i);
        }
    }

    private void SortLevelByEntrophy(int level)
    {
        _buildVolumes[level].Sort(delegate (GameObject a, GameObject b) {
            return (a.GetComponent<BuildVolume>().GetEntrophy()).CompareTo(b.GetComponent<BuildVolume>().GetEntrophy());
        });
    }

    private IEnumerator GenerateBuildings(Action action)
    {
        for (int i = 0; i < _buildVolumes.Count; ++i)
        {
            SortLevelByEntrophy(i);
            while (_buildVolumes[i].Count > 0)
            {
                if (_plotType == BuildingsData.PlotType.PARK)
                {
                    InstantiateParkTile(_buildVolumes[i][0]);
                }
                else
                {
                    InstantiateBuildingBlock(_buildVolumes[i][0], i);
                }
                yield return new WaitForSeconds(0.002f);
            }
        }   
        action();
        yield break;
    }

    private void InstantiateBuildingBlock(GameObject volume, int level)
    {
        BuildVolume volumeScript = volume.GetComponent<BuildVolume>();
        bool lastLevel = level == (_maxBuildingHeight - 1);
        
        GameObject block = Instantiate(_buildingManager.GetTestBlock(volumeScript.SelectRandomBlock()),
                                      volumeScript.GetPostition(),
                                      Quaternion.identity);

        if (_plotType == BuildingsData.PlotType.RESIDENTIAL)
        {
            block.GetComponent<BuildingClass>().CreateBuildingBlock(_plotType,
                                                                    _bpm.GetComponent<BuildingsPrefabManager>(),
                                                                    volumeScript.GetIdOfBlockBelow(),
                                                                    lastLevel);
        }
        else
        {
            block.GetComponent<BuildingClass>().SetColour(_colour);
        }
        
        Propogate(volumeScript, block);
        block.transform.SetParent(gameObject.transform);
        volumeScript.SetSolved();

        _buildVolumes[level].Remove(volume);

        SortLevelByEntrophy(level);
    }

    private void InstantiateParkTile(GameObject volume)
    {
        BuildVolume volumeScript = volume.GetComponent<BuildVolume>();

        GameObject block = Instantiate(_bpm.GetBuilding(BuildingsData.PlotType.PARK, 0),
                                      volumeScript.GetPostition(),
                                      Quaternion.identity);

        //Propogate(volumeScript, block);
        block.transform.SetParent(gameObject.transform);
        volumeScript.SetSolved();

        _buildVolumes[0].Remove(volume);

        //SortVolumesByEntrophy();
    }

    private void Propogate(BuildVolume volumeScript, GameObject block)
    {
        // Propogate to North
        if (volumeScript.GetNorth())
        {
            BuildVolume north = volumeScript.GetNorth().GetComponent<BuildVolume>();
            if (!north.IsSolved())
            {
                PropogateToBlock(volumeScript.GetNorth(), block.GetComponent<BuildingClass>().GetNorth());
                north.Propogate();
            }
        }

        // Propogate to West
        if (volumeScript.GetWest())
        {
            BuildVolume west = volumeScript.GetWest().GetComponent<BuildVolume>();
            if (!west.IsSolved())
            {
                PropogateToBlock(volumeScript.GetWest(), block.GetComponent<BuildingClass>().GetWest());
                west.Propogate();
            }
        }

        // Propogate to South
        if (volumeScript.GetSouth())
        {
            BuildVolume south = volumeScript.GetSouth().GetComponent<BuildVolume>();
            if (!south.IsSolved())
            {
                PropogateToBlock(volumeScript.GetSouth(), block.GetComponent<BuildingClass>().GetSouth());
                south.Propogate();
            }
        }

        // Propogate to East
        if (volumeScript.GetEast())
        {
            BuildVolume east = volumeScript.GetEast().GetComponent<BuildVolume>();
            if (!east.IsSolved())
            {
                PropogateToBlock(volumeScript.GetEast(), block.GetComponent<BuildingClass>().GetEast());
                east.Propogate();
            }
        }

        // Propogate to Up
        if (volumeScript.GetUp())
        {
            BuildVolume up = volumeScript.GetUp().GetComponent<BuildVolume>();
            if (!up.IsSolved())
            {
                PropogateToBlock(volumeScript.GetUp(), block.GetComponent<BuildingClass>().GetAbove());
                up.Propogate();
            }
        }

        // Propogate to Down
        
        if (volumeScript.GetDown())
        {
            BuildVolume down = volumeScript.GetDown().GetComponent<BuildVolume>();
            if (!down.IsSolved())
            {
                PropogateToBlock(volumeScript.GetDown(), block.GetComponent<BuildingClass>().GetBelow());
                down.Propogate();
            }
        }
    }

    private void PropogateToBlock(GameObject buildVolume, List<int> _validBlocks)
    {
        BuildVolume volumeScript = buildVolume.GetComponent<BuildVolume>();
        volumeScript.Collapse(_buildingManager.GetInvalidBlocks(_validBlocks));
    }
}
