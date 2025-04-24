using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * Handles input from PlayerInputAction asset through new input system
 * 
 * Jeff Stevenson
 * 2.25.25
 */

public class PlayerControls : MonoBehaviour
{
    #region Private Variables

    private Vector2 _movementInput;
    private Vector2 _lookInput;
    private float _zoomInput;
    private float _rollInput;
    private bool _dashPressed;
    private bool _pausePressed;
    private bool _commandPressed;

    #endregion

    #region Getters

    public Vector2 MovementInput 
    { 
        get { return _movementInput; } 
    }
    public Vector2 LookInput 
    { 
        get { return _lookInput; }
    }

    public float ZoomInput
    {
        get { return _zoomInput; }
    }

    public float RollInput
    {
        get { return _rollInput; }
    }

    public bool DashPressed
    {
        get { return _dashPressed; }
    }

    public bool PausePressed
    {
        get { return _pausePressed; }
    }

    public bool CommandPressed
    {
        get { return _commandPressed; }
    }

    #endregion

    #region Methods
    /* NOTE: all methods must be named On____, where ____ is the name of an
    * input action in the PlayerInputActions asset
    */
    public void OnMove(InputValue value)
    {
        _movementInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        _lookInput = value.Get<Vector2>();
    }

    public void OnZoom(InputValue value)
    {
        _zoomInput = value.Get<float>();
    }

    public void OnRoll(InputValue value)
    {
        _rollInput = value.Get<float>();
    }

    public void OnDash(InputValue value)
    {
        _dashPressed = value.isPressed;
    }

    public void OnPause(InputValue value)
    {
        _pausePressed = value.isPressed;
    }

    public void OnCommand(InputValue value)
    {
        _commandPressed = value.isPressed;
    }

    #endregion
}
;