using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public static class ObjectPoolFactory
{
    public static ObjectPool<GameObject> CreatePooler(GameObject prefab)
    {
        PoolHandler poolHandler = new PoolHandler(prefab);

        return new ObjectPool<GameObject>(poolHandler.OnCreate, poolHandler.OnGet, poolHandler.OnRelease);
    }


    private class PoolHandler
    {

        GameObject prefab;

        public PoolHandler(GameObject newPrefab)
        {
            prefab = newPrefab;
        }

        public GameObject OnCreate()
        {
            return GameObject.Instantiate(prefab);
        }

        public void OnGet(GameObject obj)
        {
            obj.gameObject.SetActive(true);
        }

        public void OnRelease(GameObject obj)
        {
            obj.gameObject.SetActive(false);
        }
    }
}
