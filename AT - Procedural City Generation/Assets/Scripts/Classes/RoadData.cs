using System.Collections.Generic;
using UnityEngine;

public static class RoadData
{
    static IDictionary<int, float> RoadWeights = new Dictionary<int, float>()
    {
        { 0,  1.0f }, // Empty
        { 1,  2.5f }, // Road, Horizontal S
        { 2,  2.5f }, // Road, Horizontal N
        { 3,  2.5f }, // Road, Vertical E
        { 4,  2.5f }, // Road, Vertical W
        { 5,  1.75f }, // Curve, SE
        { 6,  1.75f }, // Curve, SW
        { 7,  1.75f }, // Curve, NW
        { 8,  1.75f }, // Curve, NE
        { 9,  0.2f }, // Corner, SE
        { 10, 0.2f }, // Corner, SW
        { 11, 0.2f }, // Corner, NW
        { 12, 0.2f }, // Corner, NE
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
