using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * Handles canvas that fades in and out when a trigger is entered/exited
 * 
 */

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(Collider))]
public class CanvasFadeTrigger : MonoBehaviour
{
    [SerializeField]
    private float fadeTime;

    [SerializeField]
    private bool pauseInputUntilClick;
    [SerializeField]
    [Tooltip("Optional - will switch to a passed-in camera if pause input is enabled to highlight a given object/view.")]
    private CinemachineVirtualCamera highlightCamera;

    private CanvasGroup canvasGroup;
    private PlayerStateController player;
    
    void Start()
    {
        player = FindObjectOfType<PlayerStateController>();
        canvasGroup = GetComponent<CanvasGroup>();
        FadeCanvasGroup(0, false);
    }

    private void FadeCanvasGroup(float fadeTime, bool fadeIn)
    {
        canvasGroup.DOFade(1f, fadeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FadeCanvasGroup(fadeTime, true);
            if (pauseInputUntilClick)
            {
                StartCoroutine(WaitForClickRoutine());
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

        if (highlightCamera != null)
        {
            highlightCamera.Priority = 5;
        }

        while (!player.Controls.CommandPressed && !isClickHeld)
        {
            isClickHeld = player.Controls.CommandPressed;
            yield return null;
        }
        FadeCanvasGroup(fadeTime, false);
    }
}
