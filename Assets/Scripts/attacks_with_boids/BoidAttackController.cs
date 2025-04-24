using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidAttackController : MonoBehaviour
{
    [Header("Attack Settings")]
    [Tooltip("How fast the boids rush forward during the attack.")]
    public float attackSpeed = 10f;

    [Tooltip("How long (in seconds) the boids will rush forward.")]
    public float attackDuration = 1f;

    [Tooltip("The maximum distance a boid can be from the player to be considered for attack.")]
    public float attackRange = 10f;

    public Transform playerTransform; // drag the Player into this in the Inspector

    void Update()
    {
        // When the player presses the E key...
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector3 attackDirection = playerTransform.forward;
            Debug.Log("E key pressed. Attack direction: " + attackDirection);

            // Find all boids with BoidAttack component in the scene
            BoidAttack[] boidAttacks = FindObjectsOfType<BoidAttack>();

            int countInRange = 0;
            foreach (BoidAttack boid in boidAttacks)
            {
                float distance = Vector3.Distance(boid.transform.position, playerTransform.position);
                if (distance <= attackRange)
                {
                    countInRange++;
                    boid.StartAttack(attackSpeed, attackDuration, attackDirection);
                }
            }
            Debug.Log("Found and commanded " + countInRange + " boids to attack.");
        }
    }
}

