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
    private bool inCurrentAbility = false;
    private float currentStartupTimer;

    public override void OnEnterState(PlayerStateController player)
    {
        if (player.isMainMenu) return;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public override void OnUpdateState(PlayerStateController player)
    {
        if (player.blockInput)
        {
            UpdateAnims(player, Vector2.zero);
            return;
        }

        UpdateAnims(player, player.Controls.MovementInput);

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
        if (player.blockInput)
        {
            player.SwimMovement.SmoothTurn(player.Rb, player.cameraController.transform, Vector2.zero, player.Data.turnSpeed * player.Data.currentAimTurnSpeedMod, player.Data.maxTurnZRotation);
            return;
        }
        // don't move if in current ability anim
        if (!inCurrentAbility)
        {
            HandleMovement(player);
        }

        HandleDash(player);
    }
    public override void OnExitState(PlayerStateController player)
    {
        return;
    }


    // coroutine that handles dash timing for visuals, startup, and force
    private IEnumerator DashRoutine(PlayerStateController player)
    {
        player.dashCooldownTimer = player.Data.dashCooldown;

        // start animation and vfx before dash force is applied
        player.anim.SetTrigger("Dash");
        player.vfxHandler.TriggerDashVFX(player.Data.dashVFXDuration + player.Data.dashStartupTime);

        yield return new WaitForSeconds(player.Data.dashStartupTime);

        // dash
        float totalDashSpeed = player.Data.dashSpeed * player.swimSpeedMod;
        player.SwimMovement.Dash(player.Rb, player.Controls.MovementInput, totalDashSpeed, player.cameraController.transform);

        // effects
        player.dashSound.Play();

        // increase player boid collection range during dash
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
            player.isDashHeld = true;
            // make sure player not using current ability
            if (!inCurrentAbility)
            {
                player.StartCoroutine(DashRoutine(player));
            }
        }
        else if (!player.Controls.DashPressed && player.dashCooldownTimer <= player.Data.dashBufferWindow)
        {
            player.isDashHeld = false;
        }
    }

    // starts current ability anim and delays ability start to match animation
    private IEnumerator AttackRoutine(PlayerStateController player)
    {
        inCurrentAbility = true;
        player.currentCooldownTimer = player.Data.currentCooldown;
        player.anim.SetTrigger("Current");

        // wait for anim startup and turn player towards look direction 
        currentStartupTimer = player.Data.currentStartupTime;
        while (currentStartupTimer > 0)
        {
            // turn while aiming - pass in vector2.one as direction input to always turn regardless of move input
            player.SwimMovement.SmoothTurn(player.Rb, player.cameraController.transform, Vector2.one, player.Data.turnSpeed * player.Data.currentAimTurnSpeedMod, player.Data.maxTurnZRotation);
            currentStartupTimer -= Time.deltaTime;
            yield return null;
        }

        // create current in direction of camera
        Vector3 attackDirection = player.cameraController.transform.forward;
        // offset attack to start at player and stretch in front
        Vector3 currentPosOffset = player.transform.position + attackDirection * player.currentAbility.transform.localScale.y;

        // create current and start cooldown
        player.currentAbility.EnableCurrentForTime(player.Data.currentLifetime, currentPosOffset, attackDirection);

        //sfx
        player.currentSound.Play();

        // wait to turn anim bool off - to not overlap with dash
        yield return new WaitForSeconds(player.Data.currentExitTime);

        inCurrentAbility = false;
    }

    // creates a current ahead of player that pushes boids forward
    private void HandleBoidAttack(PlayerStateController player)
    {
        // create current if new current input pressed and ability is off cooldown
        if (player.Controls.CommandPressed && !player.isCommandHeld && player.currentCooldownTimer <= 0)
        {
            player.isCommandHeld = true;
            // make sure player not in dash
            if (player.dashCooldownTimer <= player.Data.dashBufferWindow)
            {
                player.StartCoroutine(AttackRoutine(player));
            }
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
    private void UpdateAnims(PlayerStateController player, Vector2 moveInput)
    {
        // value used for determing if dash roll should spin left or right - 
        // first use x move input, if 0 use diff from player forward and cam forward to spin towards camera center
        float xInputVal;
        if (moveInput.sqrMagnitude <= 0)
        {
            xInputVal = 0f;
        }
        else
        {
            float differenceFromCamDirection = Vector3.SignedAngle(player.cameraController.transform.forward, player.transform.forward, player.cameraController.transform.up);
            xInputVal = moveInput.x != 0 ? moveInput.x : -Mathf.Sign(differenceFromCamDirection);
        }

        // want player to bank if they are moving laterally or moving at all and turning camera - otherwise they should level out to flat
        bool shouldBeBanking = moveInput.x != 0 || (moveInput.sqrMagnitude > 0 && player.Controls.LookInput.sqrMagnitude > 0);
        // if should be banking, we want to lerp float towards bank direction, otherwise bank to 0 (level)
        float finalSign = shouldBeBanking ? Mathf.Sign(xInputVal) : 0;
        float lerpVal = Mathf.Lerp(player.anim.GetFloat("BankValue"), finalSign, Time.deltaTime);
        // if lerp value is small enough, just set to 0
        if (Mathf.Abs(finalSign - lerpVal) < 0.01) lerpVal = finalSign;

        // set bank value and dash direction
        player.anim.SetFloat("BankValue", lerpVal);
        player.anim.SetFloat("DashDirection", xInputVal);
    }
}
