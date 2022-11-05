using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuildingsData
{
    public const int MAX_BUILDING_HEIGHT = 10;
    public const int MIN_BUILDING_HEIGHT = 3;

    static IDictionary<int, int> buildingWeights = new Dictionary<int, int>()
    {
        { 0,  10 }, // Inside-empty
        { 1,  50 }, // NW Corner
        { 2,  50 }, // N Edge
        { 3,  50 }, // NE Corner
        { 4,  50 }, // W Edge
        { 5,  35 }, // E Edge
        { 6,  35 }, // SW Corner
        { 7,  35 }, // S Edge
        { 8,  35 }, // SE Corner
        { 9,  5 }, // S-E Curve
        { 10, 5 }, // S-W Curve
        { 11, 5 }, // N-E Curve
        { 12, 5 }, // N-W Curve
        { 13, 5 }, // N Single line
        { 14, 5 }, // S Single line
        { 15, 5 }, // W Single line
        { 16, 5 }, // E Single line
        { 17, 15 }, // NE-S Narrowing
        { 18, 15 }, // NW-S Narrowing
        { 19, 15 }, // SE-N Narrowing
        { 20, 15 }, // SW-N Narrowing
        { 21, 15 }, // SW-E Narrowing
        { 22, 15 }, // SE-W Narrowing
        { 23, 15 }, // NW-E Narrowing
        { 24, 15 }, // NW-W Narrowing
        { 25, 5 }, // Single block
        { 26, 7 }, // Empty volume
    };

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

    public enum Direction3D
    {
        NORTH = 0,
        WEST = 1,
        SOUTH = 2,
        EAST = 3,
        UP = 4,
        DOWN = 5
    }

    struct WeightPairs
    {
        public int idx;
        public int weight;

        public WeightPairs(int _idx, int weight) : this()
        {
            this.idx = _idx;
            this.weight = weight;
        }
    }

    public static int GetWeightedBlockIndex(List<int> _indices)
    {
        if (_indices.Count < 2)
        {
            return _indices[0];
        }    
        List<WeightPairs> weightedValues = new List<WeightPairs>();
        int sumWeight = 0;

        foreach (int index in _indices)
        {
            sumWeight += buildingWeights[index];
            weightedValues.Add(new WeightPairs(index, sumWeight));
        }

        int rnd = Random.Range(0, sumWeight);

        foreach (WeightPairs weightedPair in weightedValues)
        {
            if (rnd < weightedPair.weight)
            {
                return weightedPair.idx;
            }
        }

        return -1;
    }
}
