using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/*
 * Handles pausing and unpausing and displaying menu panels
 * 
 * Jeff Stevenson
 * 4.25.25
 */

public class MenuManager : MonoBehaviour
{
    // name of main menu scene
    private const string MAIN_MENU_STR_REF = "MainMenu";
    private const string GAME_START_SCENE = "TJSceneUpdatedbyDev";

    public static bool IsGamePaused;

    [SerializeField]
    private bool isMainMenu;

    [SerializeField]
    private PlayerControls playerControls;

    [SerializeField]
    private MenuPanel[] pausePanels;
    [SerializeField]
    private MenuPanel defaultPanel;

    private MenuPanel currentPanel;
    private CanvasGroup menuCanvasGroup;
    private bool isPauseHeld;

    // Start is called before the first frame update
    void Start()
    {
        menuCanvasGroup = GetComponent<CanvasGroup>();

        if (!isMainMenu)
        {
            foreach (var panel in pausePanels)
            {
                panel.SetPanelActive(false);
            }

            Unpause();
        }
        else
        {
            currentPanel = defaultPanel;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMainMenu)
        {
            if (playerControls.PausePressed && !isPauseHeld)
            {
                TogglePause();
                isPauseHeld = true;
            }
            else if (!playerControls.PausePressed)
            {
                isPauseHeld = false;
            }
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        menuCanvasGroup.alpha = 1f;
        menuCanvasGroup.interactable = true;
        menuCanvasGroup.blocksRaycasts = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SwitchToPanel(defaultPanel);
        IsGamePaused = true;
    }

    public void Unpause()
    {
        currentPanel?.SetPanelActive(false);
        menuCanvasGroup.alpha = 0f;
        menuCanvasGroup.interactable = false;
        menuCanvasGroup.blocksRaycasts = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        IsGamePaused = false;
    }

    public void TogglePause()
    {
        if (IsGamePaused) Unpause();
        else Pause();
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene(MAIN_MENU_STR_REF);
    }    

    public void NewGame()
    {
        SceneManager.LoadScene(GAME_START_SCENE);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SwitchToPanel(int panelIndex)
    {
        SwitchToPanel(pausePanels[panelIndex]);
    }

    public void SwitchToPanel(MenuPanel menuPanel)
    {
        currentPanel?.SetPanelActive(false);
        currentPanel = menuPanel;
        currentPanel.SetPanelActive(true);
    }
}
