using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MechaGolemAttacks : MonoBehaviour
{
    [SerializeField] private Rock _rockPrefab;
    [SerializeField] private int _rockPool = 5;
    [SerializeField] Transform _armFirePoint;
    private Transform _player;
    private Animator _animator;
    private PoolingManager<Rock> _poolingManager;
    private bool _isPerformingAction = false;
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
            //case 1:
            //    yield return StartCoroutine(ThrowRock());
            //    break;
            //case 2:
            //    yield return StartCoroutine(Defend());
            //    break;
            //case 3:
            //    yield return StartCoroutine(ShootLaser());
            //    break;

            case 1:
                yield return StartCoroutine(Defend());
                break;
            case 2:
                yield return StartCoroutine(Defend());
                break;
            case 3:
                yield return StartCoroutine(Defend());
                break;
        }
         yield return new WaitForSeconds(3f);
        _isPerformingAction = false;
    }

    private IEnumerator ThrowRock()
    {
        var rock = _poolingManager.GetObject();
        if (rock != null)
        {
            if(_player.transform.position.x > transform.position.x)
            {
                transform.localScale = new Vector3(6, 6, 1);
            }
            else
            {
                {
                    transform.localScale = new Vector3(-6, 6, 1);
                }
            }
            _animator.SetTrigger("Throw");
            rock.transform.position = _armFirePoint.position;
            rock.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            _animator.SetTrigger("Idle");     
        }
    }

    private IEnumerator Defend()
    {
        _animator.SetTrigger("Defend");
        yield return new WaitForSeconds(2f);
        _animator.SetTrigger("Idle");
    }

    private IEnumerator ShootLaser()
    {
        _animator.SetTrigger("Laser");
        yield return new WaitForSeconds(1.5f);
        _animator.SetTrigger("Idle");
    }

    private int GetRandomDecision()
    {
        return Random.Range(1, 4);
    }
}
