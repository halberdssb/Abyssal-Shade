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
public class WaterCurrent : MonoBehaviour
{
    private Vector3 currentDirection = Vector3.forward; // Direction of the current
    private float currentStrength = .05f; // Strength of the force
    private ForceMode forceMode = ForceMode.VelocityChange; // Type of force applied
    public float changeInterval = 3f; // Time in seconds before changing direction

  //  public ParticleSystem currentParticles; // Reference to the particle system

    private HashSet<Rigidbody> affectedBodies = new HashSet<Rigidbody>();
    
    private void Start()
    {
        StartCoroutine(ChangeCurrentDirection());
    }

    private void FixedUpdate()
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
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            affectedBodies.Add(rb);
            Debug.Log($"{rb.name} entered the current!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            affectedBodies.Remove(rb);
        }
    }
}
