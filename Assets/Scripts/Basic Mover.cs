using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMover : MonoBehaviour
{
    public float moveSpeed = 5f;      // Normal movement speed
    public float boostMultiplier = 2f; // Speed multiplier when boosting
    public float boostDuration = 1f;   // Duration of the boost
    private float boostTimeRemaining = 0f; // Time remaining for the boost

    void Update()
    {
        // Check for boost input (spacebar)
        if (Input.GetKeyDown(KeyCode.Space) && boostTimeRemaining <= 0f)
        {
            // Start the boost when space is pressed
            boostTimeRemaining = boostDuration;
        }

        // Move the character
        MoveCharacter();
        
        // Handle boost timer
        if (boostTimeRemaining > 0f)
        {
            boostTimeRemaining -= Time.deltaTime; // Decrease boost time over time
        }
    }

    void MoveCharacter()
    {
        // Get input for movement (WASD / Arrow keys)
        float moveX = Input.GetAxis("Horizontal"); // Left/Right movement (A/D or Arrow Left/Right)
        float moveZ = Input.GetAxis("Vertical");   // Forward/Backward movement (W/S or Arrow Up/Down)
        float moveY = 0f; // Initialize Y-axis movement

        // Get vertical movement with up/down arrow keys
        if (Input.GetKey(KeyCode.Equals))
        {
            moveY = 1f; // Move up when "+" is pressed
        }
        else if (Input.GetKey(KeyCode.Minus))
        {
            moveY = -1f; // Move down when "-" is pressed
        }

        // Movement vector based on input (X, Y, Z)
        Vector3 moveDirection = new Vector3(moveX, moveY, moveZ).normalized;

        // If boosting, increase speed
        float currentMoveSpeed = (boostTimeRemaining > 0f) ? moveSpeed * boostMultiplier : moveSpeed;

        // Apply movement
        transform.Translate(moveDirection * currentMoveSpeed * Time.deltaTime, Space.World);
    }
}
