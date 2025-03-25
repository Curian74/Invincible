using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private Text scoreDisplay;
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button mainMenuBtn;

    void Start()
    {
        restartBtn.onClick.AddListener(Restart);
        mainMenuBtn.onClick.AddListener(MainMenu);
        //StartCoroutine(Delay(1));
        gameObject.SetActive(false);
    }

    private IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    public void Setup()
    {
        Time.timeScale = 0;
        gameObject.SetActive(true);
        var score = FindAnyObjectByType<ScoreDisplay>().score;
        scoreDisplay.text = $"Score: {score}";
    }

    public void Restart()
    {
        SceneManager.LoadScene("DemoScene2");
        Time.timeScale = 1;
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("StartMenu");
        Time.timeScale = 1;
    }

}
