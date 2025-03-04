using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Manages and calls update on all boids in scene
 * 
 * Jeff Stevenson
 * 3.4.25
 */

public class BoidManager : MonoBehaviour
{
    [SerializeField]
    private BoidData boidData;

    private BoidObject[] boidsInScene;

    void Start()
    {
        boidsInScene = FindObjectsOfType<BoidObject>();

        foreach (BoidObject boid in boidsInScene)
        {
            if (boid.data == null)
            {
                boid.data = boidData;
            }
        }
    }

    void Update()
    {
        foreach (BoidObject boid in boidsInScene)
        {
            boid.UpdateBoid();

            // for testing
        }
    }
}
