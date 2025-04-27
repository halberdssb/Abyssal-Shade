using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidCollectionHandler : MonoBehaviour
{
    public int numBoids;
    [SerializeField]
    private List<BoidObject> collectedBoids = new List<BoidObject>();

    public void AddCollectedBoid(BoidObject boid)
    {
        collectedBoids.Add(boid);
        numBoids++;
    }

    public void RemoveBoid(BoidObject boid)
    {
        if (collectedBoids.Contains(boid))
        {
            collectedBoids.Remove(boid);
            numBoids--;
        }
    }

    public int GetNumberOfBoids()
    {
        return collectedBoids.Count;
    }

    public BoidObject[] CallBoids()
    {
        return CallBoids(GetNumberOfBoids());
    }

    public BoidObject[] CallBoids(int numToPull)
    {
        BoidObject[] boidsToPull = new BoidObject[numToPull];

        for (int i = 0; i < numToPull; i++)
        {
            boidsToPull[i] = collectedBoids[i];
            numBoids--;
        }
        for (int i = 0; i < numToPull; i++)
        {
            collectedBoids.RemoveAt(0);
        }

        return boidsToPull;
    }
}
