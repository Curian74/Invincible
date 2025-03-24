using UnityEngine;

public class PlayerAware : MonoBehaviour
{
    public bool AwareOfPlayer { get; private set; }
    public Vector2 DirectionToPlayer { get; private set; }
    public Vector2 PlayerPosition => _player != null ? (Vector2)_player.position : Vector2.zero; 

    [SerializeField]
    private float _playerAwarenessDistance;

    private Transform _player;

    void Update()
    {
        FindNearestTurret();

        if (_player == null) 
        {
            AwareOfPlayer = false;
            return;
        }

        Vector2 enemyToTurretVector = _player.position - transform.position;
        DirectionToPlayer = enemyToTurretVector.normalized;

        AwareOfPlayer = enemyToTurretVector.magnitude <= _playerAwarenessDistance;
    }

    private void FindNearestTurret()
    {
        GameObject[] turrets = GameObject.FindGameObjectsWithTag("Player");

        if (turrets.Length == 0)
        {
            _player = null; 
            return;
        }

        Transform nearestTurret = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject turret in turrets)
        {
            float distance = Vector2.Distance(transform.position, turret.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestTurret = turret.transform;
            }
        }

       _player = nearestTurret;
    }
}
