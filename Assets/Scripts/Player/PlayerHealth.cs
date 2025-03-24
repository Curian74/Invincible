using UnityEngine;

public class PlayerHealth : Health
{
    public void IncreaseHealth(float amount)
    {
        maxHealth += amount;
        currentHealth += amount;
    }

}
