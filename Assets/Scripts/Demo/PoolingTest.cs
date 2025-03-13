using UnityEngine;

public class PoolingTest : MonoBehaviour
{
    [SerializeField] private float spawnRate = 1;
    public Player playerPrefab;
    public PoolingManager<Player> playerPool;
    private float spawnRateTimer = 0;
    void Start()
    {
        playerPool = new PoolingManager<Player>(playerPrefab, 5, transform);
    }

    void Update()
    {
        spawnRateTimer += Time.deltaTime;
        if(spawnRateTimer >= spawnRate)
        {
            SpawnPlayer();
            spawnRateTimer = 0;
        }
    }

    public Player SpawnPlayer()
    {
        Player newPlayer = playerPool.GetObject();

        if (newPlayer == null)
        {
            return null;
        }

        Vector2 randomPosition = new Vector2(
            Random.Range(1, 4),
            Random.Range(1, 3)
        );

        newPlayer.transform.position = randomPosition;
        return newPlayer;
    }

    public void ReturnPlayer(Player player)
    {
        playerPool.BackToPool(player);
    }
}
