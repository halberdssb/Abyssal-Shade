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
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("boid hit!");
        BoidManager.DespawnBoids(other.GetComponent<BoidObject>());
    }
}
