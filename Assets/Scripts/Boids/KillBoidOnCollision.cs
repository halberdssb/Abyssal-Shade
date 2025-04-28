using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Despawns boids on collision - used for anglerfish maw
 * 
 * Jeff Stevenson
 * 4.26.25
 */

public class KillBoidOnCollision : MonoBehaviour
{
    private BoidCollectionHandler boidCollectionHandler;

    private void Start()
    {
        boidCollectionHandler = FindObjectOfType<BoidCollectionHandler>();
    }
    private void OnTriggerEnter(Collider other)
    {
        boidCollectionHandler.PlayDeathSound();
        BoidManager.DespawnBoids(other.GetComponent<BoidObject>());
    }
}
