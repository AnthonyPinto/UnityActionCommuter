using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectSpawner : MonoBehaviour
{
    float timeToNextRat;

    float spawnXPos = 20;


    public Poolable ratPrefab;
    ObjectPool<GameObject> ratObjectPool;
    float ratYOffset = 0.25f;
    float maxRatWait = 6;
    float minRatWait = 1;

    private void Awake()
    {
        ratObjectPool = PoolerFactory.CreatePooler(ratPrefab.gameObject);
    }


    private void Start()
    {
        StartCoroutine(SpawningRoutine());
    }

    private void Update()
    {
        timeToNextRat -= Time.deltaTime;
    }

    IEnumerator SpawningRoutine()
    {
        while (true)
        {
            SpawnObjects();
            yield return new WaitForSeconds(1);
        }
    }


    void SpawnObjects()
    {
        if (timeToNextRat <= 0)
        {
            Constants.Layer channel = Random.value > 0.5 ? Constants.Layer.ChannelOne : Constants.Layer.ChannelThree;
            GameObject channelObject = LayerHelper.instance.GetObjectForLayer(channel);

            GameObject rat = ratObjectPool.Get();
            rat.GetComponent<Poolable>().pool = ratObjectPool;
            rat.transform.position = new Vector3(spawnXPos, channelObject.transform.position.y + ratYOffset, 0);
            Debug.Log(LayerMask.NameToLayer(Constants.LayerString[channel]));
            rat.gameObject.layer = LayerMask.NameToLayer(Constants.LayerString[channel]);

            timeToNextRat = Random.Range(minRatWait, maxRatWait);
        }
    }

}
