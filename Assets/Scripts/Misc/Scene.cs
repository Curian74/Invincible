using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scene : MonoBehaviour
{
    [SerializeField] private Button startBtn;
    void Start()
    {
        startBtn.onClick.AddListener(StartGame);
    }

    void Update()
    {
        
    }

    private void StartGame()
    {
        SceneManager.LoadScene("DemoScene2");
    }
}
