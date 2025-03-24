using UnityEngine;

public class PlayerHealth : Health
{
    [SerializeField] private GameOverScreen gameoverScreen;

    public void IncreaseHealth(float amount)
    {
        maxHealth += amount;
        currentHealth += amount;
    }

    protected override void Die()
    {
        gameoverScreen.Setup();
        base.Die();
    }

}
