using System.Collections.Generic;
using UnityEngine;

public static class RoadData
{
    static IDictionary<int, float> RoadWeights = new Dictionary<int, float>()
    {
        { 0,  5.0f }, // Empty
        { 1,  3.0f }, // Road, Horizontal
        { 2,  3.0f }, // Road, Vertical
        { 3,  0.5f }, // Curve, SE
        { 4,  0.5f }, // Curve, SW
        { 5,  0.5f }, // Curve, NW
        { 6,  0.5f }, // Curve, NE
        { 7,  0.5f }, // T-junktion, N
        { 8,  0.5f }, // T-junktion, E
        { 9,  0.5f }, // T-junktion, S
        { 10, 0.5f }, // T-junktion, W
        { 11, 1.0f }, // Crossroad
    };

    public static int GetWeightedRoadIndex(List<int> _indices)
    {
        List<WeightPairs> weightedValues = new List<WeightPairs>();
        float sumWeight = 0.0f;

        foreach (int index in _indices)
        {
            sumWeight += RoadWeights[index];
            weightedValues.Add(new WeightPairs(index,sumWeight));
        }

        float rnd = Random.Range(0.0f, sumWeight);

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
        public float weight;

        public WeightPairs(int _idx, float weight) : this()
        {
            this.idx = _idx;
            this.weight = weight;
        }
    }
}
