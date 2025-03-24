using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    [SerializeField] private float speed = 2f;
    public int score = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float h = Mathf.PingPong(Time.time * speed, 1f);
        scoreText.color = Color.HSVToRGB(h, 1f, 1f);
    }

    public void IncrementScore(int amount)
    {
        score += amount; 
        scoreText.text = $"Score: {score}";
    }
}
