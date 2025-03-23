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
        // rotate input vector based on camera rotation

        Vector3 rotatedInputVector = player.Controls.MovementInput * player.cameraFollow.transform.forward;
        Debug.Log(rotatedInputVector);
        player.SwimMovement.Swim(player.Rb, player.Controls.MovementInput, player.Data.swimSpeed, player.Data.strafeSpeed, player.cameraFollow.transform);
        //player.SwimMovement.Turn(player.CameraLookRb, player.Controls.LookInput, player.Data.lookSensitivity);
        player.SwimMovement.Roll(player.Rb, player.Controls.RollInput, player.Data.rollSpeed);
    }
    public override void OnExitState(PlayerStateController player)
    {

    }
}
