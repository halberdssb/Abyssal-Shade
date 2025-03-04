using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

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

    private List<Transform> neighborBoids;

    private Vector3 velocity;

    public BoidObject(BoidData boidData)
    {
        this.data = boidData;
    }

    // Start is called before the first frame update
    void Start()
    {
        neighborBoids = new List<Transform>();
        Debug.Log(neighborBoids);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // gets separation, alignment, and cohesion values and adds them to move boid
    // called by BoidManager
    public void UpdateBoid()
    {
        FindNeighborBoids();

        Vector3 separation = CalculateSeparation();
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;

        Vector3 acceleration = Vector3.forward * data.moveSpeed;
        acceleration += separation + alignment + cohesion;
        acceleration = Vector3.ClampMagnitude(acceleration, data.maxSpeed);

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
            // check if boid is in field of view
            float dotProductOfNeighborBoid = Vector3.Dot(transform.forward, transform.position - hits[i].point);
            if (dotProductOfNeighborBoid < data.fovCutoff)
            {
                // if out of fov, remove from list
                neighborBoids.Add(hits[i].transform);
            }
        }

        //return neighborBoids.ToArray();
    }

    // add repelling separation force from other neighbor boids
    private Vector3 CalculateSeparation()
    {
        Vector3 separationForce = Vector3.zero;

        // add forces away from all neighbors - stronger the closer the boid is
        foreach (Transform neighborBoid in neighborBoids)
        {
            Vector3 forceFromNeighbor = transform.position - neighborBoid.position;
            float forceStrength = Mathf.InverseLerp(data.neighborCastRadius * data.neighborCastRadius, 0, forceFromNeighbor.sqrMagnitude);
            separationForce += forceFromNeighbor * forceStrength;
        }

        separationForce = separationForce.normalized * data.separationInfluence;

        return separationForce;
    }

    private void CalculateAlignment()
    {

    }

    private void CalculateCohesion()
    {

    }
}
