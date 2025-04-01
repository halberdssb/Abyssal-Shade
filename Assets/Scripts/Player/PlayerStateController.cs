using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Handles finite state machine logic for the player controller
 * 
 * Jeff Stevenson
 * 2.25.25
 */

public class PlayerStateController : MonoBehaviour
{

    #region Public Variables

    public PlayerCameraController cameraController;

    public float dashCooldownTimer;
    #endregion

    #region Private Variables

    [SerializeField]
    private PlayerData data;
    // state instances
    private PlayerSwimState swimState = new PlayerSwimState();

    private PlayerBaseState currentState; // current state player is in

    #endregion

    #region Component References

    private Rigidbody rb;
    private PlayerControls controls;
    private SwimMovement swimMovement;

    #endregion

    #region Getters & Setters

    public PlayerData Data
    {
        get { return data; }
    }

    public PlayerSwimState SwimState
    {
        get { return swimState; }
    }

    public Rigidbody Rb
    { 
        get { return rb; } 
    }

    public PlayerControls Controls
    {
        get { return controls; }
    }

    public SwimMovement SwimMovement
    {
        get { return swimMovement; }
    }

    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        controls = GetComponent<PlayerControls>();
        swimMovement = GetComponent<SwimMovement>();

        SwitchState(SwimState);
    }

    private void Update()
    {
        currentState.OnUpdateState(this);
    }

    private void FixedUpdate()
    {
        currentState.OnFixedUpdatedState(this);
    }

    // switches current state to passed in state, calls OnEnter and OnExit on appropriate states
    public void SwitchState(PlayerBaseState stateToSwitchTo)
    {
        currentState?.OnExitState(this);
        currentState = stateToSwitchTo;
        currentState.OnEnterState(this);
    }
}
