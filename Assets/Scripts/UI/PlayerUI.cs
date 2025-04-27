using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private PlayerStateController player;

    [Space]
    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private TextMeshProUGUI boidCount;
    [SerializeField]
    private Image currentCooldown;

    void Start()
    {
        
    }

    void Update()
    {
        // update boid count
        if (boidCount.text != player.boidCollectionHandler.GetNumberOfBoids().ToString())
        {
            boidCount.text = player.boidCollectionHandler.GetNumberOfBoids().ToString();

        }

        // add current cooldown anim/effect here
    }

    public void FadeUI(bool fadeIn, float fadeTime)
    {
        float alpha = fadeIn ? 1.0f : 0.0f;
        canvasGroup.DOFade(alpha, fadeTime);
    }
}
