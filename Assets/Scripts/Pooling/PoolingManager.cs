using System.Collections.Generic;
using UnityEngine;

public class PoolingManager<T> where T : MonoBehaviour
{
    private List<T> pool = new List<T>();
    private readonly T _prefab;
    private Transform _parent;

    public PoolingManager(T prefab, int initSize, Transform parent = null)
    {
        _prefab = prefab;
        _parent = parent;

        for (int i = 0; i < initSize; i++)
        {
            AddToPool();
        }
    }

    private T AddToPool()
    {
        T obj = Object.Instantiate(_prefab, _parent);
        obj.gameObject.SetActive(false);
        pool.Add(obj);
        return obj;
    }

    public T GetObject()
    {
        foreach (T obj in pool)
        {
            if (!obj.gameObject.activeInHierarchy)
            {
                obj.gameObject.SetActive(true);
                return obj;
            }
        }

        return null;
    }

    public void BackToPool(T obj)
    {
        obj.gameObject.SetActive(false);
    }
}
