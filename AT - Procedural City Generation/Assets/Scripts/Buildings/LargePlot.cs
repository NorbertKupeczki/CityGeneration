using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargePlot : MonoBehaviour
{
    [SerializeField] BuildingsData.PlotType _plotType = BuildingsData.PlotType.UNDEFINED;
    [SerializeField] int _landSize = 0;
    [SerializeField] List<GameObject> _buildingPlots;
    [SerializeField] List<GameObject> _buildVolumes = null;

    // CODE FOR TESTING
    private Color color;

    private void Awake()
    {
        color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
    }

    public void AddBuildingPlot(GameObject _newPlot)
    {
        _buildingPlots.Add(_newPlot);
    }

    public void SetPlotsToComplete()
    {
        foreach (GameObject _plot in _buildingPlots)
        {
            _plot.GetComponent<BuildingPlot>().SetLinkingState(BuildingsData.PlotLinking.COMPLETED);
        }
    }

    public bool CheckPlotInList(GameObject plot)
    {
        return _buildingPlots.Contains(plot);
    }
}
