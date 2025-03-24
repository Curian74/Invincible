using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private Text scoreDisplay;
    public void Setup()
    {
        gameObject.SetActive(true);
        var score = FindAnyObjectByType<ScoreDisplay>().score;
        scoreDisplay.text = $"Score: {score}";
        Time.timeScale = 0;
    }

    public void RestartButton()
    {
        SceneManager.LoadScene("DemoScene2");
        Time.timeScale = 1;
    }
    public void MainMenuButton()
    {
        SceneManager.LoadScene("StartMenu");
        Time.timeScale = 1;
    }

}
