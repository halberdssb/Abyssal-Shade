using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * An object that can be restored by a certain number of soulfish
 * Handles pulling soulfish towards and swapping models
 * 
 * Jeff Stevenson
 * 4.21.25 
 */

public class RestorationObject : MonoBehaviour
{
    [SerializeField]
    private int soulfishNeededToRestore;

    private PlayerStateController player;
    private Vector3[] fishSurroundPoints;

    private bool isRestored;

    void Start()
    {
        fishSurroundPoints = NavigationSphereCaster.GetNavigationSphereVectors(soulfishNeededToRestore);
        player = FindObjectOfType<PlayerStateController>();

        isRestored = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isRestored)
        {
            BoidCollectionHandler playerBoids = player.boidCollectionHandler;
            if (playerBoids.GetNumberOfBoids() >= soulfishNeededToRestore)
            {
                Debug.Log("restoring object! enough boids present");
                RestoreObject(playerBoids.CallBoids(soulfishNeededToRestore));
            }
            else Debug.Log("not enough fish in player! num fish player has: " + playerBoids.GetNumberOfBoids());

            isRestored = true;
        }
    }

    // handles putting soulfish into position around object and playing restore anim
    private void RestoreObject(BoidObject[] boids)
    {
        for (int i = 0; i < boids.Length; i++)
        {
            BoidObject boid = boids[i];
            float moveTime = 2f;
            float distanceScalar = 2f;

            boid.ToggleBoidBehavior(false);
            boid.transform.DOMove(transform.position + (fishSurroundPoints[i] * distanceScalar), moveTime);
        }
    }
}
