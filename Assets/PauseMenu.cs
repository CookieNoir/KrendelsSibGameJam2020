using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isGamePaused = false;
    public GameObject pauseMenuUI;
    public GameObject pauseButton;
    public UiMovement items;
    public UiMovement timer;
    public UiMovement tasks;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeState();
        }
    }

    public void ChangeState()
    {
        if (isGamePaused)
        {
            pauseMenuUI.SetActive(false);
            pauseButton.SetActive(true);
            Time.timeScale = 1f;
            isGamePaused = false;
        }
        else
        {
            pauseMenuUI.SetActive(true);
            pauseButton.SetActive(false);
            Time.timeScale = 0f;
            isGamePaused = true;
        }
        items.Translate();
        timer.Translate();
        tasks.Translate();
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Quiting game ...");
        Application.Quit();
    }
}
