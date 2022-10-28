using System.Collections.Generic;
using UnityEngine;

public static class RoadData
{
    static IDictionary<int, int> RoadWeights = new Dictionary<int, int>()
    {
        { 0,  1 }, // Empty
        { 1,  50 }, // Road, Horizontal S
        { 2,  50 }, // Road, Horizontal N
        { 3,  50 }, // Road, Vertical E
        { 4,  50 }, // Road, Vertical W
        { 5,  35 }, // Curve, SE
        { 6,  35 }, // Curve, SW
        { 7,  35 }, // Curve, NW
        { 8,  35 }, // Curve, NE
        { 9,  5 }, // Corner, SE
        { 10, 5 }, // Corner, SW
        { 11, 5 }, // Corner, NW
        { 12, 5 }, // Corner, NE
    };

    public static int GetWeightedRoadIndex(List<int> _indices)
    {
        List<WeightPairs> weightedValues = new List<WeightPairs>();
        int sumWeight = 0;

        foreach (int index in _indices)
        {
            sumWeight += RoadWeights[index];
            weightedValues.Add(new WeightPairs(index,sumWeight));
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
}
