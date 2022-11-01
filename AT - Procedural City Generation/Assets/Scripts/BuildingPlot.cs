using System.Collections.Generic;
using UnityEngine;

public class BuildingPlot : MonoBehaviour
{
    [SerializeField] GameObject buildingBlock;
    [SerializeField] float scaleFactor;
    [SerializeField] float curbHeight = 0.02f;
    [SerializeField] BuildingsData.PlotType plotType = BuildingsData.PlotType.UNDEFINED;
    [SerializeField] Vector2 worldCoord;

    private BuildingManager manager;

    [Header("Neighbours")]
    [SerializeField] BuildingsData.PlotLinking _plotLink = BuildingsData.PlotLinking.NOT_STARTED;
    [SerializeField] GameObject north;
    [SerializeField] GameObject west;
    [SerializeField] GameObject south;
    [SerializeField] GameObject east;

    [Header("Build Volume")]
    [SerializeField] GameObject buildVolume;
    [SerializeField] int maxHeight = 10;
    [SerializeField] List<GameObject> buildVolumes = new List<GameObject> { };

    private void Awake()
    {
        manager = FindObjectOfType<BuildingManager>();
        worldCoord = new Vector2(gameObject.transform.position.z, gameObject.transform.position.x);
    }

    public void Build(int levels)
    {
        switch (plotType)
        {
            case BuildingsData.PlotType.UNDEFINED:
                break;
            case BuildingsData.PlotType.RESIDENTIAL:
                BuildTestBlock(levels, Color.green);
                break;
            case BuildingsData.PlotType.COMMERCIAL:
                BuildTestBlock(levels, Color.blue);
                break;
            case BuildingsData.PlotType.INDUSTRIAL:
                BuildTestBlock(levels, Color.yellow);
                break;
            case BuildingsData.PlotType.PARK:
                GameObject park = Instantiate(manager.GetParkTile(),
                                  new Vector3(gameObject.transform.position.x, gameObject.transform.localPosition.y, gameObject.transform.position.z),
                                  Quaternion.Euler(new Vector3(gameObject.transform.rotation.x, 90 * Random.Range(0, 5) ,gameObject.transform.rotation.z)));
                park.transform.SetParent(gameObject.transform);
                break;
            default:
                break;
        }

        /*for (int i = 0; i < levels; i++)
        {
            if (i == 0)
            {
                buildingBlock = manager.GetBuildingBlock(BuildingsData.BuildingLevel.BASE);
            }
            else if (i == levels - 1)
            {
                buildingBlock = manager.GetBuildingBlock(BuildingsData.BuildingLevel.TOP);
            }
            else
            {
                buildingBlock = manager.GetBuildingBlock(BuildingsData.BuildingLevel.MID);
            }

            Instantiate(buildingBlock,
                        new Vector3(gameObject.transform.position.x, gameObject.transform.localScale.y * i * 0.5f * scaleFactor, gameObject.transform.position.z),
                        Quaternion.identity);
        }*/
    }

    private void BuildTestBlock(int levels, Color colour)
    {
        GameObject building = Instantiate(buildingBlock,
                              new Vector3 (gameObject.transform.position.x,
                                           gameObject.transform.localScale.y * (levels * 0.5f) * scaleFactor + curbHeight,
                                           gameObject.transform.position.z),
                              Quaternion.identity);
        building.transform.localScale = new Vector3 (building.transform.localScale.x,
                                                     building.transform.localScale.y * levels,
                                                     building.transform.localScale.z);
        building.transform.SetParent(gameObject.transform);
        building.GetComponent<Renderer>().material.color = colour;
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

        for (int i = 0; i < maxHeight; ++i)
        {
            buildVolumes.Add(Instantiate(buildVolume, new Vector3(gameObject.transform.position.x, i * 0.5f, gameObject.transform.position.z), Quaternion.identity));
            buildVolumes[i].transform.SetParent(gameObject.transform);
            if (i > 0)
            {
                LinkVolumes(buildVolumes[i].GetComponent<BuildVolume>(), BuildingsData.Direction3D.DOWN, buildVolumes[i - 1].GetComponent<BuildVolume>());
            }
        }
    }
    
    public void LinkVolumes()
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
