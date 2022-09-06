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
    List<TrackManager.TrackSectionKey> ratTrackSections = new List<TrackManager.TrackSectionKey>() { TrackManager.TrackSectionKey.ChannelOne, TrackManager.TrackSectionKey.ChannelThree };

    float timeToNextItem;
    public Poolable itemPrefab;
    ObjectPool<GameObject> itemObjectPool;
    float itemYOffset = 0.25f;
    float maxItemWait = 8;
    float minItemWait = 3;
    List<TrackManager.TrackSectionKey> itemTrackSections = new List<TrackManager.TrackSectionKey>() { TrackManager.TrackSectionKey.RailOne, TrackManager.TrackSectionKey.RailTwo, TrackManager.TrackSectionKey.RailThree, TrackManager.TrackSectionKey.RailFour };

    float timeToNextPillar;
    public Poolable pillarPrefab;
    ObjectPool<GameObject> pillarObjectPool;
    float pillarYOffset = 4f;
    float pillarWait = 2;
    List<TrackManager.TrackSectionKey> pillarTrackSections = new List<TrackManager.TrackSectionKey>() { TrackManager.TrackSectionKey.ChannelTwo };

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
            GameObject rat = ratObjectPool.Get();
            rat.GetComponent<Poolable>().pool = ratObjectPool;

            TrackManager.TrackSectionKey trackSectionKey = ratTrackSections[Random.Range(0, ratTrackSections.Count)];
            TrackManager.TrackSection trackSection = TrackManager.instance.GetTrackSectionByKey(trackSectionKey);

            rat.transform.position = new Vector3(
                spawnXPosition,
                trackSection.yPosition + ratYOffset,
                0);
            TrackManager.instance.SetObjectLayerToMatchTrackSection(rat, trackSection);

            timeToNextRat = Random.Range(minRatWait, maxRatWait);
        }

        // Spawn item
        if (timeToNextItem <= 0)
        {
            GameObject item = itemObjectPool.Get();
            item.GetComponent<Poolable>().pool = itemObjectPool;

            TrackManager.TrackSectionKey trackSectionKey = itemTrackSections[Random.Range(0, itemTrackSections.Count)];
            TrackManager.TrackSection trackSection = TrackManager.instance.GetTrackSectionByKey(trackSectionKey);

            item.transform.position = new Vector3(
                spawnXPosition,
                trackSection.yPosition + itemYOffset,
                0);
            TrackManager.instance.SetObjectLayerToMatchTrackSection(item, trackSection);

            timeToNextItem = Random.Range(minItemWait, maxItemWait);
        }

        // Spawn pillar
        if (timeToNextPillar <= 0)
        {
            GameObject pillar = pillarObjectPool.Get();
            pillar.GetComponent<Poolable>().pool = pillarObjectPool;

            TrackManager.TrackSectionKey trackSectionKey = pillarTrackSections[Random.Range(0, pillarTrackSections.Count)];
            TrackManager.TrackSection trackSection = TrackManager.instance.GetTrackSectionByKey(trackSectionKey);

            pillar.transform.position = new Vector3(
                spawnXPosition,
                trackSection.yPosition + pillarYOffset,
                0);
            TrackManager.instance.SetObjectLayerToMatchTrackSection(pillar, trackSection);

            timeToNextPillar = pillarWait;
        }
    }

}
