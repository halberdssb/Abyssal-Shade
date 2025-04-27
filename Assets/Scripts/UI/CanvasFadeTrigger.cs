using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * Handles canvas that fades in and out when a trigger is entered/exited
 * 
 * Jeff Stevenson
 * 4.27.25
 */

[RequireComponent(typeof(Collider))]
public class CanvasFadeTrigger : MonoBehaviour
{
    private const float CAM_BRAIN_BLEND_TIME = 2f;
    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private float fadeTime;

    [Space]
    [SerializeField]
    private bool pauseInputUntilClick;
    [SerializeField]
    private float waitTimeUntilSkippable;
    [SerializeField]
    [Tooltip("Optional - will switch to a passed-in camera if pause input is enabled to highlight a given object/view.")]
    private CinemachineVirtualCamera highlightCamera;

    private PlayerStateController player;
    private bool useCamera;
    
    void Start()
    {
        player = FindObjectOfType<PlayerStateController>();
        if (highlightCamera != null)
        {
            highlightCamera.Priority = -1;
            useCamera = true;
        }
        FadeCanvasGroup(0, false);
    }

    private void FadeCanvasGroup(float fadeTime, bool fadeIn)
    {
        float alpha = fadeIn ? 1f : 0f;
        canvasGroup.DOFade(alpha, fadeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (pauseInputUntilClick)
            {
                StartCoroutine(WaitForClickRoutine());
            }
            if (!useCamera)
            {
                FadeCanvasGroup(fadeTime, true);
            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!pauseInputUntilClick)
            {
                FadeCanvasGroup(fadeTime, false);
            }
        }
    }

    // blocks input until user clicks to progress through popup 
    private IEnumerator WaitForClickRoutine()
    {
        player.blockInput = true;
        bool isClickHeld = player.Controls.CommandPressed;

        if (useCamera)
        {
            highlightCamera.Priority = 12;
            yield return new WaitForSeconds(CAM_BRAIN_BLEND_TIME);
        }

        yield return new WaitForSeconds(waitTimeUntilSkippable);


        if (useCamera)
        {
            FadeCanvasGroup(fadeTime, true);
        }

        yield return new WaitForSeconds(fadeTime);

        while (!player.Controls.CommandPressed && !isClickHeld)
        {
            isClickHeld = player.Controls.CommandPressed;
            yield return null;
        }

        if (useCamera)
        {
            highlightCamera.Priority = -1;
            yield return new WaitForSeconds(CAM_BRAIN_BLEND_TIME);
        }
        player.isCommandHeld = true;
        player.blockInput = false;
        FadeCanvasGroup(fadeTime, false);
    }
}
