using UnityEngine;

public class BuildingPlot : MonoBehaviour
{
    [SerializeField] GameObject buildingBlock;
    [SerializeField] float scaleFactor;
    [SerializeField] float curbHeight = 0.02f;
    [SerializeField] BuildingsData.PlotType plotType = BuildingsData.PlotType.UNDEFINED;

    private BuildingManager manager;

    [Header("Neighbours")]
    [SerializeField] BuildingsData.PlotLinking _plotLink = BuildingsData.PlotLinking.NOT_STARTED;
    [SerializeField] GameObject north;
    [SerializeField] GameObject west;
    [SerializeField] GameObject south;
    [SerializeField] GameObject east;

    private void Awake()
    {
        manager = FindObjectOfType<BuildingManager>();
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
                Instantiate(manager.GetParkTile(),
                        new Vector3(gameObject.transform.position.x, gameObject.transform.localPosition.y, gameObject.transform.position.z),
                        Quaternion.Euler(new Vector3(gameObject.transform.rotation.x, 90 * Random.Range(0, 5) ,gameObject.transform.rotation.z)));
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
}
