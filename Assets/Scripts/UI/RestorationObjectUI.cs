using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RestorationObjectUI : MonoBehaviour
{
    [SerializeField]
    private RestorationObject parentRestorationObj;
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private CanvasGroup canvasGroup;

    [Space]
    [SerializeField]
    private float fadeTime = 1f;

    private PlayerCameraController playerCam;

    // Start is called before the first frame update
    void Start()
    {
        playerCam = FindObjectOfType<PlayerCameraController>();

        text.text = parentRestorationObj.soulfishNeededToRestore.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        LookAtCamera();
    }

    private void LookAtCamera()
    {
        Vector3 distanceToCam = playerCam.transform.position - transform.position;
        transform.LookAt(transform.position - distanceToCam);
    }

    public void FadeUI(bool fadeIn)
    {
        float alpha = fadeIn ? 1.0f : 0.0f;
        canvasGroup.DOFade(alpha, fadeTime);
    }
}
