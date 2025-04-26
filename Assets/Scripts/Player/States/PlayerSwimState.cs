using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private Coroutine dashBoidCollectionRoutine;

    public override void OnEnterState(PlayerStateController player)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public override void OnUpdateState(PlayerStateController player)
    {
        UpdateAnims(player);

        HandleCameraMovement(player);

        HandleBoidAttack(player);

        // decrement dash cooldown
        if (player.dashCooldownTimer > 0)
        {
            player.dashCooldownTimer -= Time.deltaTime;
        }
    }
    public override void OnFixedUpdatedState(PlayerStateController player)
    {
        HandleMovement(player);

        HandleDash(player);
    }
    public override void OnExitState(PlayerStateController player)
    {
        return;
    }

    // increases boid collection radius for a period of time while dashing
    private IEnumerator DashCollectionRadiusChangeRoutine(PlayerStateController player)
    {
        PlayerStateController.BoidCollectionDistance = player.Data.dashBoidCollectionDistance;

        yield return new WaitForSeconds(player.Data.dashBoidCollectionTime);

        PlayerStateController.BoidCollectionDistance = player.Data.defaultBoidCollectionDistance;
    }

    // handles moving and turning player with physics system
    private void HandleMovement(PlayerStateController player)
    {
        // handle movement, turning, dashing
        float totalSwimSpeed = player.Data.swimSpeed * player.swimSpeedMod;
        float totalStrafeSpeed = player.Data.strafeSpeed * player.swimSpeedMod;
        player.SwimMovement.Swim(player.Rb, player.Controls.MovementInput, totalSwimSpeed, totalStrafeSpeed, player.cameraController.transform);
        player.SwimMovement.SmoothTurn(player.Rb, player.cameraController.transform, player.Controls.MovementInput, player.Data.turnSpeed, player.Data.maxTurnZRotation);
    }

    // moves camera based on mouse input
    private void HandleCameraMovement(PlayerStateController player)
    {
        // handle camera movement
        player.cameraController.Rotate();
        player.cameraController.Zoom();
    }

    // handles dash forward, cooldown, and dash effects
    private void HandleDash(PlayerStateController player)
    {
        // if new dash input occurs and off cooldown, dash
        if (player.Controls.DashPressed && !player.isDashHeld && player.dashCooldownTimer <= 0 && player.Controls.MovementInput.sqrMagnitude > 0)
        {
            // dash
            float totalDashSpeed = player.Data.dashSpeed * player.swimSpeedMod;
            player.SwimMovement.Dash(player.Rb, player.Controls.MovementInput, totalDashSpeed, player.cameraController.transform);

            // effects
            player.dashSound.Play();
            player.vfxHandler.TriggerDashVFX(player.Data.dashVFXDuration);
            player.anim.SetTrigger("Dash");

            // set cooldown and input buffer
            player.isDashHeld = true;
            player.dashCooldownTimer = player.Data.dashCooldown;

            // increase player boid collection range during dash
            if (dashBoidCollectionRoutine != null)
            {
                dashBoidCollectionRoutine = player.StartCoroutine(DashCollectionRadiusChangeRoutine(player));
            }
        }
        else if (!player.Controls.DashPressed && player.dashCooldownTimer <= player.Data.dashBufferWindow)
        {
            player.isDashHeld = false;
        }
    }

    // creates a current ahead of player that pushes boids forward
    private void HandleBoidAttack(PlayerStateController player)
    {
        // create current if new current input pressed and ability is off cooldown
        if (player.Controls.CommandPressed && !player.isCommandHeld && player.currentCooldownTimer <= 0)
        {
            player.isCommandHeld = true;

            // create current in direction of camera
            Vector3 attackDirection = player.cameraController.transform.forward;
            // offset attack to start at player and stretch in front
            Vector3 currentPosOffset = player.transform.position + attackDirection * player.currentAbility.transform.localScale.y;

            // create current and start cooldown
            player.currentAbility.EnableCurrentForTime(player.Data.currentLifetime, currentPosOffset, attackDirection);
            player.currentCooldownTimer = player.Data.currentCooldown;
        }
        else if (!player.Controls.CommandPressed)
        {
            player.isCommandHeld = false;
        }

        // decrement cooldown timer
        if (player.currentCooldownTimer > 0)
        {
            player.currentCooldownTimer -= Time.deltaTime;
        }
    }

    // update animation parameters for player
    private void UpdateAnims(PlayerStateController player)
    {
        // value used for determing if dash roll should spin left or right - 
        // first use x move input, if 0 use diff from player forward and cam forward to spin towards camera center
        float differenceFromCamDirection = Vector3.SignedAngle(player.cameraController.transform.forward, player.transform.forward, player.cameraController.transform.up);
        float xInputVal = player.Controls.MovementInput.x != 0 ? player.Controls.MovementInput.x : -Mathf.Sign(differenceFromCamDirection);
        player.anim.SetFloat("XInput", xInputVal);
    }
}
