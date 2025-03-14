using UnityEngine;

public class MouseLook : MonoBehaviour
{
    void Update()
    {
        // Get mouse position in world space
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f; // Ensure it's on the same plane as the player

        // Calculate direction from player to mouse
        Vector3 direction = mousePos - transform.position;

        // Calculate angle relative to the upward direction (Vector2.up)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply rotation
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
