using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Holds data values for player controller & state machine
 * 
 * Jeff Stevenson
 * 2.25.25
 */

[CreateAssetMenu(menuName = "Data/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Movement")]
    public float swimSpeed;
    public float strafeSpeed;
    public float rollSpeed;
    [Range(0f, 1f)]
    public float lookSensitivity;
}
