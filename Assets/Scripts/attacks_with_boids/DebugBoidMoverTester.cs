using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugBoidMoverTester : MonoBehaviour
{
    public float speed = 10f;
    public float duration = 2f;

    private bool moving = false;
    private float timer = 0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            moving = true;
            timer = 0f;
            Debug.Log("DebugBoidMover: Movement started");
        }

        if (moving)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
            timer += Time.deltaTime;

            if (timer >= duration)
            {
                moving = false;
                Debug.Log("DebugBoidMover: Movement ended");
            }
        }
    }
}