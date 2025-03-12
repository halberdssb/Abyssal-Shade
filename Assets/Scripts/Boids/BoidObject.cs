using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

/*
 * Handles an individual boid object and its movement
 * including separation, alignment, and cohesion calculations
 * 
 * Jeff Stevenson
 * 3.2.25
 */


public class BoidObject : MonoBehaviour
{
    [HideInInspector]
    public BoidData data;

    [HideInInspector]
    public Vector3 neighborsDirection;
    [HideInInspector]
    public Vector3 neighborsCenter;
    [HideInInspector]
    public Vector3 neighborsSeparationForce;
    [HideInInspector]
    public int numNeighborBoids;

    private List<Transform> neighborBoids;

    private Vector3 velocity;

    public BoidObject(BoidData boidData)
    {
        this.data = boidData;
    }

    // called when a boid is found at start of scene by BoidManager
    public void BoidStart(BoidData data)
    {
        //neighborBoids = new List<Transform>();

        this.data = data;

        velocity = transform.forward * data.moveSpeed;
        transform.rotation = Random.rotation;
    }

    // gets separation, alignment, and cohesion values and adds them to move boid
    // called by BoidManager
    public void UpdateBoid()
    {
        Vector3 acceleration = Vector3.zero; //transform.rotation * Vector3.forward * data.moveSpeed;

        if (numNeighborBoids > 0)
        {
            neighborsCenter /= numNeighborBoids;

            Vector3 separation = neighborsSeparationForce * data.separationInfluence;
            Vector3 alignment = neighborsDirection * data.alignmentInfluence;
            Vector3 cohesion = neighborsCenter * data.cohesionInfluence;


            acceleration += separation + alignment + cohesion;
        }

        velocity += acceleration / Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, data.maxSpeed);

        transform.position += velocity * Time.deltaTime;
        transform.forward = velocity.normalized;
    }

    // spherecasts for nearby boid neighbors and returns array of neighbors in vision
    private void FindNeighborBoids()
    {
        RaycastHit[] hits;
        hits = Physics.SphereCastAll(transform.position, data.neighborCastRadius, Vector3.forward, data.neighborCastRadius, data.neighborCastMask);

        for (int i = 0; i < hits.Length; i++)
        {
            // check if boid is self
            if (hits[i].collider.gameObject != this.gameObject)
            {
                // check if boid is in field of view
                float dotProductOfNeighborBoid = Vector3.Dot(transform.forward, transform.position - hits[i].point);
                if (dotProductOfNeighborBoid < data.fovCutoff)
                {
                    // if out of fov, remove from list
                    neighborBoids.Add(hits[i].transform);
                }
            }
        }

        //return neighborBoids.ToArray();
    }

    // calculates repelling separation force from other neighbor boids
    private Vector3 CalculateSeparation()
    {
        if (neighborBoids.Count == 0) return Vector3.zero;

        Vector3 separationForce = Vector3.zero;

        // add forces away from all neighbors - stronger the closer the boid is
        foreach (Transform neighborBoid in neighborBoids)
        {
            Vector3 forceFromNeighbor = transform.position - neighborBoid.position;
            separationForce += forceFromNeighbor / forceFromNeighbor.sqrMagnitude;
        }

        separationForce = separationForce.normalized * data.separationInfluence;

        return separationForce;
    }

    // calculates average direction force of neighbors to help align boids to same direction
    private Vector3 CalculateAlignment()
    {
        if (neighborBoids.Count == 0) return Vector3.zero;

        Vector3 alignmentForce = Vector3.zero;

        // combine forward directions from all neighbors
        foreach (Transform neighborBoid in neighborBoids)
        {
            alignmentForce += neighborBoid.forward;
        }

        alignmentForce /= neighborBoids.Count;
        alignmentForce = alignmentForce.normalized * data.alignmentInfluence;

        return alignmentForce;
    }

    // calculates center of neighbor boids to help form boids into flocks/schoo;s
    private Vector3 CalculateCohesion()
    {
        if (neighborBoids.Count == 0) return Vector3.zero;

        Vector3 cohesionForce = Vector3.zero;

        foreach (Transform neighborBoid in neighborBoids)
        {
            cohesionForce += neighborBoid.position;
        }

        cohesionForce /= neighborBoids.Count;
        cohesionForce = cohesionForce.normalized * data.cohesionInfluence;

        return cohesionForce;
    }
}
