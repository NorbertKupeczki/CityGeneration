using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadNetworkManager : MonoBehaviour
{
    private enum GenerationMethod
    {
        MANUAL = 0,
        AUTO = 1,
        VISUALIZED = 2
    }

    [Header("Tiles")]
    [SerializeField] GameObject[] tiles = new GameObject[] { };

    [Header("Wave Funciton Collapse data")]
    [SerializeField] GameObject wfcTile;
    [SerializeField] List<GameObject> wfcTiles = new List<GameObject>();
    [SerializeField] List<GameObject> shortlistedTiles = new List<GameObject>();

    [Header("Road network")]
    [SerializeField] GenerationMethod generationMethod = GenerationMethod.AUTO;
    [SerializeField] int width;
    [SerializeField] int height;

    private void Awake()
    {
        PerlinGenerator perlinGenerator = FindObjectOfType<PerlinGenerator>();
        perlinGenerator.SetMapSize(width, height);
    }

    void Start()
    {
        GenerateGrid();
        //InstantiateTile(PickRandomGridTile());

        switch (generationMethod)
        {
            case GenerationMethod.MANUAL:
                break;
            case GenerationMethod.AUTO:
                GenerateRoadNetwork();
                break;
            case GenerationMethod.VISUALIZED:
                StartCoroutine(GenerateRoads());
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) &&
            generationMethod == GenerationMethod.MANUAL)
        {
            InstantiateTile(shortlistedTiles[0]);
        }
    }

    IEnumerator GenerateRoads()
    {
        while (shortlistedTiles.Count > 0)
        {
            InstantiateTile(shortlistedTiles[0]);
            yield return new WaitForSeconds(0.025f);
        }
    }

    private void GenerateGrid()
    {
        int tileId = 0;
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                GameObject tile = Instantiate(wfcTile, new Vector3(x, 0, y), Quaternion.identity);
                tile.GetComponent<WFC_Tile>().SetID(tileId);
                wfcTiles.Add(tile);

                if(tileId > 0 && tileId % width != 0)
                {
                    tile.GetComponent<WFC_Tile>().SetWestNeighbour(wfcTiles[tileId - 1]);
                    wfcTiles[tileId - 1].GetComponent<WFC_Tile>().SetEastNeighbour(tile);
                }

                if (tileId >= width)
                {
                    tile.GetComponent<WFC_Tile>().SetNorthNeighbour(wfcTiles[tileId - width]);
                    wfcTiles[tileId - width].GetComponent<WFC_Tile>().SetSouthNeighbour(tile);
                }

                ++tileId;
            }
        }
        
        GameObject edgeTile = wfcTiles[0];
        for (int i = 0; i < width; ++i)
        {
            if (i != Mathf.Floor(width * 0.5f) + 1)
            {
                InstantiateSpecificTile(edgeTile,
                                        i == Mathf.Floor(width * 0.5f) ? 3 : 0);
            }
            else
            {
                InstantiateSpecificTile(edgeTile, 4);
            }

            if (edgeTile.GetComponent<WFC_Tile>().GetID() == width -1)
            {
                edgeTile = edgeTile.GetComponent<WFC_Tile>().GetSouthNeighbour();
            }
            else
            {
                edgeTile = edgeTile.GetComponent<WFC_Tile>().GetEastNeighbour();
            }
        }

        for (int i = 1; i < height; ++i)
        {
            if (i != Mathf.Floor(height * 0.5f) + 1)
            {
                InstantiateSpecificTile(edgeTile,
                                    i == Mathf.Floor(height * 0.5f) ? 1 : 0);
            }
            else
            {
                InstantiateSpecificTile(edgeTile, 2);
            }
            
            if (edgeTile.GetComponent<WFC_Tile>().GetID() == height * width - 1)
            {
                edgeTile = edgeTile.GetComponent<WFC_Tile>().GetWestNeighbour();
            }
            else
            {
                edgeTile = edgeTile.GetComponent<WFC_Tile>().GetSouthNeighbour();
            }
        }

        for (int i = 1; i < width; ++i)
        {
            if (i != Mathf.Floor(width * 0.5f) + 1)
            {
                InstantiateSpecificTile(edgeTile,
                                    i == Mathf.Floor(height * 0.5f) ? 4 : 0);
            }
            else
            {
                InstantiateSpecificTile(edgeTile, 3);
            }
            
            if (edgeTile.GetComponent<WFC_Tile>().GetID() == (height -1) * width)
            {
                edgeTile = edgeTile.GetComponent<WFC_Tile>().GetNorthNeighbour();
            }
            else
            {
                edgeTile = edgeTile.GetComponent<WFC_Tile>().GetWestNeighbour();
            }
        }

        for (int i = 1; i < height - 1; ++i)
        {
            if (i != Mathf.Floor(height * 0.5f) + 1)
            {
                InstantiateSpecificTile(edgeTile,
                                    i == Mathf.Floor(height * 0.5f) ? 2 : 0);
            }
            else
            {
                InstantiateSpecificTile(edgeTile, 1);
            }

            edgeTile = edgeTile.GetComponent<WFC_Tile>().GetNorthNeighbour();
            
        }
    }

    private void GenerateRoadNetwork()
    {
        while(shortlistedTiles.Count > 0)
        {
            InstantiateTile(shortlistedTiles[0]);
        }
    }

    private GameObject PickRandomGridTile()
    {
        return wfcTiles[Random.Range(0, wfcTiles.Count)];
    }

    private void InstantiateSpecificTile(GameObject _wfcTile, int _tileID)
    {
        WFC_Tile tileScript = _wfcTile.GetComponent<WFC_Tile>();

        GameObject tile = Instantiate(tiles[_tileID],
                                      tileScript.GetPostition(),
                                      Quaternion.identity);
        Propogate(tileScript, tile);
        tile.GetComponent<RoadClass>().SetMapSize(new Vector2(width, height));

        tileScript.SetSolved();

        wfcTiles.Remove(_wfcTile);
        shortlistedTiles.Remove(_wfcTile);

        SortShortlist();
    }

    private void InstantiateTile(GameObject _wfcTile)
    {
        WFC_Tile tileScript = _wfcTile.GetComponent<WFC_Tile>();

        GameObject tile = Instantiate(tiles[tileScript.SelectRandomTile()],
                                      tileScript.GetPostition(),
                                      Quaternion.identity);

        Propogate(tileScript, tile);

        tileScript.SetSolved();

        wfcTiles.Remove(_wfcTile);
        shortlistedTiles.Remove(_wfcTile);

        SortShortlist();
    }

    private void Propogate(WFC_Tile tileScript, GameObject tile)
    {
        // Propogate to North
        if (tileScript.GetNorthNeighbour())
        {
            if (!tileScript.GetNorthNeighbour().GetComponent<WFC_Tile>().IsSolved())
            {
                PropogateToTile(tileScript.GetNorthNeighbour(), tile.GetComponent<RoadClass>().GetNorth());
                ShortlistTile(tileScript.GetNorthNeighbour());
            }
        }

        // Propogate to South
        if (tileScript.GetSouthNeighbour())
        {
            if (!tileScript.GetSouthNeighbour().GetComponent<WFC_Tile>().IsSolved())
            {
                PropogateToTile(tileScript.GetSouthNeighbour(), tile.GetComponent<RoadClass>().GetSouth());
                ShortlistTile(tileScript.GetSouthNeighbour());
            }
        }

        // Propogate to West
        if (tileScript.GetWestNeighbour())
        {
            if (!tileScript.GetWestNeighbour().GetComponent<WFC_Tile>().IsSolved())
            {
                PropogateToTile(tileScript.GetWestNeighbour(), tile.GetComponent<RoadClass>().GetWest());
                ShortlistTile(tileScript.GetWestNeighbour());
            }
        }

        // Propogate to East
        if (tileScript.GetEastNeighbour())
        {
            if (!tileScript.GetEastNeighbour().GetComponent<WFC_Tile>().IsSolved())
            {
                PropogateToTile(tileScript.GetEastNeighbour(), tile.GetComponent<RoadClass>().GetEast());
                ShortlistTile(tileScript.GetEastNeighbour());
            }
        }
    }

    private void PropogateToTile(GameObject _wfcTile, List<int> _validTiles)
    {
        WFC_Tile tileScript = _wfcTile.GetComponent<WFC_Tile>();
        tileScript.Collapse(GetInvalidTiles(_validTiles));
    }

    private void ShortlistTile(GameObject tile)
    {
        if (!shortlistedTiles.Contains(tile))
        {
            shortlistedTiles.Add(tile);
        }            
    }

    private void SortShortlist()
    {
        if (shortlistedTiles.Count > 0)
        {
            shortlistedTiles.Sort(delegate (GameObject a, GameObject b) {
                return (a.GetComponent<WFC_Tile>().GetEntrophy()).CompareTo(b.GetComponent<WFC_Tile>().GetEntrophy());
            });
        }
    }

    private List<int> GetInvalidTiles(List<int> validTiles)
    {
        List<int> invalidTiles = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

        foreach  (int item in validTiles)
        {
            invalidTiles.Remove(item);
        }

        return invalidTiles;
    }
}
