using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsTarget : MonoBehaviour
{
    //Gets the player character to move towards
    public GameObject playerCharacter;

    //Speed which the eel moves to the player
    public float enemySpeed = 3f;

    void Update()
    {
        //Gets the position of the player and then moves towards them

        Vector3 targetPosition = playerCharacter.transform.position;

        Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, enemySpeed * Time.deltaTime);

        transform.position = newPosition;
    }
}

