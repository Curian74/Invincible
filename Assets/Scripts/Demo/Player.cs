using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    private Rigidbody2D rb;
    // private PoolingTest poolingTest;
    private float inputX;
    private float inputY;
    
    void Start()
    {
        // poolingTest = FindFirstObjectByType<PoolingTest>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        rb.linearVelocity = new Vector2(inputX * speed, inputY * speed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // poolingTest.ReturnPlayer(this); 
    }

}
