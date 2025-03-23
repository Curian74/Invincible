using UnityEngine;
using UnityEngine.UI;

public class BossSpawner : MonoBehaviour
{
	[SerializeField]private GameObject _bringerOfDeathPrefab;
    [SerializeField] private AudioClip _spawnSound;   
    private Rigidbody2D _rb;
    private ScoreManager _scoreManager;
	private Transform _player;
	private GameObject _bossHealthBar;
	void Awake()
    {
        _scoreManager = ScoreManager.Instance;
        _player = GameObject.FindWithTag("Player").transform;
        _rb = GetComponent<Rigidbody2D>();
	}

    public void spawnBoss()
    {
        Vector3 offSet = new Vector3(7, 7, 0);
        if (!_bringerOfDeathPrefab.gameObject.activeSelf)
        {
            _bringerOfDeathPrefab.gameObject.GetComponent<Rigidbody2D>().simulated = true;
            _bringerOfDeathPrefab.transform.position = _player.position + offSet;
            var maxHealth = _bringerOfDeathPrefab.GetComponent<Health>().GetMaxHealth();
            _bringerOfDeathPrefab.GetComponent<Health>().Heal(maxHealth);
            SoundManager.Instance.PlaySFX(_spawnSound);
            _bringerOfDeathPrefab.gameObject.SetActive(true);
        }
    }
}
