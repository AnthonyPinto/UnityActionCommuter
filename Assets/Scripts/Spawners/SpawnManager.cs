using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    float spawnTick = 0.25f;

    // Where we initialize all of the 'rules' that dictate what is spawned on each tick
    List<ISpawnRule> spawnRules;
    Dictionary<TrackManager.TrackSectionKey, Dictionary<SpawnableType, float>> latestSpawnTimes;

    enum SpawnableType { Rat, Coffee, Column, TrainFromFront, TrainFromBehind, Sunglasses, Paper }
    List<SpawnableType> SpawnableTypeList = new List<SpawnableType>() { SpawnableType.Rat, SpawnableType.Coffee, SpawnableType.Column, SpawnableType.TrainFromFront, SpawnableType.TrainFromBehind, SpawnableType.Sunglasses, SpawnableType.Paper };

    public ObjectSpawner columnSpawner;
    public ObjectSpawner coffeeSpawner;
    public ObjectSpawner sunglassesSpawner;
    public ObjectSpawner ratSpawner;
    public ObjectSpawner trainFromFront;
    public ObjectSpawner trainFromBehind;
    public ObjectSpawner paperSpawner;

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
            {SpawnableType.TrainFromFront, trainFromFront },
            {SpawnableType.TrainFromBehind, trainFromBehind },
            {SpawnableType.Paper, paperSpawner }

        };

        // Construct spawnRules
        // NOTE order matters
        spawnRules = new List<ISpawnRule>() {
            // NOTE Uncomment to enable columns
            //new ColumnRule(),
            new EnemyRule(),
            new ItemRule()
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
        float lastCoffeeTime = float.NegativeInfinity;
        float currentCoffeeWait;
        float minCoffeeWait = 2f;
        float maxCoffeeWait = 2;

        float lastSunglassesTime = float.NegativeInfinity;
        float currentSunglassesWait;
        float minSunglassesWait = 5;
        float maxSunglassesWait = 5;

        float lastPaperTime = float.NegativeInfinity;
        float currentPaperWait;
        float minPaperWait = 7;
        float maxPaperWait = 7;

        float timeAtLastTick;

        List<TrackManager.TrackSectionKey> allowedTrackSections = new List<TrackManager.TrackSectionKey>() {
            TrackManager.TrackSectionKey.RailOne,
            TrackManager.TrackSectionKey.RailTwo,
            TrackManager.TrackSectionKey.RailThree,
            TrackManager.TrackSectionKey.RailFour
        };

        public ItemRule()
        {
            currentCoffeeWait = Random.Range(minCoffeeWait, maxCoffeeWait);
            currentSunglassesWait = Random.Range(minSunglassesWait, maxSunglassesWait);
            currentPaperWait = Random.Range(minPaperWait, maxPaperWait);
        }



        public Dictionary<TrackManager.TrackSectionKey, SpawnableType?> Evaluate(
            Dictionary<TrackManager.TrackSectionKey, Dictionary<SpawnableType, float>> latestSpawnTimes,
            Dictionary<TrackManager.TrackSectionKey, SpawnableType?> nextSpawnMap
        )
        {
            // If player already has sunglasses, keep pushing back the timing for the next ones
            // This way we don't immediately spawn new ones after the player loses them
            if (GameManager.instance.playerHasSunglasses)
            {
                lastSunglassesTime = Time.time;
            }



            if (lastPaperTime + currentPaperWait < Time.time)
            {
                nextSpawnMap[GetAllowedNextTrackSection()] = SpawnableType.Paper;
                lastPaperTime = Time.time;
                currentPaperWait = Random.Range(minPaperWait, maxPaperWait);
            }
            else if (lastSunglassesTime + currentSunglassesWait < Time.time)
            {
                nextSpawnMap[GetAllowedNextTrackSection()] = SpawnableType.Sunglasses;
                lastSunglassesTime = Time.time;
                currentSunglassesWait = Random.Range(minSunglassesWait, maxSunglassesWait);
            }
            else if (lastCoffeeTime + currentCoffeeWait < Time.time)
            {
                nextSpawnMap[GetAllowedNextTrackSection()] = SpawnableType.Coffee;
                lastCoffeeTime = Time.time;
                currentCoffeeWait = Random.Range(minCoffeeWait, maxCoffeeWait);
            }

            return nextSpawnMap;
        }

        TrackManager.TrackSectionKey GetAllowedNextTrackSection()
        {
            return allowedTrackSections[Random.Range(0, allowedTrackSections.Count)]; // select one at random
        }
    }

    class EnemyRule : ISpawnRule
    {
        float timeBetweenWaves = 2;

        float trainToRatRatio = 0.2f;

        int minEnemiesPerWave = 4;

        int maxEnemiesPerWave = 10;


        // Waits are used for what is coming next - so if we are waiting to spawn a rat
        // use the rat wait range
        float minRatWait = 0.5f;
        float maxRatWait = 1;

        float minTrainWait = 2;
        float maxTrainWait = 2;


        float waitForNext;
        TrackManager.TrackSectionKey trackSectionForNext;
        float lastSpawnTime = float.NegativeInfinity;

        Stack<SpawnableType> enemiesForWave;

        Dictionary<SpawnableType, List<TrackManager.TrackSectionKey>> allowedTrackSectionsByEnemy =
        new Dictionary<SpawnableType, List<TrackManager.TrackSectionKey>>() {
            {
                SpawnableType.TrainFromFront,
                new List<TrackManager.TrackSectionKey>() {
                    TrackManager.TrackSectionKey.ChannelOne,
                }
            },
            {
                SpawnableType.TrainFromBehind,
                new List<TrackManager.TrackSectionKey>() {
                    TrackManager.TrackSectionKey.ChannelThree,
                }
            },
            {
                SpawnableType.Rat,
                new List<TrackManager.TrackSectionKey>() {
                    TrackManager.TrackSectionKey.ChannelOne,
                    TrackManager.TrackSectionKey.ChannelTwo,
                    TrackManager.TrackSectionKey.ChannelThree,
                    TrackManager.TrackSectionKey.ChannelFour
                }
            }
        };

        public EnemyRule()
        {
            enemiesForWave = CreateWave();
            lastSpawnTime = Time.time;
            waitForNext = timeBetweenWaves;
            trackSectionForNext = GetAllowedNextTrackSection(enemiesForWave);

        }


        TrackManager.TrackSectionKey GetAllowedNextTrackSection(Stack<SpawnableType> enemiesForWave)
        {
            return allowedTrackSectionsByEnemy[enemiesForWave.Peek()] // from the allowed track sections for the upcoming enemy
                    [Random.Range(0, allowedTrackSectionsByEnemy[enemiesForWave.Peek()].Count)]; // select one at random
        }


        public Dictionary<TrackManager.TrackSectionKey, SpawnableType?> Evaluate(
            Dictionary<TrackManager.TrackSectionKey, Dictionary<SpawnableType, float>> latestSpawnTimes,
            Dictionary<TrackManager.TrackSectionKey, SpawnableType?> nextSpawnMap
        )
        {
            if (Time.time - lastSpawnTime >= waitForNext)
            {
                nextSpawnMap[trackSectionForNext] = enemiesForWave.Pop();
                lastSpawnTime = Time.time;
                waitForNext = 0; // clear wait so we can build it below

                if (enemiesForWave.Count <= 0)
                {
                    enemiesForWave = CreateWave();
                    waitForNext += timeBetweenWaves; // add time between wave in addition to whatever wait for next enemy
                }

                // TODO: add logic to manage repeating trains on a single rail
                // TODO: add logic for double rat spawns
                trackSectionForNext = GetAllowedNextTrackSection(enemiesForWave);
                waitForNext += enemiesForWave.Peek() == SpawnableType.Rat ? Random.Range(minRatWait, maxRatWait) : Random.Range(minTrainWait, maxTrainWait);
            }

            return nextSpawnMap;
        }

        Stack<SpawnableType> CreateWave()
        {
            Stack<SpawnableType> newWave = new Stack<SpawnableType>();
            int enemyCount = Random.Range(minEnemiesPerWave, maxEnemiesPerWave);

            for (int i = 0; i <= enemyCount; i++)
            {
                float trainFromFrontPercent = trainToRatRatio / 2f;
                float seed = Random.value;

                SpawnableType enemy = seed < trainFromFrontPercent ? SpawnableType.TrainFromFront :
                    seed < trainToRatRatio ? SpawnableType.TrainFromBehind :
                    SpawnableType.Rat;

                newWave.Push(enemy);
            }

            return newWave;
        }
    }
}
