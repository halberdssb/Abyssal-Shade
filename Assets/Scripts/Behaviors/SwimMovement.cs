using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Contains methods for swimming movement for all swimming entities
 * 
 * Jeff Stevenson
 * 2.25.25
 */

public class SwimMovement : MonoBehaviour
{
    void Start()
    {
        
    }

    // returns swim force vector to move entity based on input
    public void Swim(Rigidbody rb, Vector2 moveInput)
    {
        Swim(rb, moveInput, 1, 1);
    }

    // overload that takes in custom swim speed values
    public void Swim(Rigidbody rb, Vector2 moveInput, float swimSpeed, float StrafeSpeed)
    {
        Vector3 forwardSwimForce = rb.transform.TransformDirection(Vector3.forward) * moveInput.y * swimSpeed;
        Vector3 strafeForce = rb.transform.TransformDirection(Vector3.right) * moveInput.x * swimSpeed;

        Vector3 totalSwimForce = forwardSwimForce + strafeForce;

        // clamp to swim speed to diagonal movement is not faster than forwards - NOT USED right now
        //totalSwimForce = totalSwimForce.normalized * swimSpeed;

        rb.AddForce(totalSwimForce, ForceMode.Force);
    }

    // adds torque to rotate/turn entity based on input
    public void Turn(Rigidbody rb, Vector2 lookInput)
    {
        Turn(rb, lookInput, 1);
    }

    // overload that takes in sensitivity (for player)
    public void Turn(Rigidbody rb, Vector2 lookInput, float sensitivity)
    {
        // multiply by -1 when necessary for proper axis inversions
        Vector3 horizontalTurnForce = rb.transform.TransformDirection(Vector3.up) * lookInput.x * sensitivity;
        Vector3 verticalTurnForce = rb.transform.TransformDirection(Vector3.right) * lookInput.y * sensitivity * -1;

        Vector3 totalTurnForce = horizontalTurnForce + verticalTurnForce;

        rb.AddTorque(totalTurnForce, ForceMode.VelocityChange);
    }

    // adds roll (rotation around forward axis) to the rigibbody
    public void Roll(Rigidbody rb, float rollInput)
    {
        Roll(rb, rollInput, 1);
    }

    // overload that takes in roll speed (for player)
    public void Roll(Rigidbody rb, float rollInput, float rollSpeed)
    {
        Vector3 rollForce = rb.transform.TransformDirection(Vector3.forward) * rollInput * rollSpeed;

        rb.AddTorque(rollForce, ForceMode.Force);
    }
}
