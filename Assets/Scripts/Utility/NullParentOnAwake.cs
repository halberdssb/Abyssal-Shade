using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Sets gameobjects's parent null on awake
 * 
 * Jeff Stevenson
 * 3.23.25
 */

public class NullParentOnAwake : MonoBehaviour
{
    private void Awake()
    {
        transform.parent = null;
    }
}
