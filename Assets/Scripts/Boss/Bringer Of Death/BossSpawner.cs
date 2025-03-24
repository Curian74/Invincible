using UnityEngine;
using UnityEngine.UI;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _bringerOfDeathPrefab;
    [SerializeField] private GameObject _mechanicGolemPrefab;
    [SerializeField] private AudioClip _spawnSound;

    private ScoreManager _scoreManager;
    private Transform _player;
    private GameObject _currentBoss; 
    private int _count;

    void Awake()
    {
        _scoreManager = ScoreManager.Instance;
        _player = GameObject.FindWithTag("Player").transform;
        _count = 0;
    }

    public void spawnBoss()
    {
        if (_currentBoss != null && _currentBoss.activeSelf)
        {
            Debug.Log("A boss is already active, cannot spawn another.");
            return;
        }

        Vector3 offSet = new Vector3(7, 7, 0);

        if (_count % 2 == 0)
        {
            _currentBoss = _bringerOfDeathPrefab; 
        }
        else
        {
            _currentBoss = _mechanicGolemPrefab;
        }

        if (!_currentBoss.activeSelf)
        {
            _currentBoss.GetComponent<Rigidbody2D>().simulated = true;
            _currentBoss.transform.position = _player.position + offSet;
            var maxHealth = _currentBoss.GetComponent<Health>().GetMaxHealth();
            _currentBoss.GetComponent<Health>().Heal(maxHealth);
            SoundManager.Instance.PlaySFX(_spawnSound);
            _currentBoss.SetActive(true);
            _count++;
        }
    }
}
