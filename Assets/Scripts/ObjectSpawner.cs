using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectSpawner : MonoBehaviour
{

    float spawnXPosition = 20;

    // how often it checks whether to spawn one or more new objects
    float spawnTick = 0.5f;

    float timeToNextRat;
    public Poolable ratPrefab;
    ObjectPool<GameObject> ratObjectPool;
    float ratYOffset = 0.5f;
    float maxRatWait = 6;
    float minRatWait = 1;

    float timeToNextItem;
    public Poolable itemPrefab;
    ObjectPool<GameObject> itemObjectPool;
    float itemYOffset = 0.25f;
    float maxSodaWait = 8;
    float minSodaWait = 3;

    float timeToNextPillar;
    public Poolable pillarPrefab;
    ObjectPool<GameObject> pillarObjectPool;
    float pillarYOffset = 4f;
    float pillarWait = 2;

    private void Awake()
    {
        ratObjectPool = ObjectPoolFactory.CreatePooler(ratPrefab.gameObject);
        itemObjectPool = ObjectPoolFactory.CreatePooler(itemPrefab.gameObject);
        pillarObjectPool = ObjectPoolFactory.CreatePooler(pillarPrefab.gameObject);
    }


    private void Start()
    {
        StartCoroutine(SpawningRoutine());
    }

    private void Update()
    {
        timeToNextRat -= Time.deltaTime;
        timeToNextItem -= Time.deltaTime;
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
            // channel = space between 2 rails
            Constants.Layer channel = Random.value > 0.5 ? Constants.Layer.ChannelOne : Constants.Layer.ChannelThree;
            GameObject channelObject = TrackManager.instance.GetObjectForLayer(channel);

            GameObject rat = ratObjectPool.Get();
            rat.GetComponent<Poolable>().pool = ratObjectPool;
            rat.transform.position = new Vector3(
                spawnXPosition,
                channelObject.transform.position.y + ratYOffset,
                0);
            rat.gameObject.SetLayerRecursively(LayerMask.NameToLayer(Constants.LayerString[channel]));

            timeToNextRat = Random.Range(minRatWait, maxRatWait);
        }

        // Spawn item
        if (timeToNextItem <= 0)
        {
            Constants.Layer rail = Constants.RailList[Random.Range(0, Constants.RailList.Count)];
            GameObject railObject = TrackManager.instance.GetObjectForLayer(rail);

            GameObject item = itemObjectPool.Get();
            item.GetComponent<Poolable>().pool = itemObjectPool;
            item.transform.position = new Vector3(
                spawnXPosition,
                railObject.transform.position.y + itemYOffset,
                0);
            item.gameObject.SetLayerRecursively(LayerMask.NameToLayer(Constants.LayerString[rail]));

            timeToNextItem = Random.Range(minSodaWait, maxSodaWait);
        }

        // Spawn pillar
        if (timeToNextPillar <= 0)
        {
            Constants.Layer layer = Constants.ChannelList[1]; // Always the middle channel
            GameObject channelObject = TrackManager.instance.GetObjectForLayer(layer);

            GameObject pillar = pillarObjectPool.Get();
            pillar.GetComponent<Poolable>().pool = pillarObjectPool;
            pillar.transform.position = new Vector3(
                spawnXPosition,
                channelObject.transform.position.y + pillarYOffset,
                0);
            pillar.gameObject.SetLayerRecursively(LayerMask.NameToLayer(Constants.LayerString[layer]));

            timeToNextPillar = pillarWait;
        }
    }

}
