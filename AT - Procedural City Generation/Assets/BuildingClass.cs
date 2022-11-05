using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingClass : MonoBehaviour
{
    [Header("Base information")]
    [SerializeField] string _name;
    [SerializeField] int id;

    [Header("Valid Neighbours")]
    [SerializeField] List<int> north = new List<int> { };
    [SerializeField] List<int> west = new List<int> { };
    [SerializeField] List<int> south = new List<int> { };
    [SerializeField] List<int> east = new List<int> { };
    [SerializeField] List<int> above = new List<int> { };
    [SerializeField] List<int> below = new List<int> { };

    private Renderer renderer;
    
    private void Awake()
    {
        renderer = GetComponentInChildren<Renderer>();
    }

    public void SetColour(Color colour)
    {
        if(renderer != null)
        {
            renderer.material.color = colour;
        }
    }

    #region ">> Valid tile accessor functions"
    public List<int> GetNorth()
    {
        return north;
    }
    public List<int> GetWest()
    {
        return west;
    }
    public List<int> GetSouth()
    {
        return south;
    }
    public List<int> GetEast()
    {
        return east;
    }
    public List<int> GetAbove()
    {
        return above;
    }
    public List<int> GetBelow()
    {
        return below;
    }
    #endregion
}
