using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Poolable : MonoBehaviour
{
    public ObjectPool<GameObject> pool;

    public void Release()
    {
        pool.Release(gameObject);
    }

}
