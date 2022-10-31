using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargePlot : MonoBehaviour
{
    [SerializeField] BuildingsData.PlotType _plotType = BuildingsData.PlotType.UNDEFINED;
    [SerializeField] int _landSize = 0;
    [SerializeField] Vector2 _centreOfLand;
    [SerializeField] List<GameObject> _buildingPlots;
    //[SerializeField] List<GameObject> _buildVolumes = null;

    [Header("District Position")]
    [SerializeField] private Vector2 _centreOfCity;
    [SerializeField] private float _distanceFromCentre;

    private void Awake()
    {
        //_plotType = (BuildingsData.PlotType)Random.Range(1, 5);
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
            _plot.GetComponent<BuildingPlot>().Build(1);
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

    public IEnumerator CreateBuildVolume()
    {
        foreach (GameObject plot in _buildingPlots)
        {
            plot.GetComponent<BuildingPlot>().CreateBuildVolume();
        }

        foreach (GameObject plot in _buildingPlots)
        {
            plot.GetComponent<BuildingPlot>().LinkVolumes();
        }

        yield return null;
    }
}
