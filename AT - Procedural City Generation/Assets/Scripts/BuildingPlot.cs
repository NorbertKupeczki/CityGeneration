using UnityEngine;

public class BuildingPlot : MonoBehaviour
{
    [SerializeField] GameObject buildingBlock;
    [SerializeField] float scaleFactor = 0.333f;

    public void Build(int levels)
    {
        for (int i = 0; i < levels; i++)
        Instantiate(buildingBlock,
                    new Vector3(gameObject.transform.position.x, gameObject.transform.localScale.y * (i + 0.5f) * scaleFactor, gameObject.transform.position.z),
                    Quaternion.identity);
    }
}
