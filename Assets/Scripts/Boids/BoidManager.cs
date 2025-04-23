using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*
 * Manages and calls update on all boids in scene
 * Also handles spawning and despawning handling
 * 
 * Jeff Stevenson
 * 3.4.25
 */

public class BoidManager : MonoBehaviour
{
    // thread group size for compute shader - MUST BE same as in BoidComputeShader
    private const int THREAD_GROUP_SIZE = 1024;

    private const int MAX_BOIDS_IN_SCENE = 500;

    // distance boids will be simulated/move within related to player
    public readonly static float BOID_DESPAWN_DISTANCE = 100;

    public readonly static int numBoidsInScene;

    [SerializeField]
    private BoidData boidData;

    private static BoidData staticBoidData;

    [SerializeField]
    private GameObject boidPrefab;

    [SerializeField]
    private ComputeShader boidComputeShader;

    //[SerializeField]
    //private GameObject boidFollowObj;

    private static PlayerStateController player;

    private static Queue<BoidObject> inactiveBoidPool = new Queue<BoidObject>();
    private static List<BoidObject> activeBoidPool = new List<BoidObject>();


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

    private void Awake()
    {
        staticBoidData = boidData;
        CreateBoidQueue();
    }

    void Start()
    {
        player = FindObjectOfType<PlayerStateController>();

        foreach (BoidObject boid in activeBoidPool)
        {
            boid.BoidStart(staticBoidData, player);
        }
    }

    void Update()
    {
        int totalNumBoids = activeBoidPool.Count;

        if (totalNumBoids <= 0) return;

        // add transform values from all boids into boid data array
        BoidComputeData[] boidComputeData = new BoidComputeData[totalNumBoids];
        
        for (int i = 0; i < totalNumBoids; i++)
        {
            // check if boid is using boid behavior - otherwise we ignore by sending position far below map
            if (activeBoidPool[i].IsUsingBoidBehavior())
            {
                boidComputeData[i].boidPosition = activeBoidPool[i].transform.position;
                boidComputeData[i].boidDirection = activeBoidPool[i].transform.forward;
            }
            else
            {
                boidComputeData[i].boidPosition = Vector3.down * -1000;
                boidComputeData[i].boidDirection = Vector3.zero;
            }

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
        for (int i = 0; i < activeBoidPool.Count; i++)
        {
            BoidObject boid = activeBoidPool[i];

            boid.neighborsDirection = boidComputeData[i].neighborsDirection;
            boid.neighborsCenter = boidComputeData[i].neighborsCenter;
            boid.neighborsSeparationForce = boidComputeData[i].neighborsSeparationForce;
            boid.numNeighborBoids = boidComputeData[i].numNeighbors;

            if (boid.IsUsingBoidBehavior())
            {
                boid.UpdateBoid();
            }
        }

        boidComputeBuffer.Release();
    }

    // spawns a specified number of boids, adds them to active queue and returns
    public static BoidObject[] SpawnBoids(int requestedNumToSpawn, Vector3 spawnLocation)
    {
        int numBoidsCanBeSpawned = MAX_BOIDS_IN_SCENE - numBoidsInScene;

        if (numBoidsCanBeSpawned == 0)
        {
            Debug.LogWarning("Cannot spawn any more boids - max number in scene!");
            return null;
        }
        else
        {
            // spawn as many boids as we can - check if going to go over capacity if spawning requested amount
            int numToSpawn = requestedNumToSpawn < numBoidsCanBeSpawned ? requestedNumToSpawn : numBoidsCanBeSpawned;

            BoidObject[] boidsToSpawn = new BoidObject[numToSpawn];

            for (int i = 0; i < numToSpawn; i++)
            {
                BoidObject boid = inactiveBoidPool.Dequeue();
                activeBoidPool.Add(boid);
                boid.gameObject.SetActive(true);
                boid.BoidStart(staticBoidData, player);
                boid.transform.position = spawnLocation;
                boid.ToggleBoidBehavior(true);
                boidsToSpawn[i] = boid;
            }

            return boidsToSpawn;
        } 
    }

    // overload to spawn following an object
    public static BoidObject[] SpawnBoids(int requestedNumToSpawn, Vector3 spawnLocation, GameObject follow)
    {
        BoidObject[] boids = SpawnBoids(requestedNumToSpawn, spawnLocation);

        foreach (var boid in boids)
        {
            boid.SetFollowObject(follow);
        }

        return boids;
    }


    // despawns boids, sets them inactive and moves them to the inactive queue
    public static void DespawnBoids(BoidObject[] boidsToDespawn)
    {
        for (int i = 0; i < boidsToDespawn.Length; i++)
        {
            BoidObject boid = boidsToDespawn[i];
            activeBoidPool.Remove(boid);
            inactiveBoidPool.Enqueue(boid);
            boid.ToggleBoidBehavior(false);
            boid.gameObject.SetActive(false);
        }
    }

    public static void DespawnBoids(BoidObject boid)
    {
        BoidObject[] singleBoid = new BoidObject[] { boid };
        DespawnBoids(singleBoid);
    }

    // creates the max number of boids and adds them to inactive stack on scene start
    private void CreateBoidQueue()
    {
        for (int i = 0; i < MAX_BOIDS_IN_SCENE; i++)
        {
            BoidObject boid = Instantiate(boidPrefab, transform).GetComponent<BoidObject>();
            inactiveBoidPool.Enqueue(boid);
            boid.gameObject.SetActive(false);
        }
    }
}