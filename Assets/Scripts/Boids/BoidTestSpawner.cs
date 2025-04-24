using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Spawns boids on start for testing
 * 
 * Jeff Stevenson
 * 3.4.25
 */

public class BoidTestSpawner : MonoBehaviour
{
    [SerializeField]
    private int numberBoidsToSpawn;

    [SerializeField]
    private GameObject boidPrefab;

    [Space]
    [SerializeField]
    private bool clampToSpawnArea;
    [SerializeField]
    private bool showSpawnBounds;

    [SerializeField]
    private BoxCollider spawnArea;

    private GameObject[] boids;

    // Start is called before the first frame update
    void Start()
    {
        spawnArea = GetComponent<BoxCollider>();

        SpawnBoids();
    }

    // Update is called once per frame
    void Update()
    {
        if (clampToSpawnArea)
        {
            foreach (var boid in boids)
            {
                if (Mathf.Abs(boid.transform.position.x) > spawnArea.transform.position.x + spawnArea.size.x / 2)
                {
                    boid.transform.position += new Vector3(spawnArea.size.x, 0, 0) * -Mathf.Sign(boid.transform.position.x);
                }
                if (Mathf.Abs(boid.transform.position.y) > spawnArea.transform.position.y + spawnArea.size.y / 2)
                {
                    boid.transform.position += new Vector3(0, spawnArea.size.y, 0) * -Mathf.Sign(boid.transform.position.y);
                }
                if (Mathf.Abs(boid.transform.position.z) > spawnArea.transform.position.z + spawnArea.size.z / 2)
                {
                    boid.transform.position += new Vector3(0, 0, spawnArea.size.z) * -Mathf.Sign(boid.transform.position.z);
                }
            }
        }
    }

    // spawns specified number of boids in spawn area
    private void SpawnBoids()
    {
        Transform boidParent = new GameObject("BoidHolder").transform;

        for (int i = 0; i < numberBoidsToSpawn / 5; i++)
        {
            Vector3 spawnPos = spawnArea.transform.position;
            spawnPos.x += Random.Range(-spawnArea.size.x * 0.5f, spawnArea.size.x * 0.5f);
            spawnPos.y += Random.Range(-spawnArea.size.y * 0.5f, spawnArea.size.y * 0.5f);
            spawnPos.z += Random.Range(-spawnArea.size.z * 0.5f, spawnArea.size.z * 0.5f);

            BoidManager.SpawnBoids(5, spawnPos);
        }
    }

    private void OnDrawGizmos()
    {
        if (showSpawnBounds)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawCube(spawnArea.center, spawnArea.size);
        }
    }
}
