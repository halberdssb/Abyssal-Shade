using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidCollectionHandler : MonoBehaviour
{
    public Stack<BoidObject> collectedBoids;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddCollectedBoid(BoidObject boid)
    {
        collectedBoids.Push(boid);
    }

    public BoidObject[] GetCollectedBoids(int numToPull)
    {
        BoidObject[] boidsToPull = new BoidObject[numToPull];

        for (int i = 0; i < numToPull; i++)
        {
            boidsToPull[i] = collectedBoids.Pop();
        }

        return boidsToPull;
    }
}
