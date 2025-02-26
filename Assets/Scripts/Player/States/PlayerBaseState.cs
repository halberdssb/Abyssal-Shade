using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Abstract base state class for player state machine
 * 
 * Jeff Stevenson
 * 2.25.25
 */

public abstract class PlayerBaseState
{
    public abstract void OnEnterState(PlayerStateController player);
    public abstract void OnUpdateState(PlayerStateController player);
    public abstract void OnFixedUpdatedState(PlayerStateController player);
    public abstract void OnExitState(PlayerStateController player);
}
