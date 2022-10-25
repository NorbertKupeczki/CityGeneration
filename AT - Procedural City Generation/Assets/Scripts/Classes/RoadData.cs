using System.Collections.Generic;
using UnityEngine;

public static class RoadData
{
    static IDictionary<int, int> RoadWeights = new Dictionary<int, int>()
    {
        { 0,  1 }, // Empty
        { 1,  25 }, // Road, Horizontal S
        { 2,  25 }, // Road, Horizontal N
        { 3,  25 }, // Road, Vertical E
        { 4,  25 }, // Road, Vertical W
        { 5,  25 }, // Curve, SE
        { 6,  25 }, // Curve, SW
        { 7,  25 }, // Curve, NW
        { 8,  25 }, // Curve, NE
        { 9,  3 }, // Corner, SE
        { 10, 3 }, // Corner, SW
        { 11, 3 }, // Corner, NW
        { 12, 3 }, // Corner, NE
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
