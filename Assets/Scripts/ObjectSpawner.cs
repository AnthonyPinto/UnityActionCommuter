using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectSpawner : MonoBehaviour
{

    float spawnXPosition = 20;

    public Poolable prefab;
    public ObjectPool<GameObject> objectPool;
    public float yOffsetAboveRail = 0.5f;

    private void Awake()
    {
        objectPool = ObjectPoolFactory.CreatePooler(prefab.gameObject);
    }


    public void SpawnObjectOnTrackSection(TrackManager.TrackSectionKey trackSectionKey)
    {
        GameObject newObject = objectPool.Get();
        newObject.GetComponent<Poolable>().pool = objectPool;
        TrackManager.TrackSection trackSection = TrackManager.instance.GetTrackSectionByKey(trackSectionKey);

        newObject.transform.position = new Vector3(
            spawnXPosition,
            trackSection.yPosition + yOffsetAboveRail,
            0);
        TrackManager.instance.SetObjectLayerToMatchTrackSection(newObject, trackSection);
    }
}
