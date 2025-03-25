using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EggHealth : EnemyHealth
{
    public enum EnemyType { Melee, Range, Suicide, Egg }
    [SerializeField] private EnemyType _enemyType;

    private Spawner _spawner;

    private void Start()
    {
        _spawner = FindAnyObjectByType<Spawner>(); 
    }

    public EnemyType GetEnemyType() => _enemyType;

    protected override void Die()
    {
        if (_spawner != null)
        {
            _spawner.BurstSpawnOnDeath(transform.position, _enemyType);
        }

        base.Die();
    }
}
