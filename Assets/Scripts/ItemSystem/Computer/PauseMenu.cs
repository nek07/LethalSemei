using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject hub;
    public string mainMenuSceneName = "MainMenu";
    public MonoBehaviour playerControllerScript; // сюда перетаскиваешь скрипт игрока

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused){
                Resume();
                hub.SetActive(true);
        }
        else{
                Pause();
                hub.SetActive(false);
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerControllerScript != null)
            playerControllerScript.enabled = true;

        isPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerControllerScript != null)
            playerControllerScript.enabled = false;

        isPaused = true;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Выход из игры");
        Application.Quit();
    }
}