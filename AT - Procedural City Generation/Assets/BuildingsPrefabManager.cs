using System.Collections.Generic;
using UnityEngine;

public class BuildingsPrefabManager : MonoBehaviour
{
    [Header("Residential prefabs")]
    [SerializeField] List<GameObject> _residentialSections = new List<GameObject>();

    [Header("Commercial prefabs")]
    [SerializeField] List<GameObject> _commercialSections = new List<GameObject>();

    [Header("Industrial prefabs")]
    [SerializeField] List<GameObject> _industrialSections = new List<GameObject>();

    [Header("Park")]
    [SerializeField] GameObject _park;

    public GameObject GetBuilding(BuildingsData.PlotType type, int id)
    {
        switch (type)
        {
            case BuildingsData.PlotType.UNDEFINED:
                return null;
            case BuildingsData.PlotType.RESIDENTIAL:
                return _residentialSections[id];
            case BuildingsData.PlotType.COMMERCIAL:
                return _commercialSections[id];
            case BuildingsData.PlotType.INDUSTRIAL:
                return _industrialSections[id];
            case BuildingsData.PlotType.PARK:
                return _park;
            default:
                return null;
        }
    }
}
