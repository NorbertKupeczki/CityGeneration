using UnityEngine;

public class CityManager : MonoBehaviour
{
    [SerializeField] GameObject roadNetwork;
    [SerializeField] GameObject cityZones;

    public void AddRoadToHierarchy(GameObject road)
    {
        road.transform.SetParent(roadNetwork.transform);
    }

    public void AddZoneToHierarchy(GameObject zone)
    {
        zone.transform.SetParent(cityZones.transform);
    }
}
