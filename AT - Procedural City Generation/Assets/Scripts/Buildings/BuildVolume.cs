using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildVolume : MonoBehaviour
{
    [Header("Basics")]
    [SerializeField] int id;
    [SerializeField] List<GameObject> _buildingBlocks;
    [SerializeField] int entrophy;
    [SerializeField] bool solved;

    [Header("Neighbours")]
    [SerializeField] GameObject _north;
    [SerializeField] GameObject _west;
    [SerializeField] GameObject _south;
    [SerializeField] GameObject _east;
    [SerializeField] GameObject _up;
    [SerializeField] GameObject _down;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
