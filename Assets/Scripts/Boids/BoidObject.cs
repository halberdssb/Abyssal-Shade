
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

/*
 * Handles an individual boid object and its movement
 * including separation, alignment, and cohesion calculations
 * 
 * Jeff Stevenson
 * 3.2.25
 * modifications lines 39,40,71,72,73 from Dev 4.23.25
 */


public class BoidObject : MonoBehaviour
{
    [HideInInspector]
    public BoidData data;
    [HideInInspector]
    public PlayerStateController player;

    [HideInInspector]
    public Vector3 neighborsDirection;
    [HideInInspector]
    public Vector3 neighborsCenter;
    [HideInInspector]
    public Vector3 neighborsSeparationForce;
    [HideInInspector]
    public int numNeighborBoids;

    [SerializeField]
    private bool showNavigationRays;

    private Vector3[] collisionNavigationCheckVectors;
    private Vector3 velocity;

    private GameObject followObj;

    private bool isUsingBoidBehavior;

    private Stack<Vector3> externalForces = new Stack<Vector3>();

    //from Dev for attack
    public bool isAttacking = false;

    public BoidObject(BoidData boidData)
    {
        this.data = boidData;
    }

    // called when a boid is found at start of scene by BoidManager
    public void BoidStart(BoidData data, PlayerStateController player, GameObject followObj = null)
    {
        this.data = data;
        this.player = player;

        velocity = transform.forward * data.maxSpeed / 2;
        transform.rotation = Random.rotation;
        externalForces = new Stack<Vector3>();

        collisionNavigationCheckVectors = NavigationSphereCaster.GetNavigationSphereVectors(data.collisionNavigationChecks);

        isUsingBoidBehavior = true;

        if (followObj)
        {
            this.followObj = followObj;
        }
    }

    // gets separation, alignment, and cohesion values and adds them to move boid
    // called by BoidManager
    public void UpdateBoid()
    {
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        if (distanceToPlayer > BoidManager.BOID_DESPAWN_DISTANCE && !followObj)
        {
            BoidManager.DespawnBoids(this);
            return;
        }

        Vector3 acceleration = Vector3.zero; //transform.rotation * Vector3.forward * data.moveSpeed;

        //from Dev, to make the attack work by pausing movement controller temporarily
        if (isAttacking)
            return;

        if (followObj)
        {
            acceleration = (followObj.transform.position - transform.position).normalized * data.followObjInfluence;
        }
        else if ((player.transform.position - transform.position).sqrMagnitude < PlayerStateController.BoidCollectionDistance * PlayerStateController.BoidCollectionDistance)
        {
            followObj = player.gameObject;
            player.boidCollectionHandler.AddCollectedBoid(this);
        }

        // boid rules - alignment/cohesion/separation
        if (numNeighborBoids > 0)
        {
            neighborsCenter /= numNeighborBoids;
            Vector3 vectorToNeighborsCenter = neighborsCenter - transform.position;

            Vector3 separation = GetWeightedClampedForce(neighborsSeparationForce, data.separationInfluence);
            Vector3 alignment = GetWeightedClampedForce(neighborsDirection, data.alignmentInfluence);
            Vector3 cohesion = GetWeightedClampedForce(vectorToNeighborsCenter, data.cohesionInfluence);

            acceleration += separation + alignment + cohesion;
        }

        // check for obstacles and avoid if so
        if (CheckForApproachingCollision())
        {
            Vector3 collisionAvoidForce = CalculateMovementAroundCollisions();
            collisionAvoidForce = GetWeightedClampedForce(collisionAvoidForce, data.collisionAvoidInfluence);

            acceleration += collisionAvoidForce;
        }

        // apply forces to boid and move
        velocity += acceleration * Time.deltaTime;
        velocity += AddAllExternalForces();
        velocity = Vector3.ClampMagnitude(velocity, data.maxSpeed);

        transform.position += velocity * Time.deltaTime;
        transform.forward = velocity;
    }

    // toggles if the boid should be moving according to boid rules or not - should be disabled for extended external forces/movement
    public void ToggleBoidBehavior(bool useBoidBehavior)
    {
        isUsingBoidBehavior = useBoidBehavior;
    }

    public bool IsUsingBoidBehavior()
    {
        return isUsingBoidBehavior;
    }

    // sets the follow object of the boid
    public void SetFollowObject(GameObject objectToFollow)
    {
        followObj = objectToFollow;
    }

    // checks for obstacle collision in front of boid
    private bool CheckForApproachingCollision()
    {
        Ray collisionCheckRay = new Ray(transform.position, transform.forward);

        return Physics.SphereCast(collisionCheckRay, data.collisionViewRadius, data.collisionViewDistance, data.collisionMask);
    }

    // finds path of least resistance to avoid approaching collisions
    private Vector3 CalculateMovementAroundCollisions()
    {
        foreach (Vector3 collisionCheckVector in collisionNavigationCheckVectors)
        {
            // check that ray is within fov of boid
            if (Vector3.Dot(transform.forward, collisionCheckVector) > data.fovCutoff)
            {
                // check in set world space direction independent from boid direction
                Vector3 worldSpaceCheckVector = transform.TransformDirection(collisionCheckVector);

                Ray collisionCheckRay = new Ray(transform.position, worldSpaceCheckVector);
                if (!Physics.SphereCast(collisionCheckRay, data.collisionViewRadius, data.collisionViewDistance, data.collisionMask))
                {
                    return worldSpaceCheckVector;
                }
            }
        }

        // if no valid way out found, do a 180
        return -transform.forward;
    }

    // returns a movement force for the boid clamped to max turn speed
    private Vector3 GetWeightedClampedForce(Vector3 force, float weight)
    {
        Vector3 forceToAdd = force.normalized * data.maxSpeed - velocity;

        return Vector3.ClampMagnitude(forceToAdd, data.maxTurnSpeed) * weight;
    }

    // pops and returns all added forces to add to fish this frame
    private Vector3 AddAllExternalForces()
    {
        if (externalForces.Count > 0)
        {
            Vector3 netExternalForce = Vector3.zero;

            int numForcesToAdd = externalForces.Count;
            for (int i = 0; i < numForcesToAdd; i++)
            {
                netExternalForce += externalForces.Pop();
            }

            return netExternalForce;
        }

        return Vector3.zero;
    }

    // adds an external force from outside source to force stack
    public void ApplyForce(Vector3 force)
    {
        externalForces.Push(force);
    }

    // shows navigation rays if toggled on
    private void OnDrawGizmosSelected()
    {
        if (showNavigationRays)
        {
            Vector3[] rays = NavigationSphereCaster.GetNavigationSphereVectors(200);

            foreach (Vector3 ray in rays)
            {
                if (Vector3.Dot(transform.forward, ray) > -0.5)
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.red;
                }

                Gizmos.DrawLine(transform.position, transform.position + ray.normalized);
            }
        }
    }
}
