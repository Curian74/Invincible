using System.Collections;
using UnityEngine;

public class KeyboardMovements : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private float dashSpeedMultiplier = 5f;
    [SerializeField] private float dashDuration = 0.075f;
    [SerializeField] private float dashCooldown = 2.0f;

    private Rigidbody2D rb;
    private Animator anim;
    private float inputX;
    private float inputY;
    private bool isDashing = false;
    private bool canDash = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        Vector2 moveDirection = new(inputX, inputY);

        if (moveDirection.magnitude > 0)
        {
            anim.SetBool("run", true);
            moveDirection.Normalize();
        }
        else
        {
            anim.SetBool("run", false);
        }

        if (!isDashing)
        {
            rb.linearVelocity = moveDirection * playerStats.speed;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash(moveDirection));
        }
    }

    private IEnumerator Dash(Vector2 dashDirection)
    {
        if (dashDirection == Vector2.zero) yield break;

        isDashing = true;
        canDash = false;

        rb.linearVelocity = dashSpeedMultiplier * playerStats.speed * dashDirection;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        rb.linearVelocity = dashDirection * playerStats.speed;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
