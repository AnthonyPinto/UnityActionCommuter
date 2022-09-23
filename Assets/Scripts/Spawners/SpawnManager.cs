using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spawner
{
    using SpawnsForSingleTick = Dictionary<TrackManager.TrackSectionKey, SpawnableType?>;

    public class SpawnManager : MonoBehaviour
    {
        float waitBeforeSpawnStart = 1;
        float spawnTick = 0.25f;
        int currentTick = 0;

        SpawnSequence spawnSequence;

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
            // Construct spawnerMap
            spawnerMap = new Dictionary<SpawnableType, ObjectSpawner> {
                {SpawnableType.Coffee, coffeeSpawner },
                {SpawnableType.Sunglasses, sunglassesSpawner },
                {SpawnableType.Rat, ratSpawner },
                {SpawnableType.TrainFromFront, trainFromFront },
                {SpawnableType.TrainFromBehind, trainFromBehind },
                {SpawnableType.Paper, paperSpawner }
            };
        }

        private void Start()
        {
            spawnSequence = SpawnSequencer.CreateSpawnSequence(GameManager.instance.GetTotalPapers());
            StartCoroutine(SpawningRoutine());
        }


        IEnumerator SpawningRoutine()
        {
            yield return new WaitForSeconds(waitBeforeSpawnStart);
            while (!GameManager.instance.GetIsGameOver())
            {
                SpawnObjects(currentTick);
                currentTick++;
                yield return new WaitForSeconds(spawnTick);
            }
        }

        void SpawnObjects(int thisTick)
        {
            SpawnsForSingleTick nextSpawnMap = spawnSequence[thisTick];

            foreach (TrackManager.TrackSectionKey trackSectionKey in TrackManager.TrackSectionKeyList)
            {
                SpawnableType? spawnable = nextSpawnMap.GetValueOrDefault(trackSectionKey, null);
                if (spawnable.HasValue)
                {
                    SpawnObjectByType(spawnable.Value, trackSectionKey);
                }
            }
        }

        void SpawnObjectByType(SpawnableType spawnableType, TrackManager.TrackSectionKey trackSectionKey)
        {
            spawnerMap[spawnableType].SpawnObjectOnTrackSection(trackSectionKey);
        }
    }
}
