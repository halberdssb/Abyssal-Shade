using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidAttack : MonoBehaviour
{
    private bool isAttacking = false;
    private float attackSpeed;
    private float attackDuration;
    private Vector3 attackDirection;

    private MonoBehaviour formationScript;


    void Update()
    {
        if (isAttacking)
        {
            Debug.DrawRay(transform.position, attackDirection * 5f, Color.red);
        }
    }

    public void StartAttack(float speed, float duration, Vector3 direction)
    {
        if (!isAttacking)
        {
            attackSpeed = speed;
            attackDuration = duration;
            attackDirection = direction.normalized;
            Debug.Log($"{gameObject.name} received attack direction {attackDirection} from player.");
            isAttacking = true;
            Debug.Log(gameObject.name + " starting attack with direction " + attackDirection);

            // Correct script name here if not BoidManager
            formationScript = GetComponent<BoidManager>();
            if (formationScript != null)
            {
                formationScript.enabled = false;
                Debug.Log(gameObject.name + " formation script disabled.");
            }

            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        float timer = 0f;
        Debug.Log(gameObject.name + " attack routine started.");

        while (timer < attackDuration)
        {
            transform.Translate(attackDirection * attackSpeed * Time.deltaTime, Space.World);
            transform.position += attackDirection * attackSpeed * Time.deltaTime;
            Debug.Log($"{gameObject.name} attacking with dir {attackDirection}, speed {attackSpeed}");

            timer += Time.deltaTime;
            yield return null;
        }

        isAttacking = false;
        Debug.Log(gameObject.name + " attack ended.");

        if (formationScript != null)
        {
            formationScript.enabled = true;
            Debug.Log(gameObject.name + " formation script re-enabled.");
        }
    }
}



