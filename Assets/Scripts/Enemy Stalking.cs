using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyStalking : MonoBehaviour
{
    // Reference to the player the enemy will track
    public Transform player;

    // Distance within which the enemy starts circling the player
    public float radius = 5f;

    // Speed at which the enemy circles the player
    public float circleSpeed = 10f;

    // Distance beyond which the enemy stops stalking the player
    public float stopDistance = 6f;

    // SmoothDamp interpolation time for smooth circling movement
    public float smoothTime = 0.3f;

    // Speed of the lunge movement toward the player
    public float lungeSpeed = 20f;

    // Time between consecutive lunges
    public float lungeCooldown = 5f;

    // Duration of the lunge movement
    public float lungeDuration = 0.5f;

    // Minimum amount of time the enemy must stalk the player before it can lunge
    public float minStalkBeforeLunge = 2f;

    // Whether the enemy is currently circling the player
    private bool isCircling = false;

    // Whether the enemy is currently performing a lunge
    private bool isLunging = false;

    // Flag to ensure the circling logic has been initialized before lunging
    private bool hasInitializedCircle = false;

    // Timestamp of when stalking began
    private float stalkStartTime = -Mathf.Infinity;

    // Timestamp of the last lunge to handle cooldown
    private float lastLungeTime = -Mathf.Infinity;

    // Used for SmoothDamp movement calculation
    private Vector3 currentVelocity;

    // The last known target circling position to return to after a lunge
    private Vector3 circlingReturnPosition;

    // Called once per frame to handle stalking, lunging, and transitions
    void Update()
    {
        if (player == null) return;

        // Measure the distance between enemy and player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Begin stalking if within radius and not already circling or lunging
        if (distanceToPlayer <= radius && !isCircling && !isLunging)
        {
            isCircling = true;
            stalkStartTime = Time.time; // Record when stalking started
        }
        // Stop stalking and reset timers if the player is too far
        else if (distanceToPlayer > stopDistance && isCircling)
        {
            isCircling = false;
            hasInitializedCircle = false;
            stalkStartTime = -Mathf.Infinity; // Reset stalking time
        }

        // Check if enemy can lunge
        bool canLunge = isCircling
            && !isLunging
            && hasInitializedCircle
            && Time.time >= stalkStartTime + minStalkBeforeLunge
            && Time.time >= lastLungeTime + lungeCooldown;

        if (canLunge)
        {
            StartCoroutine(LungeAtPlayer());
        }

        // Perform circling motion if not lunging
        if (isCircling && !isLunging)
        {
            CircleAroundPlayer();
        }
    }

    // Handles circling motion around the player in a smooth circular path
    void CircleAroundPlayer()
    {
        float angle = Time.time * circleSpeed;

        // Calculate position offset in a circular pattern
        float offsetX = Mathf.Cos(angle) * radius;
        float offsetZ = Mathf.Sin(angle) * radius;

        // Target position around the player
        Vector3 targetPosition = new Vector3(player.position.x + offsetX, player.position.y, player.position.z + offsetZ);
        Vector3 swimSlitherOffset = Mathf.Sin(Time.time / 2) * (targetPosition - player.position) * 0.2f;
        


        // Save the target as the return location after a lunge
        circlingReturnPosition = targetPosition;

        // Mark that circling is now initialized
        hasInitializedCircle = true;

        // Smoothly move toward the circling position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition + swimSlitherOffset, ref currentVelocity, smoothTime);
        Debug.DrawLine(transform.position, targetPosition);
        // offset for model rotation 
        //Quaternion rotation = Quaternion.AngleAxis(-90, Vector3.right);
        RotateHeadTowards(targetPosition + swimSlitherOffset);
    }

    // Coroutine that performs the lunge toward the player and retreats afterward
    IEnumerator LungeAtPlayer()
    {
        isLunging = true;
        lastLungeTime = Time.time;

        Vector3 startPosition = transform.position;
        Vector3 lungeTarget = new Vector3(player.position.x, player.position.y, player.position.z);
        RotateHeadTowards(lungeTarget);

        float elapsed = 0f;

        // Move toward the player using Lerp for the lunge
        while (elapsed < lungeDuration)
        {
            transform.position = Vector3.Lerp(startPosition, lungeTarget, elapsed / lungeDuration);
            elapsed += Time.deltaTime * (lungeSpeed / 10f);
            yield return null;
        }

        transform.position = lungeTarget;

        yield return new WaitForSeconds(0.1f); // Small delay after hit

        // Smoothly return to circling position
        elapsed = 0f;
        Vector3 retreatStart = transform.position;

        while (elapsed < lungeDuration)
        {
            transform.position = Vector3.Lerp(retreatStart, circlingReturnPosition, elapsed / lungeDuration);
            elapsed += Time.deltaTime * (lungeSpeed / 10f);
            yield return null;
        }

        // Snap to exact return point
        transform.position = circlingReturnPosition;
        isLunging = false;
    }

    // rotates serpent accounting for rotation offset of model
    private void RotateHeadTowards(Vector3 target)
    {
        transform.LookAt(target);
        transform.eulerAngles += new Vector3(0, 0, -90);
        transform.eulerAngles += new Vector3(0, 90, 0);
    }
}
