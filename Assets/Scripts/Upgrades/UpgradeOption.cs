using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Upgrade/Upgrade Option")]
public class UpgradeOption : ScriptableObject
{
    public string upgradeName;
    public string description;
    public Sprite icon;
    public Sprite backgroundColor;
    public UpgradeType type;
    public float value;

    public enum UpgradeType
    {
        Health, Damage, Speed, BulletLifeTime, BulletSpeed
    }

    public void Apply(PlayerStats playerStats)
    {
        switch (type)
        {
            case UpgradeType.Health:
                Debug.Log("Health upgrade");
                playerStats.IncreaseHealth(value);
                break;
            case UpgradeType.Damage:
                playerStats.IncreaseDamage(value);
                break;
            case UpgradeType.Speed:
                playerStats.IncreaseSpeed(value);
                break;
            case UpgradeType.BulletLifeTime:
                playerStats.IncreaseBulletLifeTime(value);
                break;

            case UpgradeType.BulletSpeed:
                playerStats.IncreaseBulletSpeed(value);
                break;
                //case UpgradeType.FireRate:
                //    playerStats.IncreaseFireRate(value);
                //    break;
        }

        Debug.Log($"{upgradeName} chosen {playerStats.GetUpgradeCount(type)} times.");

    }

}
