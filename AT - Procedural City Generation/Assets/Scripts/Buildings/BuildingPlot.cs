using System.Collections.Generic;
using UnityEngine;

public class BuildingPlot : MonoBehaviour
{
    [SerializeField] GameObject buildingBlock;
    [SerializeField] float scaleFactor;
    [SerializeField] float curbHeight = 0.02f;
    [SerializeField] BuildingsData.PlotType plotType = BuildingsData.PlotType.UNDEFINED;

    [Header("Neighbours")]
    [SerializeField] BuildingsData.PlotLinking _plotLink = BuildingsData.PlotLinking.NOT_STARTED;
    [SerializeField] GameObject north;
    [SerializeField] GameObject west;
    [SerializeField] GameObject south;
    [SerializeField] GameObject east;

    [Header("Build Volume")]
    [SerializeField] GameObject buildVolumePrefab;
    [SerializeField] int maxHeight = 10;
    [SerializeField] List<GameObject> buildVolumes = new();


    public int GetMaxHeight()
    {
        return maxHeight;
    }

    public bool IsNotLinked()
    {
        return _plotLink == BuildingsData.PlotLinking.NOT_STARTED;
    }

    public BuildingsData.PlotLinking GetLinkState()
    {
        return _plotLink;
    }

    public void SetLinkingState(BuildingsData.PlotLinking newState)
    {
        _plotLink = newState;
    }

    #region ">> Neighbour mutator functions
    public void SetNorth(GameObject newNeighbour)
    {
        north = newNeighbour;
        // Link building volumes
    }
    public void SetWest(GameObject newNeighbour)
    {
        west = newNeighbour;
        // Link building volumes
    }
    public void SetSouth(GameObject newNeighbour)
    {
        south = newNeighbour;
        // Link building volumes
    }
    public void SetEast(GameObject newNeighbour)
    {
        east = newNeighbour;
        // Link building volumes
    }
    #endregion

    public bool HasNeighbour (BuildingsData.Direction direction)
    {
        switch (direction)
        {
            case BuildingsData.Direction.NORTH:
                return north != null;
            case BuildingsData.Direction.WEST:
                return west != null;
            case BuildingsData.Direction.SOUTH:
                return south != null;
            case BuildingsData.Direction.EAST:
                return east != null;
            default:
                return false;
        }
    }

    public void SetPlotType (BuildingsData.PlotType newType)
    {
        plotType = newType;
    }

    public List<GameObject> GetBuildVolumes()
    {
        return buildVolumes;
    }

    public void CreateBuildVolume(int maxBuildingHeight)
    {
        maxHeight = maxBuildingHeight;
        if (plotType == BuildingsData.PlotType.PARK)
        {
            GameObject newVolume = Instantiate(buildVolumePrefab, new Vector3(gameObject.transform.position.x, 0.0f, gameObject.transform.position.z), Quaternion.identity);
            buildVolumes.Add(newVolume);
            newVolume.transform.SetParent(gameObject.transform);
        }
        else
        {
            for (int i = 0; i < maxHeight; ++i)
            {

                GameObject newVolume = Instantiate(buildVolumePrefab, new Vector3(gameObject.transform.position.x, i * 0.25f, gameObject.transform.position.z), Quaternion.identity);
                buildVolumes.Add(newVolume);
                newVolume.transform.SetParent(gameObject.transform);
                newVolume.GetComponent<BuildVolume>().SetMinHeight(Mathf.CeilToInt(maxHeight * 0.5f));
                if (i > 0)
                {
                    LinkVolumes(newVolume.GetComponent<BuildVolume>(), BuildingsData.Direction3D.DOWN, buildVolumes[i - 1].GetComponent<BuildVolume>());
                }

            }
        }
    }
    
    public void CreateLinks()
    {
        if(north != null)
        {
            LinkVolumesWithNeighbour(BuildingsData.Direction.NORTH);
        }
        if (west != null)
        {
            LinkVolumesWithNeighbour(BuildingsData.Direction.WEST);
        }
        if (south != null)
        {
            LinkVolumesWithNeighbour(BuildingsData.Direction.SOUTH);
        }
        if (east != null)
        {
            LinkVolumesWithNeighbour(BuildingsData.Direction.EAST);
        }
    }

    private void LinkVolumes (BuildVolume a, BuildingsData.Direction3D direction, BuildVolume b)
    {
        if (!a.HasNeighbour(direction))
        {
            switch (direction)
            {
                case BuildingsData.Direction3D.NORTH:
                    a.SetNorth(b.gameObject);
                    b.SetSouth(a.gameObject);
                    break;
                case BuildingsData.Direction3D.WEST:
                    a.SetWest(b.gameObject);
                    b.SetEast(a.gameObject);
                    break;
                case BuildingsData.Direction3D.SOUTH:
                    a.SetSouth(b.gameObject);
                    b.SetNorth(a.gameObject);
                    break;
                case BuildingsData.Direction3D.EAST:
                    a.SetEast(b.gameObject);
                    b.SetWest(a.gameObject);
                    break;
                case BuildingsData.Direction3D.UP:
                    a.SetUp(b.gameObject);
                    b.SetDown(a.gameObject);
                    break;
                case BuildingsData.Direction3D.DOWN:
                    a.SetDown(b.gameObject);
                    b.SetUp(a.gameObject);
                    break;
                default:
                    break;
            }
        }
    }

    private void LinkVolumesWithNeighbour(BuildingsData.Direction direction)
    {
        switch (direction)
        {
            case BuildingsData.Direction.NORTH:                
                List<GameObject> northVolumes = north.GetComponent<BuildingPlot>().GetBuildVolumes();
                for (int i = 0; i < buildVolumes.Count; ++i)
                {
                    LinkVolumes(buildVolumes[i].GetComponent<BuildVolume>(), BuildingsData.Direction3D.NORTH, northVolumes[i].GetComponent<BuildVolume>());
                }
                
                break;

            case BuildingsData.Direction.WEST:
                List<GameObject> westVolumes = west.GetComponent<BuildingPlot>().GetBuildVolumes();
                for (int i = 0; i < buildVolumes.Count; ++i)
                {
                    LinkVolumes(buildVolumes[i].GetComponent<BuildVolume>(), BuildingsData.Direction3D.WEST, westVolumes[i].GetComponent<BuildVolume>());
                }
                break;

            case BuildingsData.Direction.SOUTH:
                List<GameObject> southVolumes = south.GetComponent<BuildingPlot>().GetBuildVolumes();
                for (int i = 0; i < buildVolumes.Count; ++i)
                {
                    LinkVolumes(buildVolumes[i].GetComponent<BuildVolume>(), BuildingsData.Direction3D.SOUTH, southVolumes[i].GetComponent<BuildVolume>());
                }
                break;

            case BuildingsData.Direction.EAST:
                List<GameObject> eastVolumes = east.GetComponent<BuildingPlot>().GetBuildVolumes();
                for (int i = 0; i < buildVolumes.Count; ++i)
                {
                    LinkVolumes(buildVolumes[i].GetComponent<BuildVolume>(), BuildingsData.Direction3D.EAST, eastVolumes[i].GetComponent<BuildVolume>());
                }
                break;

            default:
                break;
        }
    }
}
