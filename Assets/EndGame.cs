using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    public GameObject GameOverMenuUI;
    public Text scoreText;
    public Text ScoreValue;
    public UiMovement timer;


    void Update()
    {
        ScoreValue.text = scoreText.text;
    }


    public void ReturnGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Debug.Log("Quiting game ...");
        Application.Quit();
    }
}
