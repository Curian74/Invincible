using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    [SerializeField] private float spawnRate = 1;
    private PoolingTest playerPool;
    private Rigidbody2D rb;
    private float inputX;
    private float inputY;
    private float spawnRateTimer = 0;
    
    void Start()
    {
        playerPool = FindFirstObjectByType<PoolingTest>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        spawnRateTimer += Time.deltaTime;

        if(spawnRateTimer >= spawnRate)
        {
            playerPool.SpawnPlayer();
            spawnRateTimer = 0;
        }

        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        rb.linearVelocity = new Vector2(inputX * speed, inputY * speed);
    }

    private void BackToPool()
    {
        playerPool.ReturnPlayer(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        BackToPool();
    }

}
