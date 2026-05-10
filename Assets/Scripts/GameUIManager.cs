using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject pauseMenuPanel;
     public GameObject endGame;

    private bool isPaused = false;

    void Start()
    { 
        mainMenuPanel.SetActive(true);
        pauseMenuPanel.SetActive(false);
         Time.timeScale = 0f;  
        Cursor.lockState = CursorLockMode.None;  
        Cursor.visible = false;
    }


    void Update()
    {
        // U¿ywamy Keyboard.current zamiast Input.GetKeyDown
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    public void StartGame()
    {
        mainMenuPanel.SetActive(false);

        // ZATRZYMAJ GRÊ I W£¥CZ STEROWANIE
        Time.timeScale = 1f;

        // KURSOR SCHOWANY I ZABLOKOWANY NA ŒRODKU
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ShowGameOver()
    {
        endGame.SetActive(true);

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenuPanel.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f;
            // W pauzie kursor MA BYÆ widoczny, ¿ebyœ móg³ nawigowaæ W/S
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
             Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}