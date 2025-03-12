using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Holds data for boid objects
 * 
 * Jeff Stevenson
 * 3.2.25
 */

[CreateAssetMenu(menuName = "Data/Boid Data")]
public class BoidData : ScriptableObject
{
    [Header("Movement Values")]
    [Range(0f, 100f)]
    public float moveSpeed;
    public float maxSpeed;
    [Range(0f, 100f)]
    public float turnSpeed;

    [Header("Boid Behavior Values")]
    [Space]
    [Range(0f, 100f)]
    public float separationInfluence;
    [Range(0f, 100f)]
    public float alignmentInfluence;
    [Range(0f, 100f)]
    public float cohesionInfluence;
    public float neighborAvoidRadius;

    [Header("Neighbor Detection Values")]
    [Space]
    public float neighborCastRadius;
    public LayerMask neighborCastMask;
    [Range(-1f, 1f)]
    public float fovCutoff;
}
