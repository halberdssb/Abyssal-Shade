using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

/*
 * Manages and calls update on all boids in scene
 * 
 * Jeff Stevenson
 * 3.4.25
 */

public class BoidManager : MonoBehaviour
{
    // thread group size for compute shader - same as in BoidComputeShader
    private const int THREAD_GROUP_SIZE = 1024; 

    [SerializeField]
    private BoidData boidData;

    [SerializeField]
    private ComputeShader boidComputeShader;

    private BoidObject[] boidsInScene;

    // Boid Compute Data Struct - holds data for boids calculated in compute shader
    public struct BoidComputeData
    {
        public Vector3 boidPosition;
        public Vector3 boidDirection;

        public Vector3 neighborsDirection;
        public Vector3 neighborsCenter;
        public Vector3 neighborsSeparationForce;
        public int numNeighbors;

        // used for compute buffer size
        public static int GetSize()
        {
            // total size of all variables (vector3 is 3 floats)
            return sizeof(float) * 3 * 5 + sizeof(int);
        }
    }

    void Start()
    {
        boidsInScene = FindObjectsOfType<BoidObject>();

        foreach (BoidObject boid in boidsInScene)
        {
            if (boid.data == null)
            {
                boid.BoidStart(boidData);
            }
        }
    }

    void Update()
    {
        int totalNumBoids = boidsInScene.Length;

        // add transform values from all boids into boid data array
        BoidComputeData[] boidComputeData = new BoidComputeData[totalNumBoids];
        
        for (int i = 0; i < totalNumBoids; i++)
        {
            boidComputeData[i].boidPosition = boidsInScene[i].transform.position;
            boidComputeData[i].boidDirection = boidsInScene[i].transform.forward;
        }

        // create compute buffer to calculate boid neighbor data
        ComputeBuffer boidComputeBuffer = new ComputeBuffer(totalNumBoids, BoidComputeData.GetSize());
        boidComputeBuffer.SetData(boidComputeData);

        // name here must match RWStructuredBuffer name in BoidComputeShader
        boidComputeShader.SetBuffer(0, "boids", boidComputeBuffer);
        boidComputeShader.SetInt("numBoids", totalNumBoids);
        boidComputeShader.SetFloat("viewRadius", boidData.neighborCastRadius);
        boidComputeShader.SetFloat("avoidRadius", boidData.neighborAvoidRadius);

        // get number of groups needed for compute shader based on num boids - can take THREAD_GROUP_SIZE boids at a time
        int numThreadGroups = Mathf.CeilToInt(totalNumBoids / (float) THREAD_GROUP_SIZE);

        // send off and receive data from compute shader
        boidComputeShader.Dispatch(0, numThreadGroups, 1, 1);

        boidComputeBuffer.GetData(boidComputeData);

        // send data form compute shader to each boid and update
        for (int i = 0; i < totalNumBoids; i++)
        {
            BoidObject boid = boidsInScene[i];

            boid.neighborsDirection = boidComputeData[i].neighborsDirection;
            boid.neighborsCenter = boidComputeData[i].neighborsCenter;
            boid.neighborsSeparationForce = boidComputeData[i].neighborsSeparationForce;
            boid.numNeighborBoids = boidComputeData[i].numNeighbors;

            boid.UpdateBoid();
        }

        boidComputeBuffer.Release();
    }
}
