using System.Collections;
using System.Collections.Generic;
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

    public BoidObject(BoidData boidData)
    {
        this.data = boidData;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // gets separation, alignment, and cohesion values and adds them to move boid
    // called by BoidManager
    public void UpdateBoid()
    {
        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;

        Transform[] neighorBoids = FindNeighborBoids();


    }

    // spherecasts for nearby boid neighbors and returns array of neighbors in vision
    private Transform[] FindNeighborBoids()
    {
        RaycastHit[] hits;
        hits = Physics.SphereCastAll(transform.position, data.neighborCastRadius, Vector3.forward, data.neighborCastRadius, data.neighborCastMask);

        List<Transform> neighborBoidTransforms = new List<Transform>();

        for (int i = 0; i < hits.Length; i++)
        {
            // check if boid is in field of view
            float dotProductOfNeighborBoid = Vector3.Dot(transform.forward, transform.position - hits[i].point);
            if (dotProductOfNeighborBoid < data.fovCutoff)
            {
                // if out of fov, remove from list
                neighborBoidTransforms.Add(hits[i].transform);
            }
        }

        return neighborBoidTransforms.ToArray();
    }

    private void CalculateSeparation()
    {

    }

    private void CalculateAlignment()
    {

    }

    private void CalculateCohesion()
    {

    }
}
