using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

/*
 * Contains methods for swimming movement for all swimming entities
 * 
 * Jeff Stevenson
 * 2.25.25
 */

public class SwimMovement : MonoBehaviour
{
    private Vector3 testVel;

    // returns swim force vector to move entity based on input
    public void Swim(Rigidbody rb, Vector2 moveInput)
    {
        Swim(rb, moveInput, 1, 1);
    }

    // overload that takes in custom swim speed values
    public void Swim(Rigidbody rb, Vector2 moveInput, float swimSpeed, float strafeSpeed, Transform rotationTransform = null)
    {
        // if no outside rotation transform used (like player cam follow), use rb transform
        if (rotationTransform == null)
        {
            rotationTransform = rb.transform;
        }

        Vector3 forwardSwimForce = rotationTransform.TransformDirection(Vector3.forward) * moveInput.y * swimSpeed;
        //float forwardSwimForce = moveInput.y * swimSpeed;
        Vector3 strafeForce = rotationTransform.TransformDirection(Vector3.right) * moveInput.x * strafeSpeed;
        //float strafeForce = moveInput.x * strafeSpeed;

        Vector3 totalSwimForce = forwardSwimForce + strafeForce;
        //Vector3 totalSwimForce = new Vector3(strafeForce, 0, forwardSwimForce);

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
    public void Roll(Rigidbody rb, float rollInput, float rollSpeed, Transform lookTransform = null)
    {
        Vector3 rollForce = rb.transform.TransformDirection(Vector3.forward) * rollInput * rollSpeed;

        rb.AddTorque(rollForce, ForceMode.Force);
    }

    // smoothly lerps a rb rotation to another - for player turning towards camera direction
    public void SmoothTurn(Rigidbody rbToTurn, Transform target, Vector2 directionalInput, float turnSpeed, float maxZRotation)
    {
        float lerpTValue = directionalInput.sqrMagnitude * turnSpeed * Time.fixedDeltaTime;
        Quaternion smoothTurnRotation = Quaternion.Slerp(rbToTurn.transform.rotation, target.rotation, lerpTValue);

        // handle z lerp rotation to animated based on horizontal movement
        float zRotation = -Vector3.Dot(-rbToTurn.transform.forward, target.right * maxZRotation);
        Vector3 zRotationVector = Vector3.zero;

        if (lerpTValue != 0)
        {
            zRotationVector.z = Mathf.Lerp(rbToTurn.transform.rotation.z, zRotation, lerpTValue);
        }

        Vector3 finalEulerRotation = smoothTurnRotation.eulerAngles + zRotationVector;

        rbToTurn.rotation = Quaternion.Euler(finalEulerRotation);
    }

    // applies a boost in the given direction for dashing/dodging
    public void Dash(Rigidbody rb, Vector2 moveInput, float dashSpeed, Transform lookTransform = null)
    {
        Vector3 adjustedMoveInput = new Vector3(moveInput.x, 0, moveInput.y);

        Transform transformForwardToUse = lookTransform != null ? lookTransform : rb.transform;

        Vector3 dashForce = transformForwardToUse.TransformDirection(adjustedMoveInput) * dashSpeed;

        rb.AddForce(dashForce, ForceMode.Impulse);
    }
}
