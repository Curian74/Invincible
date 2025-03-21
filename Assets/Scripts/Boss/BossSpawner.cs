using UnityEngine;
using UnityEngine.UI;

public class BossSpawner : MonoBehaviour
{
	[SerializeField]private GameObject _bringerOfDeathPrefab;
	//[SerializeField]private Transform _bringerOfDeath;
    private ScoreManager _scoreManager;
	private Transform _player;
	private GameObject _bossHealthBar;
	void Awake()
    {
        _scoreManager = ScoreManager.Instance;
        _player = GameObject.FindWithTag("Player").transform;

	}

 
    void Update()
    {
		Vector3 offSet = new Vector3(7, 7, 0);
		var playTime = _scoreManager.GetTimePlayed();
		if (playTime >= 10f && Mathf.FloorToInt(playTime) % 10 == 0 && !_bringerOfDeathPrefab.gameObject.activeSelf)
		{
			_bringerOfDeathPrefab.transform.position = _player.position + offSet;
			var maxHealth = _bringerOfDeathPrefab.GetComponent<Health>().GetMaxHealth();
			_bringerOfDeathPrefab.GetComponent<Health>().Heal(maxHealth);
			_bringerOfDeathPrefab.gameObject.SetActive(true);
		}
	}
}
