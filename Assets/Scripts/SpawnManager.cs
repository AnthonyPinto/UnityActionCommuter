using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    float spawnTick = 0.25f;

    // Where we initialize all of the 'rules' that dictate what is spawned on each tick
    List<ISpawnRule> spawnRules = new List<ISpawnRule>() { new ColumnRule(), new ItemRule() };
    Dictionary<TrackManager.TrackSectionKey, Dictionary<SpawnableType, float>> latestSpawnTimes;

    enum SpawnableType { Rat, Coffee, Column, Train, Sunglasses }
    List<SpawnableType> SpawnableTypeList = new List<SpawnableType>() { SpawnableType.Rat, SpawnableType.Coffee, SpawnableType.Column, SpawnableType.Train, SpawnableType.Sunglasses };

    public ObjectSpawner columnSpawner;
    public ObjectSpawner coffeeSpawner;
    public ObjectSpawner sunglassesSpawner;
    public ObjectSpawner ratSpawner;
    public ObjectSpawner trainSpawner;


    Dictionary<SpawnableType, ObjectSpawner> spawnerMap;

    private void Awake()
    {
        // Construct latestSpawnTimes map
        latestSpawnTimes = new Dictionary<TrackManager.TrackSectionKey, Dictionary<SpawnableType, float>>();
        foreach (TrackManager.TrackSectionKey trackSectionKey in TrackManager.TrackSectionKeyList)
        {
            Dictionary<SpawnableType, float> spawnTimesForTrackSection = new Dictionary<SpawnableType, float>();
            foreach (SpawnableType spawnableType in SpawnableTypeList)
            {
                spawnTimesForTrackSection.Add(spawnableType, float.NegativeInfinity);
            }
            latestSpawnTimes.Add(trackSectionKey, spawnTimesForTrackSection);
        }

        // Construct spawnerMap
        spawnerMap = new Dictionary<SpawnableType, ObjectSpawner> {
            {SpawnableType.Column, columnSpawner},
            {SpawnableType.Coffee, coffeeSpawner },
            {SpawnableType.Sunglasses, sunglassesSpawner },
            {SpawnableType.Rat, ratSpawner },
            {SpawnableType.Train, trainSpawner }

        };
    }

    private void Start()
    {
        StartCoroutine(SpawningRoutine());
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
        Dictionary<TrackManager.TrackSectionKey, SpawnableType?> nextSpawnMap = new Dictionary<TrackManager.TrackSectionKey, SpawnableType?>();
        // TODO: confirm if we need to instantiate all the keys in nextSpawnMap to avoid errors

        foreach (ISpawnRule rule in spawnRules)
        {
            nextSpawnMap = rule.Evaluate(latestSpawnTimes, nextSpawnMap);
        }

        foreach (TrackManager.TrackSectionKey trackSectionKey in TrackManager.TrackSectionKeyList)
        {
            SpawnableType? spawnable = nextSpawnMap.GetValueOrDefault(trackSectionKey, null);
            if (spawnable.HasValue)
            {
                SpawnObjectByType(spawnable.Value, trackSectionKey);
                latestSpawnTimes[trackSectionKey][spawnable.Value] = Time.time;
            }
        }
    }

    void SpawnObjectByType(SpawnableType spawnableType, TrackManager.TrackSectionKey trackSectionKey)
    {
        spawnerMap[spawnableType].SpawnObjectOnTrackSection(trackSectionKey);
    }



    interface ISpawnRule
    {
        public Dictionary<TrackManager.TrackSectionKey, SpawnableType?> Evaluate(
            Dictionary<TrackManager.TrackSectionKey, Dictionary<SpawnableType, float>> latestSpawnTimes,
            Dictionary<TrackManager.TrackSectionKey, SpawnableType?> nextSpawnMap

        );
    }

    class ColumnRule : ISpawnRule
    {
        float pillarWait = 2;
        TrackManager.TrackSectionKey columnTrackSection = TrackManager.TrackSectionKey.ChannelTwo;

        public Dictionary<TrackManager.TrackSectionKey, SpawnableType?> Evaluate(
            Dictionary<TrackManager.TrackSectionKey, Dictionary<SpawnableType, float>> latestSpawnTimes,
            Dictionary<TrackManager.TrackSectionKey, SpawnableType?> nextSpawnMap
        )
        {
            float timeSinceLastColumn = Time.time - latestSpawnTimes[columnTrackSection][SpawnableType.Column];
            if (timeSinceLastColumn >= pillarWait)
            {
                nextSpawnMap[columnTrackSection] = SpawnableType.Column;
            }
            return nextSpawnMap;
        }
    }

    class ItemRule : ISpawnRule
    {
        float minItemWait = 0.5f;
        float maxItemWait = 1;

        float waitForNextItem;

        float minSunglassesWait = 4;
        float maxSunglassesWait = 4;

        float waitForNextSungglasses;

        float timeAtLastTick;

        List<TrackManager.TrackSectionKey> allowedTrackSections = new List<TrackManager.TrackSectionKey>() {
            TrackManager.TrackSectionKey.RailOne,
            TrackManager.TrackSectionKey.RailTwo,
            TrackManager.TrackSectionKey.RailThree,
            TrackManager.TrackSectionKey.RailFour
        };



        public Dictionary<TrackManager.TrackSectionKey, SpawnableType?> Evaluate(
            Dictionary<TrackManager.TrackSectionKey, Dictionary<SpawnableType, float>> latestSpawnTimes,
            Dictionary<TrackManager.TrackSectionKey, SpawnableType?> nextSpawnMap
        )
        {
            float latestItemTime = float.NegativeInfinity;
            float latestSunglassesTime = float.NegativeInfinity;

            // If player already has sunglasses, keep pushing back the timing for the next ones
            // This way we don't immediately spawn new ones after the player loses them
            if (GameManager.instance.playerHasSunglasses)
            {
                waitForNextSungglasses += Time.time - timeAtLastTick;
            }

            foreach (TrackManager.TrackSectionKey trackSectionKey in TrackManager.TrackSectionKeyList)
            {
                latestItemTime = Mathf.Max(latestItemTime, latestSpawnTimes[trackSectionKey][SpawnableType.Sunglasses]);
                latestItemTime = Mathf.Max(latestItemTime, latestSpawnTimes[trackSectionKey][SpawnableType.Coffee]);

                latestSunglassesTime = Mathf.Max(latestSunglassesTime, latestSpawnTimes[trackSectionKey][SpawnableType.Sunglasses]);
            }

            // If we have finished the waitfornextitem add something
            if (
                Time.time - latestItemTime >= waitForNextItem
            )
            {
                waitForNextItem = Random.Range(minItemWait, maxItemWait); // no matter what item we spawn push out next item wait
                TrackManager.TrackSectionKey trackSectionKey = allowedTrackSections[Random.Range(0, allowedTrackSections.Count)];

                if (Time.time - latestSunglassesTime >= waitForNextSungglasses)
                {
                    nextSpawnMap[trackSectionKey] = SpawnableType.Sunglasses;
                    waitForNextSungglasses = Random.Range(minSunglassesWait, maxSunglassesWait);
                }
                else
                {
                    nextSpawnMap[trackSectionKey] = SpawnableType.Coffee;
                }
            }
            timeAtLastTick = Time.time;
            return nextSpawnMap;
        }
    }
}
