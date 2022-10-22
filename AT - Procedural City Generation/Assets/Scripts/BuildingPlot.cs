using UnityEngine;

public class BuildingPlot : MonoBehaviour
{
    [SerializeField] GameObject buildingBlock;
    [SerializeField] float scaleFactor;

    public void Build(int levels)
    {
        /*for (int i = 0; i < levels; i++)
        Instantiate(buildingBlock,
                    new Vector3(gameObject.transform.position.x, gameObject.transform.localScale.y * (i + 0.5f) * scaleFactor, gameObject.transform.position.z),
                    Quaternion.identity);*/

        GameObject building = Instantiate(buildingBlock,
                              new Vector3 (gameObject.transform.position.x,
                                           gameObject.transform.localScale.y * (levels * 0.5f) * scaleFactor,
                                           gameObject.transform.position.z),
                              Quaternion.identity);
        building.transform.localScale = new Vector3 (building.transform.localScale.x,
                                                     building.transform.localScale.y * levels,
                                                     building.transform.localScale.z);
    }
}
