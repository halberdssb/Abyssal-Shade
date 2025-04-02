using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Default swimming/movement state for player
 * Allows basic movement, looking, and abilities
 * 
 * Jeff Stevenson
 * 2.25.25
 */

public class PlayerSwimState : PlayerBaseState
{
    private bool isDashHeld;
    private Coroutine dashBoidCollectionRoutine;

    public override void OnEnterState(PlayerStateController player)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public override void OnUpdateState(PlayerStateController player)
    {
        // handle camera movement
        player.cameraController.Rotate();
        player.cameraController.Zoom();

        // decrement dash cooldown
        if (player.dashCooldownTimer > 0)
        {
            player.dashCooldownTimer -= Time.deltaTime;
        }
    }
    public override void OnFixedUpdatedState(PlayerStateController player)
    {
        // handle movement, turning, dashing
        Debug.Log("input: " + player.Controls.MovementInput.x);
        player.SwimMovement.Swim(player.Rb, player.Controls.MovementInput, player.Data.swimSpeed, player.Data.strafeSpeed, player.cameraController.transform);

        // currently disabling roll - don't like how it feels w/ camera movement
        //player.SwimMovement.Roll(player.Rb, player.Controls.RollInput, player.Data.rollSpeed, player.cameraController.transform);

        player.SwimMovement.SmoothTurn(player.Rb, player.cameraController.transform, player.Controls.MovementInput, player.Data.turnSpeed, player.Data.maxTurnZRotation);

        if (player.Controls.DashPressed && !isDashHeld && player.dashCooldownTimer <= 0 && player.Controls.MovementInput.sqrMagnitude > 0)
        {
            player.SwimMovement.Dash(player.Rb, player.Controls.MovementInput, player.Data.dashSpeed, player.cameraController.transform);
            player.dashSound.Play();
            isDashHeld = true;
            player.dashCooldownTimer = player.Data.dashCooldown;

            if (dashBoidCollectionRoutine != null)
            {
                dashBoidCollectionRoutine = player.StartCoroutine(DashCollectionRadiusChangeRoutine(player));
            }
        }
        else if (!player.Controls.DashPressed && player.dashCooldownTimer <= player.Data.dashBufferWindow)
        {
            isDashHeld = false;
        }
    }
    public override void OnExitState(PlayerStateController player)
    {

    }

    // increases boid collection radius for a period of time while dashing
    private IEnumerator DashCollectionRadiusChangeRoutine(PlayerStateController player)
    {
        PlayerStateController.BoidCollectionDistance = player.Data.dashBoidCollectionDistance;

        yield return new WaitForSeconds(player.Data.dashBoidCollectionTime);

        PlayerStateController.BoidCollectionDistance = player.Data.defaultBoidCollectionDistance;
    }
}
