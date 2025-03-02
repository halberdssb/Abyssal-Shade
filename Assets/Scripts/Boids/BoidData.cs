using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Holds data for boid objects
 * 
 * Jeff Stevenson
 * 3.2.25
 */

[CreateAssetMenu(menuName = "Data")]
public class BoidData : ScriptableObject
{
    [Range(0f, 100f)]
    public float moveSpeed;
    [Range(0f, 100f)]
    public float turnSpeed;
    [Range(0f, 100f)]
    public float separationInfluence;
    [Range(0f, 100f)]
    public float alignmentInfluence;
    [Range(0f, 100f)]
    public float cohesionInfluence;

    [Space]
    public float neighborCastRadius;
    public LayerMask neighborCastMask;
    [Range(-1f, 1f)]
    public float fovCutoff;
}
