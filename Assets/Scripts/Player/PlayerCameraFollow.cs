using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*
 * Handles rotation of camera around player to smooth model 
 * 
 * Jeff Stevenson
 * 3.23.25
 */

public class PlayerCameraFollow : MonoBehaviour
{
    private PlayerStateController player;

    private void Awake()
    {
        player = GetComponentInParent<PlayerStateController>();
        transform.parent = null;
    }


    void Update()
    {
        // update camera movement
        //Rotate();
    }

    // rotates based on player mouse input and sensitivity
    public void Rotate()
    {
        // switch x and y axis of mouse input to correctly rotate based on input
        Vector3 flippedLookInput = new Vector3(-player.Controls.LookInput.y, player.Controls.LookInput.x);
        transform.Rotate(flippedLookInput * player.Data.lookSensitivity);

        // clamp vertical rotation to min and max values to prevent camera jittering
        Debug.Log(transform.rotation.eulerAngles.x);

        float clampedYLook = transform.rotation.eulerAngles.x;
        if (clampedYLook > 180)
        {
            clampedYLook = Mathf.Clamp(clampedYLook, 275, 360);
        }
        else
        {
            clampedYLook = Mathf.Clamp(clampedYLook, 0, 85);
        }
        transform.rotation = Quaternion.Euler(clampedYLook, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }
}
