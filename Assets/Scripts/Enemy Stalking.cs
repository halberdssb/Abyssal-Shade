using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStalking : MonoBehaviour
{
    public Transform player;           // Reference to the player
    public float radius = 5f;          // Radius within which the object will start circling
    public float circleSpeed = 10f;    // Speed at which the object circles around the player
    public float stopDistance = 6f;    // Distance at which the object stops circling
    public float smoothTime = 0.3f;    // Time for smooth rotation

    private bool isCircling = false;   // Whether the object is currently circling or not
    private Vector3 currentVelocity;   // Used for smooth damping movement

    void Update()
    {
        // Calculate the distance between the object and the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Start circling when within radius and stop when the player moves farther than stopDistance
        if (distanceToPlayer <= radius && !isCircling)
        {
            isCircling = true;
        }
        else if (distanceToPlayer > stopDistance && isCircling)
        {
            isCircling = false;
        }

        // If the object should circle, perform circular motion
        if (isCircling)
        {
            CircleAroundPlayer();
        }
    }

    void CircleAroundPlayer()
    {
        // Calculate the position offset relative to the player based on a circular motion
        float angle = Time.time * circleSpeed; // Use Time.time for continuous motion

        float offsetX = Mathf.Cos(angle) * radius;
        float offsetZ = Mathf.Sin(angle) * radius;

        // Create the desired position for the object to circle around the player
        Vector3 targetPosition = new Vector3(player.position.x + offsetX, player.position.y, player.position.z + offsetZ);

        // Smoothly move the object towards the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
    }
}
