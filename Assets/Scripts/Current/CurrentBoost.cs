using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * when right click is pressed, sends the player forward with a boost while spawning a bubble particle system for effects.
 * 
 * Devraj Singh
 * 4.11.25
 */

public class CurrentBoost : MonoBehaviour
{
    [Header("Current Settings")]
    [Tooltip("Prefab of the current effect to instantiate.")]
    public GameObject currentPrefab;

    [Tooltip("Duration the current effect should persist.")]
    public float currentDuration = 2f;

    [Header("Boost Settings")]
    [Tooltip("Force applied to the player for an instant speed boost.")]
    public float boostForce = 10f;

    [Tooltip("Cooldown duration (in seconds) between boosts.")]
    public float BoostCooldown = 10f;

    // HUD reference (assign in inspector)
    [Tooltip("UI Text element that displays the boost cooldown status")]
    public TextMeshProUGUI BoostCooldownText;


    // Private variables for internal tracking.
    private float nextBoostTime = 0f;
    private Rigidbody playerRb;

    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        if (playerRb == null)
        {
            Debug.LogError("CurrentBoost: No Rigidbody component found on the player!");
        }
        if (currentPrefab == null)
        {
            Debug.LogError("CurrentBoost: No currentPrefab assigned!");
        }
    }

    private void Update()
    {
        // Update the HUD element if one is assigned.
        if (BoostCooldownText != null)
        {
            float remainingCooldown = nextBoostTime - Time.time;
            if (remainingCooldown > 0f)
                BoostCooldownText.text = "Boost: " + remainingCooldown.ToString("F1") + " sec";
            else
                BoostCooldownText.text = "Boost: Ready!";
        }

        // Check for right mouse button click and ensure cooldown has expired.
        if (Input.GetMouseButtonDown(1) && Time.time >= nextBoostTime)
        {
            // Spawn the current in front of the player.
            Vector3 spawnPosition = transform.position + transform.forward;
            Quaternion spawnRotation = Quaternion.LookRotation(transform.forward);

            if (currentPrefab != null)
            {
                GameObject currentInstance = Instantiate(currentPrefab, spawnPosition, spawnRotation);
                Destroy(currentInstance, currentDuration);
            }

            // Apply an impulse force to the player's Rigidbody.
            if (playerRb != null)
            {
                playerRb.AddForce(transform.forward * boostForce, ForceMode.Impulse);
            }

            // Set the next allowed boost time.
            nextBoostTime = Time.time + BoostCooldown;
        }
    }
}


