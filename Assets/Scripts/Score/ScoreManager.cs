using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    private float playedTime = 0f;
    private int score = 0; // Player's score
    private BossSpawner _bossSpawner;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text gameoverScoreText;
    [SerializeField] private Text timerText;
    [SerializeField] private Text barrageText;
    [SerializeField] private Text hpText;
    [SerializeField] private Text speedText;
    [SerializeField] private Text hyperVelocityText;
    [SerializeField] private Text infernoText;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private int _spawnTime = 10;
    private int _lastSpawnTime = -1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        _bossSpawner = FindFirstObjectByType<BossSpawner>();


    }

    void Start()
    {
        UpdateScoreUI();
        UpdateTimerUI();
        UpdateUpgradeLevelUI();
    }

    void Update()
    {
        playedTime += Time.deltaTime; 
        UpdateTimerUI();

        int currentTime = Mathf.FloorToInt(playedTime);

        if (playedTime >= 10 && currentTime % _spawnTime == 0 && _lastSpawnTime != currentTime)
        {
            _bossSpawner.spawnBoss();  
            _lastSpawnTime = currentTime; 
        }
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
            gameoverScoreText.text = $"Score: {score}";
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            Debug.Log("timer txt not null");
            timerText.text = Helper.FormatTime(playedTime);
        }
        else
        {
            Debug.Log("timer txt null");
        }
    }

    public void UpdateUpgradeLevelUI()
    {
        barrageText.text = playerStats.GetUpgradeCount(UpgradeOption.UpgradeType.BulletLifeTime).ToString();
        hpText.text = playerStats.GetUpgradeCount(UpgradeOption.UpgradeType.Health).ToString();
        speedText.text = playerStats.GetUpgradeCount(UpgradeOption.UpgradeType.Speed).ToString();
        hyperVelocityText.text = playerStats.GetUpgradeCount(UpgradeOption.UpgradeType.BulletSpeed).ToString();
        infernoText.text = playerStats.GetUpgradeCount(UpgradeOption.UpgradeType.Damage).ToString();
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
