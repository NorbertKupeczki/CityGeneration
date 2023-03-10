using UnityEngine;

public class BuildingLevelSelector : MonoBehaviour
{
    [Header("Building level prefabs")]
    [SerializeField] GameObject _base;
    [SerializeField] GameObject _middle;
    [SerializeField] GameObject _top;

    private void Awake()
    {
        _base.SetActive(false);
        _middle.SetActive(false);
        _top.SetActive(false);
    }

    public void SelectLevel (BuildingsData.BuildingLevel level)
    {
        switch (level)
        {
            case BuildingsData.BuildingLevel.BASE:
                _base.SetActive(true);
                // Destroy(_middle);
                // Destroy(_top);
                break;
            case BuildingsData.BuildingLevel.MID:
                _middle.SetActive(true);
                // Destroy(_base);
                // Destroy(_top);
                break;
            case BuildingsData.BuildingLevel.TOP:
                _top.SetActive(true);
                // Destroy(_base);
                // Destroy(_middle);
                break;
            default:
                break;
        }
    }
}
