using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleForwardMove : MonoBehaviour
{
    public float speed = 10f;

    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
        }
    }
}
