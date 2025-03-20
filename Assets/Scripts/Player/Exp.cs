using UnityEngine;
using System;

public class Exp : MonoBehaviour
{
    public event Action<float> OnExpChanged; // Event for UI updates
    public event Action<int> OnLevelUp; // Event triggered on level up

    [SerializeField] private int level = 1;
    [SerializeField] private float maxExp = 100f;
    private float currentExp;

    private void Awake()
    {
        currentExp = 0;
        Debug.Log($"Level: {level}, Current EXP: {currentExp}/{maxExp}");
    }

    public void GainExp(float amount)
    {
        currentExp += amount;
        if (currentExp >= maxExp)
        {
            LevelUp();
        }

        OnExpChanged?.Invoke(currentExp / maxExp);
        Debug.Log($"Current EXP: {currentExp}/{maxExp}");
    }

    private void LevelUp()
    {
        level++;
        currentExp = 0;
        maxExp += 50; // Increase EXP needed for next level

        OnLevelUp?.Invoke(level);
        Debug.Log($"Leveled Up! New Level: {level}");
    }

    public int GetLevel() => level;
    public float GetCurrentExp() => currentExp;
    public float GetMaxExp() => maxExp;
}
