using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class MenuPanel : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // activates/deactivates panel canvas group
    public void SetPanelActive(bool activate)
    {
        canvasGroup.alpha = activate ? 1.0f : 0f;
        canvasGroup.interactable = activate;
        canvasGroup.blocksRaycasts = activate;
    }


}
