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
    public float maxSpeed;
    //[Range(1f, 100f)]
    public float maxTurnSpeed;

    [Space]
    [Header("Boid Behavior Values")]
    [Range(1f, 100f)]
    public float separationInfluence;
    [Range(1f, 100f)]
    public float alignmentInfluence;
    [Range(1f, 100f)]
    public float cohesionInfluence;
    [Range(1, 100f)]
    public float collisionAvoidInfluence;
    [Range(1, 100f)]
    public float followObjInfluence;
    [Range(1, 5f)]
    public float followObjFewNeighborsMod;
    public int followObjModNeighborCount;

    [Space]
    [Header("Neighbor Detection Values")]
    public float neighborCastRadius;
    public float neighborAvoidRadius;

    [Space]
    [Header("Obstacle & Collision Values")]
    public LayerMask collisionMask;
    public float collisionViewDistance;
    public float collisionViewRadius;
    public int collisionNavigationChecks;
    [Range(-1f, 1f)]
    public float fovCutoff;

}
