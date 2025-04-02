using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsTarget : MonoBehaviour
{
    public GameObject playerCharacter;

    public float enemySpeed = 3f;

    void Update()
    {
        Vector3 targetPosition = playerCharacter.transform.position;

        Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, enemySpeed * Time.deltaTime);

        transform.position = newPosition;
    }
}

