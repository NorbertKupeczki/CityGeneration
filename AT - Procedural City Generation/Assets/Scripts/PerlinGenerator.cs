using UnityEngine;

public class PerlinGenerator : MonoBehaviour
{
    [SerializeField] Vector2 mapSize;
    [SerializeField] Texture2D perlinMap;
    [SerializeField] float scale = 200.0f;
    [SerializeField] float xOffset;
    [SerializeField] float yOffset;

    public Renderer meshRenderer;
    // Start is called before the first frame update
    void Awake()
    {
        xOffset = Random.Range(0.0f, 999.0f);
        yOffset = Random.Range(0.0f, 999.0f);
        meshRenderer.material.mainTexture = GeneratePerlinMap();
    }

    public void SetMapSize(int width, int height)
    {
        mapSize = new Vector2(width, height);
    }

    public float GetNoiseValue(int x, int y)
    {
        return perlinMap.GetPixel(x, y).r;
    }

    private Texture2D GeneratePerlinMap()
    {
        Texture2D noiseMap = new Texture2D(Mathf.FloorToInt(mapSize.x), Mathf.FloorToInt(mapSize.y));

        for (int y = 0; y < mapSize.y; ++y)
        {
            for (int x = 0; x < mapSize.x; ++x)
            {
                noiseMap.SetPixel(x, y, CalculateColour(x, y));
            }
        }
        noiseMap.Apply();
        perlinMap = noiseMap;
        return noiseMap;
    }

    private Color CalculateColour(int x, int y)
    {
        float sample = Mathf.PerlinNoise(x / mapSize.x * scale + xOffset,
                                         y / mapSize.y * scale + yOffset);
        return new Color(sample, sample, sample);
    }
}
