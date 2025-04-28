using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Spawns soulfish in specified area at certain times/intervals
 * 
 * Jeff Stevenson
 * 4.22.25
 */

[RequireComponent(typeof(SphereCollider))]
public class SoulfishSpawner : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Number of soulfish spawned in one wave")]
    private int numSoulfishPerWave;
    [SerializeField]
    [Tooltip("Number of waves to spawn - 0 = will spawn until global maximum is hit")]
    private int numWavesToSpawn;
    [SerializeField]
    [Tooltip("True = fish will spawn passively while in load distance, false = will wait until player is within trigger or external action spawns fish")]
    private bool spawnPassively;
    [SerializeField]
    [Tooltip("True = fish will spawn randomly throughout space in collider, false = fish will spawn at designated spawn point")]
    private bool spawnRandomlyInCollider;
    [SerializeField]
    [Tooltip("Only used if spawnRandomlyInCollider = false")]
    private Vector3 spawnPoint;
    [SerializeField]
    [Tooltip("Time between spawning waves - 0 = fish will all spawn at once rather than in waves")]
    private float timeBetweenWaves;

    // time between individual fish spawns in a wave
    private float timeBetweenIndividualSpawns = 0.25f;
    // tracks if the spawner is actively spawning fish or not
    private bool isActivated;

    // timer to stagger fish spawns in ONE wave
    private float fishSpawnTimer;
    // timer to stagger waves between each other
    private float waveSpawnTimer;
    // tracks how many fish in a wave have been spawned
    private int numFishSpawnedThisWave;

    // collider used for spawning fish and/or triggering spawns
    private SphereCollider spawnCollider;

    // player reference
    PlayerStateController player;

    void Start()
    {
        spawnCollider = GetComponent<SphereCollider>();
        spawnCollider.isTrigger = true;

        player = FindObjectOfType<PlayerStateController>();

        if (spawnPassively)
        {
            Invoke("CheckPlayerDistance", 1f);
        }
    }

    private void Update()
    {
        // handle spawn timers/spawning if active
        if (isActivated)
        {
            HandleFishAndWaveTiming();
        }

        // check if spawner should be loaded/unloaded if set to always spawn
        if (!spawnPassively)
        {
            CheckPlayerDistance();
        }
    }

    // handles timing for spawning fish and waves in update
    private void HandleFishAndWaveTiming()
    {
        // if time between waves is up, start spawning fish in a wave
        if (waveSpawnTimer > timeBetweenWaves)
        {
            // check if individual fish should be spawned
            if (fishSpawnTimer > timeBetweenIndividualSpawns)
            {
                Vector3 spawnPos = spawnRandomlyInCollider ? GetRandomizedSpawnPosition() : transform.position + spawnPoint;
                BoidManager.SpawnBoids(1, spawnPos);

                numFishSpawnedThisWave++;
                fishSpawnTimer = 0;
            }

            if (numFishSpawnedThisWave >= numSoulfishPerWave)
            {
                waveSpawnTimer = 0;
                numFishSpawnedThisWave = 0;
            }
            else
            {
                fishSpawnTimer += Time.deltaTime;
            }
        }
        // else increment wave spawn timer til 
        else
        {
            waveSpawnTimer += Time.deltaTime;
        }
    }

    // checks distance to player and activates/deactivates based on distance
    private void CheckPlayerDistance()
    {
        float playerDistance = Vector3.Distance(player.transform.position, transform.position);

        if (isActivated && playerDistance > BoidManager.BOID_DESPAWN_DISTANCE)
        {
            ActivateSpawner(false);
        }
        else if (!isActivated && playerDistance < BoidManager.BOID_DESPAWN_DISTANCE)
        {
            ActivateSpawner(true);
        }
    }

    // enables/disables spawn routines
    public void ActivateSpawner(bool activate)
    {
        isActivated = activate;
        fishSpawnTimer = 0f;
        waveSpawnTimer = 0f;
    }

    // gets a randomized position within the spawn collider area
    private Vector3 GetRandomizedSpawnPosition()
    {
        Vector3 spawnPos = spawnCollider.transform.position;
        spawnPos.x += GetRandomPointOnSphereAxis();
        spawnPos.y += GetRandomPointOnSphereAxis();
        spawnPos.z += GetRandomPointOnSphereAxis();

        return spawnPos;
    }

    // gets a random float point on one axes of the sphere spawn area
    private float GetRandomPointOnSphereAxis()
    {
        return Random.Range(-spawnCollider.radius * 0.5f, spawnCollider.radius * 0.5f);
    }

    // if not set to start on awake, activate when player enters trigger area
    private void OnTriggerEnter(Collider other)
    {

    }

    // draw sphere on spawn point
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;

        Gizmos.DrawSphere(transform.position + spawnPoint, 0.5f);
    }
}
