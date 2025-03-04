using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Spawns boids on start for testing
 * 
 * Jeff Stevenson
 * 3.4.25
 */

public class BoidSpawner : MonoBehaviour
{
    [SerializeField]
    private int numberBoidsToSpawn;

    [SerializeField]
    private GameObject boidPrefab;

    private BoxCollider spawnArea;

    // Start is called before the first frame update
    void Awake()
    {
        spawnArea = GetComponent<BoxCollider>();

        SpawnBoids();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // spawns specified number of boids in spawn area
    private void SpawnBoids()
    {
        Transform boidParent = new GameObject("BoidHolder").transform;

        for (int i = 0; i < numberBoidsToSpawn; i++)
        {
            Vector3 spawnPos = spawnArea.transform.position;
            spawnPos.x += Random.Range(-spawnArea.size.x * 0.5f, spawnArea.size.x * 0.5f);
            spawnPos.y += Random.Range(-spawnArea.size.y * 0.5f, spawnArea.size.y * 0.5f);
            spawnPos.z += Random.Range(-spawnArea.size.z * 0.5f, spawnArea.size.z * 0.5f);

            Instantiate(boidPrefab, spawnPos, Random.rotation);
        }
    }
}
