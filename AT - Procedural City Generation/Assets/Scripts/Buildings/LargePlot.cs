using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargePlot : MonoBehaviour
{
    [SerializeField] BuildingsData.PlotType _plotType = BuildingsData.PlotType.UNDEFINED;
    [SerializeField] int _landSize = 0;
    [SerializeField] List<GameObject> _buildingPlots;
    //[SerializeField] List<GameObject> _buildVolumes = null;


    private void Awake()
    {
        _plotType = (BuildingsData.PlotType)Random.Range(1, 5);
    }

    public void AddBuildingPlot(GameObject _newPlot)
    {
        _buildingPlots.Add(_newPlot);
    }

    public void SetPlotsToComplete()
    {
        foreach (GameObject _plot in _buildingPlots)
        {
            _landSize = _buildingPlots.Count;

            _plot.GetComponent<BuildingPlot>().SetLinkingState(BuildingsData.PlotLinking.COMPLETED);
            _plot.GetComponent<BuildingPlot>().SetPlotType(_plotType);
            _plot.GetComponent<BuildingPlot>().Build(1);
        }
    }

    public bool CheckPlotInList(GameObject plot)
    {
        return _buildingPlots.Contains(plot);
    }

    private void ApplyPlotType()
    {
        foreach (GameObject plot in _buildingPlots)
        {
            plot.GetComponent<BuildingPlot>().SetPlotType(_plotType);
        }
    }

    public int GetLandSize()
    {
        return _landSize;
    }
}
