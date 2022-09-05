using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectSpawner : MonoBehaviour
{


    float spawnXPos = 20;

    float spawnTick = 0.5f;

    float timeToNextRat;
    public Poolable ratPrefab;
    ObjectPool<GameObject> ratObjectPool;
    float ratYOffset = 0.5f;
    float maxRatWait = 6;
    float minRatWait = 1;

    float timeToNextSoda;
    public Poolable sodaPrefab;
    ObjectPool<GameObject> sodaObjectPool;
    float sodaYOffset = 0.25f;
    float maxSodaWait = 8;
    float minSodaWait = 3;

    float timeToNextPillar;
    public Poolable pillarPrefab;
    ObjectPool<GameObject> pillarObjectPool;
    float pillarYOffset = 4f;
    float pillarWait = 2;

    private void Awake()
    {
        ratObjectPool = PoolerFactory.CreatePooler(ratPrefab.gameObject);
        sodaObjectPool = PoolerFactory.CreatePooler(sodaPrefab.gameObject);
        pillarObjectPool = PoolerFactory.CreatePooler(pillarPrefab.gameObject);
    }


    private void Start()
    {
        StartCoroutine(SpawningRoutine());
    }

    private void Update()
    {
        timeToNextRat -= Time.deltaTime;
        timeToNextSoda -= Time.deltaTime;
        timeToNextPillar -= Time.deltaTime;
    }

    IEnumerator SpawningRoutine()
    {
        while (true)
        {
            SpawnObjects();
            yield return new WaitForSeconds(spawnTick);
        }
    }


    void SpawnObjects()
    {
        // Spawn rat
        if (timeToNextRat <= 0)
        {
            Constants.Layer channel = Random.value > 0.5 ? Constants.Layer.ChannelOne : Constants.Layer.ChannelThree;
            GameObject channelObject = LayerHelper.instance.GetObjectForLayer(channel);

            GameObject rat = ratObjectPool.Get();
            rat.GetComponent<Poolable>().pool = ratObjectPool;
            rat.transform.position = new Vector3(spawnXPos, channelObject.transform.position.y + ratYOffset, 0);
            rat.gameObject.SetLayerRecursively(LayerMask.NameToLayer(Constants.LayerString[channel]));

            timeToNextRat = Random.Range(minRatWait, maxRatWait);
        }

        // Spawn soda
        if (timeToNextSoda <= 0)
        {
            Constants.Layer rail = Constants.RailList[Random.Range(0, Constants.RailList.Count)];
            GameObject railObject = LayerHelper.instance.GetObjectForLayer(rail);

            GameObject soda = sodaObjectPool.Get();
            soda.GetComponent<Poolable>().pool = sodaObjectPool;
            soda.transform.position = new Vector3(spawnXPos, railObject.transform.position.y + sodaYOffset, 0);
            soda.gameObject.SetLayerRecursively(LayerMask.NameToLayer(Constants.LayerString[rail]));

            timeToNextSoda = Random.Range(minSodaWait, maxSodaWait);
        }

        // Spawn pillar
        if (timeToNextPillar <= 0)
        {
            Constants.Layer layer = Constants.ChannelList[1]; // Always the middle channel
            GameObject channelObject = LayerHelper.instance.GetObjectForLayer(layer);

            GameObject pillar = pillarObjectPool.Get();
            pillar.GetComponent<Poolable>().pool = pillarObjectPool;
            pillar.transform.position = new Vector3(spawnXPos, channelObject.transform.position.y + pillarYOffset, 0);
            pillar.gameObject.SetLayerRecursively(LayerMask.NameToLayer(Constants.LayerString[layer]));

            timeToNextPillar = pillarWait;
        }
    }

}
