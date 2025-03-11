using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStalking : MonoBehaviour
{
    public Transform centerObject;
    public float orbitRadius = 20f;
    public float rotationSpeed = 1f;
    public float yOffset = 0f; // Optional: Add an offset for the y position

    void Update()
    {
        if (centerObject == null) return;

        // Calculate the angle based on time and rotation speed
        float angle = Time.time * rotationSpeed;

        // Calculate the new position using trigonometry
        float x = centerObject.position.x + Mathf.Cos(angle) * orbitRadius;
        float z = centerObject.position.z + Mathf.Sin(angle) * orbitRadius;

        // Set the new position
        transform.position = new Vector3(x, centerObject.position.y + yOffset, z);
    }
}
