using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    private float playedTime = 0f;
    private int score = 0; // Player's score
    [SerializeField] private Text scoreText;
    [SerializeField] private Text timerText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensures persistence between scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        UpdateScoreUI();
        UpdateTimerUI();
    }

    void Update()
    {
        playedTime += Time.deltaTime; // Increment played time
        UpdateTimerUI();
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = Helper.FormatTime(playedTime);
        }
    }

    public int GetScore()
    {
        return score;
    }

    public float GetTimePlayed()
    {
        return playedTime;
    }
}
