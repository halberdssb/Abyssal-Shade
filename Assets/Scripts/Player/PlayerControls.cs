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

    #endregion

    #region Methods
    /* NOTE: all methods must be named On____, where ____ is the name of an
    * input action in the PlayerInputActions asset
    */
    public void OnMove(InputValue value)
    {
        Debug.Log("movement happening!");
        _movementInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        _lookInput = value.Get<Vector2>();
    }

    private void Update()
    {
        //Debug.Log(MovementInput);
    }

    #endregion
}
;