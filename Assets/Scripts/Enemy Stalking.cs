using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyStalking : MonoBehaviour
{
    //Reference to the player the serpent will track
    public Transform player;

    //Distance within which the serpent starts circling the player
    public float radius = 5f;

    //Speed at which the serpent circles the player
    public float circleSpeed = 10f;

    //Distance beyond which the serpent stops stalking the player
    public float stopDistance = 6f;

    //SmoothDamp interpolation time for smooth circling movement
    public float smoothTime = 0.3f;

    //Speed of the lunge movement toward the player
    public float lungeSpeed = 20f;

    //Time between consecutive lunges
    public float lungeCooldown = 5f;

    //Duration of the lunge movement
    public float lungeDuration = 0.5f;

    //Minimum amount of time the serpent must stalk the player before it can lunge
    public float minStalkBeforeLunge = 2f;

    //Whether the serpent is currently circling the player
    private bool isCircling = false;

    //Whether the serpent is currently performing a lunge
    private bool isLunging = false;

    //Flag to ensure the circling logic has been initialized before lunging
    private bool hasInitializedCircle = false;

    //Timestamp of when stalking began
    private float stalkStartTime = -Mathf.Infinity;

    //Timestamp of the last lunge to handle cooldown
    private float lastLungeTime = -Mathf.Infinity;

    //Used for SmoothDamp movement calculation
    private Vector3 currentVelocity;

    //The last known circling position to return to after a lunge
    private Vector3 circlingReturnPosition;

    private void Start()
    {
        //Initialize circling the player when called
        CircleAroundPlayer();
    }

    // Called once per frame to handle stalking, lunging, and transitions
    void Update()
    {
        CircleAroundPlayer();
        return;

        //Measure the distance between serpent and player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        //Begin stalking if within radius and not already circling or lunging
        if (distanceToPlayer <= radius && !isCircling && !isLunging)
        {
            isCircling = true;
            //Begins timer for stalking, used for lunging logic
            stalkStartTime = Time.time;
        }

        // Check if enemy can lunge
        bool canLunge = isCircling
            && !isLunging
            && hasInitializedCircle
            && Time.time >= stalkStartTime + minStalkBeforeLunge
            && Time.time >= lastLungeTime + lungeCooldown;

        // Perform circling motion if not lunging
        if (isCircling && !isLunging)
        {
            CircleAroundPlayer();
        }
    }

    //Handles circling motion around the player in a smooth circular path
    void CircleAroundPlayer()
    {
        float angle = Time.time * circleSpeed;

        //Calculate position offset in a circular pattern
        float offsetX = Mathf.Cos(angle) * radius;
        float offsetZ = Mathf.Sin(angle) * radius;

        //Target position around the player
        Vector3 targetPosition = new Vector3(player.position.x + offsetX, player.position.y, player.position.z + offsetZ);
        Vector3 swimSlitherOffset = Mathf.Sin(Time.time / 2) * (targetPosition - player.position) * 0.2f;
        

        //Save the target as the return location after a lunge
        circlingReturnPosition = targetPosition;

        //Mark that circling is now initialized
        hasInitializedCircle = true;

        //Smoothly move toward the circling position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition + swimSlitherOffset, ref currentVelocity, smoothTime);
        Debug.DrawLine(transform.position, targetPosition);

        //Rotate to face circling position with offset for model orientation
        RotateHeadTowards(targetPosition + swimSlitherOffset);
    }

    //Lunges towards the player in a straight line before returning to circling
    IEnumerator LungeAtPlayer()
    {
        //Sets lunging state
        isLunging = true;

        //Records time of lunge for the cooldown
        lastLungeTime = Time.time;

        //Gets current position to return to after lunge
        Vector3 startPosition = transform.position;

        //Player's position is the target for the lunge
        Vector3 lungeTarget = new Vector3(player.position.x, player.position.y, player.position.z);
        RotateHeadTowards(lungeTarget);

        //Elapsed time for the lunge movement
        float elapsed = 0f;

        //Move toward the player using lerp for the lunge
        while (elapsed < lungeDuration)
        {
            transform.position = Vector3.Lerp(startPosition, lungeTarget, elapsed / lungeDuration);
            elapsed += Time.deltaTime * (lungeSpeed / 10f);
            yield return null;
        }

        //Snaps to exact target position at the end of the lunge
        transform.position = lungeTarget;

        //Short delay before returning to circling
        yield return new WaitForSeconds(0.1f);

        //Smoothly return to circling position
        elapsed = 0f;
        Vector3 retreatStart = transform.position;

        //Lerp back to the circling return position
        while (elapsed < lungeDuration)
        {
            transform.position = Vector3.Lerp(retreatStart, circlingReturnPosition, elapsed / lungeDuration);
            elapsed += Time.deltaTime * (lungeSpeed / 10f);
            yield return null;
        }

        //Snap to exact return point
        transform.position = circlingReturnPosition;
        isLunging = false;
    }

    //Rotates serpent accounting for rotation offset of model
    private void RotateHeadTowards(Vector3 target)
    {
        transform.LookAt(target);
        transform.eulerAngles += new Vector3(0, 0, -90);
        transform.eulerAngles += new Vector3(0, 90, 0);
    }
}
