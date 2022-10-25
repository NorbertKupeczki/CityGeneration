using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuildingsData
{
    private static List<int> _baseBlocks = new List<int> { };
    private static List<int> _midBlocks = new List<int> { };
    private static List<int> _topBlocks = new List<int> { };

    public enum BuildingLevel
    {
        BASE = 0,
        MID = 1,
        TOP = 2
    }

    public enum PlotLinking
    {
        NOT_STARTED = 0,
        IN_PROGRESS = 1,
        COMPLETED = 2
    }

    public enum PlotType
    {
        UNDEFINED = 0,
        RESIDENTIAL = 1,
        COMMERCIAL = 2,
        INDUSTRIAL = 3,
        PARK = 4
    }

    public enum Direction
    {
        NORTH = 0,
        WEST = 1,
        SOUTH = 2,
        EAST = 3
    }

}
