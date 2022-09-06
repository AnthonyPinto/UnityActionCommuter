using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectSpawner : MonoBehaviour
{

    float spawnXPosition = 20;

    // how often it checks whether to spawn one or more new objects
    float spawnTick = 0.5f;
    float timeToNextSpawn;

    public Poolable prefab;
    public ObjectPool<GameObject> objectPool;
    public float yOffsetAboveRail = 0.5f;
    public float maxWait = 6;
    public float minWait = 1;
    public List<TrackManager.TrackSectionKey> enabledTrackSections;

    private void Awake()
    {
        objectPool = ObjectPoolFactory.CreatePooler(prefab.gameObject);
    }


    private void Start()
    {
        StartCoroutine(SpawningRoutine());
    }

    private void Update()
    {
        timeToNextSpawn -= Time.deltaTime;
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
        // Spawn newObject
        if (timeToNextSpawn <= 0)
        {
            GameObject newObject = objectPool.Get();
            newObject.GetComponent<Poolable>().pool = objectPool;

            TrackManager.TrackSectionKey trackSectionKey = enabledTrackSections[Random.Range(0, enabledTrackSections.Count)];
            TrackManager.TrackSection trackSection = TrackManager.instance.GetTrackSectionByKey(trackSectionKey);

            newObject.transform.position = new Vector3(
                spawnXPosition,
                trackSection.yPosition + yOffsetAboveRail,
                0);
            TrackManager.instance.SetObjectLayerToMatchTrackSection(newObject, trackSection);

            timeToNextSpawn = Random.Range(minWait, maxWait);
        }
    }

}
