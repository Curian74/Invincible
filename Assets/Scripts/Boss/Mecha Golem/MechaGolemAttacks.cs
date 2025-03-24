using NUnit.Framework;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MechaGolemAttacks : MonoBehaviour
{
    [SerializeField] private Rock _rockPrefab;
    [SerializeField] private int _rockPool = 15;
    [SerializeField] Transform _armFirePoint;
    [SerializeField] Transform _bodyFirePoint;
    [SerializeField] private AudioClip _throwRock;
    [SerializeField] private AudioClip _lazer;
    [SerializeField] private AudioClip _defend;
    [SerializeField] private AudioClip _explode;
    private Transform _player;
    private Animator _animator;
    private PoolingManager<Rock> _poolingManager;
    private bool _isPerformingAction = false;
    private int _polygonMissles = 15;
    private float _startAngle = 0f;
    void Awake()
    {
        _player = GameObject.FindWithTag("Player").transform;
        _animator = GetComponent<Animator>();
        _poolingManager = new PoolingManager<Rock>(_rockPrefab, _rockPool, null);
    }

    
    void Update()
    {
        if (_player != null && _player.gameObject.activeInHierarchy)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
            if (!_isPerformingAction)
            {
                StartCoroutine(PerformRandomAttack(GetRandomDecision()));
            }
        }
        
    }

    private IEnumerator PerformRandomAttack(int decision)
    {
        _isPerformingAction = true;
        switch (decision)
        {
            case 1:
                yield return StartCoroutine(ThrowRock());
                break;
            case 2:
                yield return StartCoroutine(Defend());
                break;
            case 3:
                float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
                if (distanceToPlayer < 10f)
                yield return StartCoroutine(ShootLaser());
                break;

        }
         yield return new WaitForSeconds(2.5f);
        _isPerformingAction = false;
    }

    private IEnumerator ThrowRock()
    {      
        for (int i = 0; i < 3; i++)
        {
            var rock = _poolingManager.GetObject();
            if (rock != null)
            {
                if (_player.transform.position.x > transform.position.x)
                {
                    transform.localScale = new Vector3(8, 8, 1);
                }
                else
                {
                        transform.localScale = new Vector3(-8, 8, 1);
                }
                _animator.SetTrigger("Throw");
                rock.transform.position = _armFirePoint.position;
                rock._isHoming = true;
                rock.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.5f);
            }
        }
        SetIdle();
    }

    private IEnumerator Defend()
    {
        _animator.SetTrigger("Defend");
        float angleStep = 360f / _polygonMissles;  
        yield return new WaitForSeconds(1.3f);
        for (int i = 0; i < _polygonMissles; i++)
        {
            var rock = _poolingManager.GetObject();
            rock._isHoming = false;
            if (rock != null)
            {
                rock.transform.position = _bodyFirePoint.position;               
                float angle = _startAngle + (i * angleStep);
                rock.SetDirection(angle);
            }
        }
        SetIdle();
    }

    private void SetIdle()
    {
        _animator.SetTrigger("Idle");
    }
    private IEnumerator ShootLaser()
    {
        _animator.SetTrigger("Laser");
        yield return new WaitForSeconds(2f);
        SetIdle();
    }

    private int GetRandomDecision()
    {
        return Random.Range(1, 4);
    }
}
