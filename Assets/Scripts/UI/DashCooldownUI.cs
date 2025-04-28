using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashCooldownUI : MonoBehaviour
{
    public PlayerStateController player;

    private Image cooldownImage;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerStateController>();
        cooldownImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log((player.Data.currentCooldown - player.currentCooldownTimer) / player.Data.currentCooldown);
        cooldownImage.fillAmount = (player.Data.currentCooldown - player.currentCooldownTimer) / player.Data.currentCooldown;
    }
}
