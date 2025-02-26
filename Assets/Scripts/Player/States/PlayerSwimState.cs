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

    }
    public override void OnFixedUpdatedState(PlayerStateController player)
    {
        player.SwimMovement.Swim(player.Rb, player.Controls.MovementInput, player.Data.swimSpeed, player.Data.strafeSpeed);
        player.SwimMovement.Turn(player.Rb, player.Controls.LookInput, player.Data.lookSensitivity);
    }
    public override void OnExitState(PlayerStateController player)
    {

    }
}
