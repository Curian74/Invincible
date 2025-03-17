using UnityEngine;

public class ConsumePowerup : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Powerup powerup = other.GetComponent<Powerup>();
        if (powerup != null)
        {
            Debug.Log($"Picked up: {other.gameObject.name}");
            powerup.Activate(gameObject);
        }
        else
        {
            Debug.Log("No power-up script found.");
        }
    }
}
