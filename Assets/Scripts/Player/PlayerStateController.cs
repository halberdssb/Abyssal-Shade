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
    #region Static Variables
    public static float BoidCollectionDistance;

    #endregion

    #region Public Variables

    public PlayerCameraController cameraController;
    public PlayerVFXHandler vfxHandler;
    public BoidCollectionHandler boidCollectionHandler;
    public Animator anim;
    public Current currentAbility;
    public PlayerUI ui;

    public AudioSource dashSound;
    public AudioSource currentSound;
    public AudioSource pickupSound;

    public bool blockInput;

    [HideInInspector]
    public float dashCooldownTimer;
    [HideInInspector]
    public float currentCooldownTimer;
    [HideInInspector]
    public bool isDashHeld;
    [HideInInspector]
    public float swimSpeedMod = 1f;
    [HideInInspector]
    public bool isCommandHeld;
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

        BoidCollectionDistance = Data.defaultBoidCollectionDistance;
        currentAbility.transform.parent = null;
        currentAbility.gameObject.SetActive(false);
        swimSpeedMod = 1f;

        SwitchState(SwimState);
    }

    private void Update()
    {
        if (!MenuManager.IsGamePaused)
        {
            currentState.OnUpdateState(this);
        }
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
