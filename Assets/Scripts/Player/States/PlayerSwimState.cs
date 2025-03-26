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
    public override void OnEnterState(PlayerStateController player)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public override void OnUpdateState(PlayerStateController player)
    {
        player.cameraFollow.Rotate();
    }
    public override void OnFixedUpdatedState(PlayerStateController player)
    {
        // rotate input vector based on camera rotation
        player.SwimMovement.Swim(player.Rb, player.Controls.MovementInput, player.Data.swimSpeed, player.Data.strafeSpeed, player.cameraFollow.transform);
        player.SwimMovement.Roll(player.Rb, player.Controls.RollInput, player.Data.rollSpeed, player.cameraFollow.transform);
        if (player.transform.forward != player.cameraFollow.transform.forward)
        {
            player.SwimMovement.SmoothTurn(player.transform, player.cameraFollow.transform, player.Controls.MovementInput, player.Data.turnSpeed);
        }
    }
    public override void OnExitState(PlayerStateController player)
    {

    }
}
