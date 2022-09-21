using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Spawner
{
    using SpawnsForSingleTick = Dictionary<TrackManager.TrackSectionKey, SpawnableType?>;
    public enum SpawnableType { Rat, Coffee, Column, TrainFromFront, TrainFromBehind, Sunglasses, Paper }



    public static class SpawnSequencer
    {
        static int ticksPerPaper = 29;



        static List<SpawnableType> SpawnableTypeList = new List<SpawnableType>() { SpawnableType.Rat, SpawnableType.Coffee, SpawnableType.Column, SpawnableType.TrainFromFront, SpawnableType.TrainFromBehind, SpawnableType.Sunglasses, SpawnableType.Paper };



        public static SpawnSequence CreateSpawnSequence(int numberOfPapers)
        {
            SpawnSequence sequence = new SpawnSequence(ticksPerPaper * numberOfPapers);
            for (int i = 0; i < numberOfPapers; i++)
            {
                sequence.AddSpawnEvent(i * ticksPerPaper, TrackManager.TrackSectionKeyList[Random.Range(0, TrackManager.TrackSectionKeyList.Count)], SpawnableType.Paper);
            }
            return sequence;
        }

    }

    public class SpawnSequence
    {
        List<SpawnsForSingleTick> spawnsByTick;

        public SpawnsForSingleTick this[int index]
        {
            get => spawnsByTick[index];
            set => spawnsByTick[index] = value;
        }

        public SpawnSequence(int numberOfTicks)
        {
            spawnsByTick = new List<SpawnsForSingleTick>();
            for (int i = 0; i < numberOfTicks; i++)
            {
                spawnsByTick.Add(CreateNewSpawnsForSingleTickObj());
            }
        }


        SpawnsForSingleTick CreateNewSpawnsForSingleTickObj()
        {
            SpawnsForSingleTick obj = new SpawnsForSingleTick();
            foreach (TrackManager.TrackSectionKey sectionKey in TrackManager.TrackSectionKeyList)
            {
                obj.Add(sectionKey, null);
            }
            return obj;
        }

        public void AddSpawnEvent(int tick, TrackManager.TrackSectionKey trackSectionKey, SpawnableType spawnableType)
        {
            spawnsByTick[tick][trackSectionKey] = spawnableType;
        }
    }

    public class SpawnOptionMap
    {
        int numberOfTicks;
        int numberOfTrackSections;

        int initialOptionCount;

        List<int> availableOptions;

        public SpawnOptionMap(int numberOfTicks)
        {
            this.numberOfTicks = numberOfTicks;
            this.numberOfTrackSections = TrackManager.TrackSectionKeyList.Count;
            this.initialOptionCount = numberOfTicks * numberOfTrackSections;
        }

        public (int, TrackManager.TrackSectionKey) GetTickAndTrackSectionForOptionIndex(int option)
        {
            int tick = option / numberOfTrackSections;
            int trackSectionIndex = option % numberOfTrackSections;
            return (tick, TrackManager.TrackSectionKeyList[trackSectionIndex]);
        }

        public int GetOptionIndexForTickAndTrackSection(int tick, TrackManager.TrackSectionKey trackSectionKey)
        {
            return (tick * numberOfTrackSections) + TrackManager.TrackSectionKeyList.FindIndex(t => t == trackSectionKey);
        }
    }


}
