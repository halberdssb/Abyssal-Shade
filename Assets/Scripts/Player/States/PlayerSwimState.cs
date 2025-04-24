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

    }

    // increases boid collection radius for a period of time while dashing
    private IEnumerator DashCollectionRadiusChangeRoutine(PlayerStateController player)
    {
        PlayerStateController.BoidCollectionDistance = player.Data.dashBoidCollectionDistance;

        yield return new WaitForSeconds(player.Data.dashBoidCollectionTime);

        PlayerStateController.BoidCollectionDistance = player.Data.defaultBoidCollectionDistance;
    }

    private void HandleMovement(PlayerStateController player)
    {
        // handle movement, turning, dashing
        player.SwimMovement.Swim(player.Rb, player.Controls.MovementInput, player.Data.swimSpeed, player.Data.strafeSpeed, player.cameraController.transform);
        player.SwimMovement.SmoothTurn(player.Rb, player.cameraController.transform, player.Controls.MovementInput, player.Data.turnSpeed, player.Data.maxTurnZRotation);
    }

    private void HandleCameraMovement(PlayerStateController player)
    {
        // handle camera movement
        player.cameraController.Rotate();
        player.cameraController.Zoom();
    }

    private void HandleDash(PlayerStateController player)
    {
        if (player.Controls.DashPressed && !player.isDashHeld && player.dashCooldownTimer <= 0 && player.Controls.MovementInput.sqrMagnitude > 0)
        {
            player.SwimMovement.Dash(player.Rb, player.Controls.MovementInput, player.Data.dashSpeed, player.cameraController.transform);
            player.dashSound.Play();
            player.vfxHandler.TriggerDashVFX(player.Data.dashVFXDuration);
            player.isDashHeld = true;
            player.dashCooldownTimer = player.Data.dashCooldown;

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

    private void HandleBoidAttack(PlayerStateController player)
    {
        if (player.Controls.CommandPressed && !player.isCommandHeld)
        {
            player.isCommandHeld = true;

            Vector3 attackDirection = player.cameraController.transform.forward;
            //Debug.Log("E key pressed. Attack direction: " + attackDirection);

            // Find all boids with BoidAttack component in the scene
            BoidObject[] boids = player.boidCollectionHandler.CallBoids();
            Debug.Log("boid attack! boids to send: " + boids.Length);
            int countInRange = 0;
            foreach (BoidObject boid in boids)
            {
                countInRange++;

                MoveAttackFollowObject(player, player.attackFollowObj);
                boid.OnBoidAttackUsed(player.attackFollowObj, player.Data.boidAttackSpeed);
            }
            Debug.Log("Found and commanded " + countInRange + " boids to attack.");
        }
        else if (!player.Controls.CommandPressed)
        {
            player.isCommandHeld = false;
        }
    }

    private void MoveAttackFollowObject(PlayerStateController player, GameObject attackFollowObj)
    {
        // cast a ray to figure out attack end position
        Ray attackRay = new Ray(player.transform.position, player.cameraController.transform.forward);
        RaycastHit hitInfo = new RaycastHit();
        bool didRaycastHit = Physics.Raycast(attackRay, out hitInfo, player.Data.boidAttackDistance, player.Data.boidAttackCollisionMask);

        Vector3 attackEndPos;
        if (didRaycastHit)
        {
            float distanceAwayFromHit = 3f;
            attackEndPos = hitInfo.point - player.cameraController.transform.forward * distanceAwayFromHit;
        }
        else
        {
            attackEndPos = player.transform.position + (player.cameraController.transform.forward * player.Data.boidAttackDistance);
        }

        attackFollowObj.transform.position = attackEndPos;
    }
}
