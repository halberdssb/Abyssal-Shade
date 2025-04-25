using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Contains methods that when placed on a Box Collider, any object that enters said collider will have its rigidbody acted upon to simulate a current system
 * 
 * Devraj Singh
 * 3.10.25
 */

[RequireComponent(typeof(Collider))]
public class Current : MonoBehaviour
{
    [SerializeField]
    private Vector3 currentDirection = Vector3.forward; // Direction of the current
    [SerializeField]
    private float currentStrength = .05f; // Strength of the force
    [SerializeField]
    private bool startOn;

    [Space]
    [SerializeField]
    private bool useTimer;
    [SerializeField]
    private float onTime;
    [SerializeField]
    private float offTime;

    [Space]
    [SerializeField]
    private bool changeDirectionOnInterval;
    [SerializeField]
    private float changeInterval = 3f; // Time in seconds before changing direction

    private ForceMode forceMode = ForceMode.VelocityChange; // Type of force applied
    //  public ParticleSystem currentParticles; // Reference to the particle system

    private HashSet<Rigidbody> affectedBodies = new HashSet<Rigidbody>();
    private HashSet<BoidObject> affectedBoids = new HashSet<BoidObject>();

    private bool isActivated;
    private bool isCycleOn;
    private float cycleTimer;
    
    private void Start()
    {
        if (changeDirectionOnInterval) StartCoroutine(ChangeCurrentDirection());
        if (startOn) ToggleCurrent(true);
    }

    private void Update()
    {
        if (useTimer)
        {
            HandleCycleOnOff();
        }
    }

    private void FixedUpdate()
    {
        if (isActivated)
        {
            // Continuously apply force to all objects in the current
            foreach (Rigidbody rb in affectedBodies)
            {
                if (rb != null)
                {
                    //Debug.Log($"Applying force to {rb.name}");
                    rb.AddForce(currentDirection.normalized * currentStrength, forceMode);
                }
            }

            foreach (BoidObject boid in affectedBoids)
            {
                boid.ApplyForce(currentDirection.normalized * currentStrength);
            }
        }
    }

    // enables/disables the whole current system, including its cycle if using
    public void ToggleCurrent(bool activate)
    {
        isActivated = activate;
        if (useTimer)
        {
            cycleTimer = 0f;
            isCycleOn = activate;
        }
    }

    // switches between on and off for timer cycle while active
    public void HandleCycleOnOff()
    {
        if (isActivated && cycleTimer > onTime)
        {
            isActivated = false;
            cycleTimer = 0f;
        }
        else if (!isActivated && cycleTimer > offTime)
        {
            isActivated = true;
            cycleTimer = 0f;
        }

        cycleTimer += Time.deltaTime;
    }

    private IEnumerator ChangeCurrentDirection()
    {
        while (true)
        {
            Vector3 newDirection = Random.onUnitSphere; // Random unit vector
            float elapsedTime = 0f;
            float transitionTime = 2f; // Time to smoothly transition

            Vector3 initialDirection = currentDirection;

            while (elapsedTime < transitionTime)
            {
                elapsedTime += Time.deltaTime;
                currentDirection = Vector3.Slerp(initialDirection, newDirection, elapsedTime / transitionTime);
  //              UpdateParticleSystem();
                yield return null;
            }

            yield return new WaitForSeconds(changeInterval);
        }
    }

  //  private void UpdateParticleSystem()
  //  {
  //      if (currentParticles != null)
  //      {
  //          // Rotate the particle system to match the new current direction
  //          var currentParticles = currentParticles.main;
  //          main.startRotation3D = new Vector3(0, Mathf.Atan2(currentDirection.x, currentDirection.z) * Mathf.Rad2Deg, 0);
  //
  //          // Adjust the velocity over lifetime direction
  //          var velocityModule = currentParticles.velocityOverLifetime;
  //          velocityModule.enabled = true; // Ensure velocity over lifetime is active
  //
  //          // Set velocity components separately
  //          velocityModule.x = new ParticleSystem.MinMaxCurve(currentDirection.x * currentStrength);
  //          velocityModule.y = new ParticleSystem.MinMaxCurve(currentDirection.y * currentStrength);
  //          velocityModule.z = new ParticleSystem.MinMaxCurve(currentDirection.z * currentStrength);
  //      }
  //  }

    //Function to print what object has entered the Current (in game the entire level will be the current)
    private void OnTriggerEnter(Collider other)
    {
        BoidObject boid = other.GetComponent<BoidObject>();
        if (boid != null)
        {
            //Debug.Log(boid.name + " entered the current!", boid.gameObject);
            affectedBoids.Add(boid);
            return;
        }

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            affectedBodies.Add(rb);
            //Debug.Log($"{rb.name} entered the current!");
            return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        BoidObject boid = other.GetComponent<BoidObject>();
        if (boid != null)
        {
            affectedBoids.Remove(boid);
            return;
        }

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            affectedBodies.Remove(rb);
        }
    }

    // draw an arrow in direction current is pushing
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        // get points for point and base corners of pyramid at top of arrow
        Vector3 topOfPyramid = transform.position + currentDirection.normalized;
        Vector3 yAxisCrossVector = Vector3.Cross(currentDirection.normalized, Vector3.up);
        Vector3 topPyramidBaseCorner = transform.position + (yAxisCrossVector / 2f);
        Vector3 bottomPyramidBaseCorner = transform.position - (yAxisCrossVector / 2f);
        Vector3 xAxisCrossVector = Vector3.Cross(currentDirection.normalized, Vector3.forward);
        Vector3 rightPyramidBaseCorner = transform.position - (xAxisCrossVector / 2f); 
        Vector3 leftPyramidBaseCorner = transform.position + (xAxisCrossVector / 2f); 
        Vector3 bottomOfArrow = transform.position - currentDirection.normalized;

        // draw lines to create 3d arrow shape
        Gizmos.DrawLine(topOfPyramid, topPyramidBaseCorner);
        Gizmos.DrawLine(topOfPyramid, bottomPyramidBaseCorner);
        Gizmos.DrawLine(topOfPyramid, rightPyramidBaseCorner);
        Gizmos.DrawLine(topOfPyramid, leftPyramidBaseCorner);
        Gizmos.DrawLine(topPyramidBaseCorner, leftPyramidBaseCorner);
        Gizmos.DrawLine(leftPyramidBaseCorner, bottomPyramidBaseCorner);
        Gizmos.DrawLine(bottomPyramidBaseCorner, rightPyramidBaseCorner);
        Gizmos.DrawLine(rightPyramidBaseCorner, topPyramidBaseCorner);
        Gizmos.DrawLine(topPyramidBaseCorner, bottomPyramidBaseCorner);
        Gizmos.DrawLine(leftPyramidBaseCorner, rightPyramidBaseCorner);
        Gizmos.DrawLine(topOfPyramid, bottomOfArrow);
    }
}
