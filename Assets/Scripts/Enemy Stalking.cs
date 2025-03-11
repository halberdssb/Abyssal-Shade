using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStalking : MonoBehaviour
{
    public Transform player;
    public Transform patrolRoute;
    public float orbitRadius = 20f;
    public float rotationSpeed = 1f;
    public float yOffset = 0f;

    void Start()
    {

    }

    void Update()
    {
        /*
        if(stalkingPlayer == true)
        {
            if (player == null) return;

            // Calculate the angle based on time and rotation speed
            float angle = Time.time * rotationSpeed;

            // Calculate the new position
            float x = player.position.x + Mathf.Cos(angle) * orbitRadius;
            float z = player.position.z + Mathf.Sin(angle) * orbitRadius;

            // Set the new position
            transform.position = new Vector3(x, player.position.y + yOffset, z);
        }
        */
    }


}
