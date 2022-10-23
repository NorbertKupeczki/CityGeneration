using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public enum BuildingLevel
    {
        BASE = 0,
        MID = 1,
        TOP = 2
    }
    [SerializeField] List<GameObject> _residentialSmall = new List<GameObject>(3);

    public GameObject GetBuildingBlock(BuildingLevel level)
    {
        return _residentialSmall[(int)level];
    }
}
