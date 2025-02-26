using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishCollectible : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.AddFish(transform); // Attach fish to the player
            GetComponent<Collider>().enabled = false; // Disable collider to prevent multiple collections
        }
    }
}