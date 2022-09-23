using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Spawner
{
    using SpawnsForSingleTick = Dictionary<TrackManager.TrackSectionKey, SpawnableType?>;
    using SpawnOptionsByType = Dictionary<SpawnableType, SpawnOptionMap>;
    public enum SpawnableType { Rat, Coffee, Sunglasses, TrainFromFront, TrainFromBehind, Paper }



    public static class SpawnSequencer
    {
        static int ticksPerPaper = 29;
        static List<SpawnableType> SpawnableTypeList = new List<SpawnableType>() { SpawnableType.Rat, SpawnableType.Coffee, SpawnableType.Sunglasses, SpawnableType.TrainFromFront, SpawnableType.TrainFromBehind, SpawnableType.Paper };



        public static SpawnSequence CreateSpawnSequence(int numberOfPapers)
        {
            int numberOfTicks = ticksPerPaper * numberOfPapers;
            SpawnSequence sequence = new SpawnSequence(numberOfTicks);
            SpawnOptionsByType spawnOptionsByType = new SpawnOptionsByType();
            foreach (SpawnableType type in SpawnableTypeList)
            {
                spawnOptionsByType[type] = new SpawnOptionMap(numberOfTicks);
            }

            ApplyAllAllowedTrackSections(spawnOptionsByType);
            AddPapers(numberOfPapers, sequence, spawnOptionsByType);
            AddCoffees(numberOfPapers * 3, sequence, spawnOptionsByType);
            AddTrains(numberOfPapers * 2, sequence, spawnOptionsByType);
            AddRats(numberOfPapers * 8, sequence, spawnOptionsByType);

            return sequence;
        }

        static void AddPapers(int numberToAdd, SpawnSequence sequence, SpawnOptionsByType spawnOptionsByType)
        {
            List<TrackManager.TrackSectionKey> allowedTrackSections = new List<TrackManager.TrackSectionKey>()
            {
                TrackManager.TrackSectionKey.RailOne,
                TrackManager.TrackSectionKey.RailTwo,
                TrackManager.TrackSectionKey.RailThree,
                TrackManager.TrackSectionKey.RailFour
            };

            for (int i = 0; i < numberToAdd; i++)
            {
                // add one paper on the final tick of each set of 'ticks per paper'
                int tick = (i * ticksPerPaper) + ticksPerPaper - 1;
                TrackManager.TrackSectionKey trackSectionKey = allowedTrackSections[Random.Range(0, allowedTrackSections.Count)];
                sequence.AddSpawnEvent(tick, trackSectionKey, SpawnableType.Paper);
                ApplySpawnMasksForPaper(tick, trackSectionKey, spawnOptionsByType);
            }
        }

        static void AddCoffees(int numberToAdd, SpawnSequence sequence, SpawnOptionsByType spawnOptionsByType)
        {
            for (int i = 0; i < numberToAdd; i++)
            {
                if (spawnOptionsByType[SpawnableType.Coffee].AvailableOptionCount <= 0)
                {
                    Debug.LogError("Unable to place Coffee, no available option for Coffee #" + (i + 1) + "/" + numberToAdd);
                    continue;
                }
                (int tick, TrackManager.TrackSectionKey trackSectionKey) = spawnOptionsByType[SpawnableType.Coffee].GetRandomValidTickAndTrackSection();
                sequence.AddSpawnEvent(tick, trackSectionKey, SpawnableType.Coffee);
                ApplySpawnMasksForCoffee(tick, trackSectionKey, spawnOptionsByType);
                Debug.Log("added Coffee, tick: " + tick + ", section: " + trackSectionKey);
            }
        }

        static void AddTrains(int numberToAdd, SpawnSequence sequence, SpawnOptionsByType spawnOptionsByType)
        {
            for (int i = 0; i < numberToAdd; i++)
            {
                // Alternate adding from front and back NOTE the ordering will still be random but there will be roughly 50/50 both ways
                if (i % 2 == 0)
                {
                    if (spawnOptionsByType[SpawnableType.TrainFromFront].AvailableOptionCount <= 0)
                    {
                        Debug.LogError("Unable to place TrainFromFront, no available option for train # " + (i + 1) + "/" + numberToAdd);
                        continue;
                    }
                    (int tick, TrackManager.TrackSectionKey trackSectionKey) = spawnOptionsByType[SpawnableType.TrainFromFront].GetRandomValidTickAndTrackSection();
                    sequence.AddSpawnEvent(tick, trackSectionKey, SpawnableType.TrainFromFront);
                    ApplySpawnMasksForTrainFromFront(tick, trackSectionKey, spawnOptionsByType);
                    Debug.Log("added TrainFromFront, tick: " + tick + ", section: " + trackSectionKey);
                }
                else
                {
                    if (spawnOptionsByType[SpawnableType.TrainFromBehind].AvailableOptionCount <= 0)
                    {
                        Debug.LogError("Unable to place TrainFromBehind, no available option for train # " + (i + 1) + "/" + numberToAdd);
                        continue;
                    }
                    (int tick, TrackManager.TrackSectionKey trackSectionKey) = spawnOptionsByType[SpawnableType.TrainFromBehind].GetRandomValidTickAndTrackSection();
                    sequence.AddSpawnEvent(tick, trackSectionKey, SpawnableType.TrainFromBehind);
                    ApplySpawnMasksForTrainFromBehind(tick, trackSectionKey, spawnOptionsByType);
                    Debug.Log("added TrainFromBack, tick: " + tick + ", section: " + trackSectionKey);
                }



            }
        }

        static void AddRats(int numberToAdd, SpawnSequence sequence, SpawnOptionsByType spawnOptionsByType)
        {
            for (int i = 0; i < numberToAdd; i++)
            {
                if (spawnOptionsByType[SpawnableType.Rat].AvailableOptionCount <= 0)
                {
                    Debug.LogError("Unable to place Rat, no available option for Rat #" + (i + 1) + "/" + numberToAdd);
                    continue;
                }
                (int tick, TrackManager.TrackSectionKey trackSectionKey) = spawnOptionsByType[SpawnableType.Rat].GetRandomValidTickAndTrackSection();
                sequence.AddSpawnEvent(tick, trackSectionKey, SpawnableType.Rat);
                ApplySpawnMasksForRat(tick, trackSectionKey, spawnOptionsByType);
                Debug.Log("added Rat, tick: " + tick + ", section: " + trackSectionKey);
            }
        }

        static void ApplyAllAllowedTrackSections(SpawnOptionsByType spawnOptionsByType)
        {
            // NOTE: perf optimization would be to pass the allowed rails in when we create the optionsMaps

            spawnOptionsByType[SpawnableType.Coffee].ApplyAllowedTrackSections(new List<TrackManager.TrackSectionKey>()
            {
                TrackManager.TrackSectionKey.RailOne,
                TrackManager.TrackSectionKey.RailTwo,
                TrackManager.TrackSectionKey.RailThree,
                TrackManager.TrackSectionKey.RailFour
            });

            spawnOptionsByType[SpawnableType.TrainFromFront].ApplyAllowedTrackSections(new List<TrackManager.TrackSectionKey>()
            {
                TrackManager.TrackSectionKey.ChannelOne,
            });

            spawnOptionsByType[SpawnableType.TrainFromBehind].ApplyAllowedTrackSections(new List<TrackManager.TrackSectionKey>()
            {
                TrackManager.TrackSectionKey.ChannelThree,
            });

            spawnOptionsByType[SpawnableType.Rat].ApplyAllowedTrackSections(new List<TrackManager.TrackSectionKey>()
            {
                TrackManager.TrackSectionKey.ChannelOne,
                TrackManager.TrackSectionKey.ChannelTwo,
                TrackManager.TrackSectionKey.ChannelThree,
                TrackManager.TrackSectionKey.ChannelFour
            });
        }


        static void ApplySpawnMasksForPaper(int tick, TrackManager.TrackSectionKey trackSectionKey, SpawnOptionsByType spawnOptionsByType)
        {
            spawnOptionsByType[SpawnableType.Coffee].ApplySpawnMask(new SpawnMask(tick + 8, tick + 11));

            if (
                trackSectionKey == TrackManager.TrackSectionKey.RailOne ||
                trackSectionKey == TrackManager.TrackSectionKey.RailTwo ||
                trackSectionKey == TrackManager.TrackSectionKey.RailThree
            )
            {
                spawnOptionsByType[SpawnableType.TrainFromFront].ApplySpawnMask(new SpawnMask(tick, tick + 14));
            }

            if (
                trackSectionKey == TrackManager.TrackSectionKey.RailThree ||
                trackSectionKey == TrackManager.TrackSectionKey.RailFour
            )
            {
                spawnOptionsByType[SpawnableType.TrainFromBehind].ApplySpawnMask(new SpawnMask(tick - 8, tick + 15));
            }
        }

        static void ApplySpawnMasksForCoffee(int tick, TrackManager.TrackSectionKey trackSectionKey, SpawnOptionsByType spawnOptionsByType)
        {
            spawnOptionsByType[SpawnableType.Coffee].ApplySpawnMask(new SpawnMask(tick - 2, tick + 3));

            if (
                trackSectionKey == TrackManager.TrackSectionKey.RailOne ||
                trackSectionKey == TrackManager.TrackSectionKey.RailTwo ||
                trackSectionKey == TrackManager.TrackSectionKey.RailThree
            )
            {
                spawnOptionsByType[SpawnableType.TrainFromFront].ApplySpawnMask(new SpawnMask(tick - 3, tick + 5));
            }

            if (
                trackSectionKey == TrackManager.TrackSectionKey.RailThree ||
                trackSectionKey == TrackManager.TrackSectionKey.RailFour
            )
            {
                spawnOptionsByType[SpawnableType.TrainFromBehind].ApplySpawnMask(new SpawnMask(tick - 10, tick + 7));
            }
        }

        static void ApplySpawnMasksForTrainFromFront(int tick, TrackManager.TrackSectionKey trackSectionKey, SpawnOptionsByType spawnOptionsByType)
        {
            spawnOptionsByType[SpawnableType.TrainFromFront].ApplySpawnMask(new SpawnMask(tick - 7, tick + 8));
            spawnOptionsByType[SpawnableType.TrainFromBehind].ApplySpawnMask(new SpawnMask(tick - 6, tick + 7));

            spawnOptionsByType[SpawnableType.Rat].ApplySpawnMask(
                new SpawnMask(
                    tick - 1,
                    tick + 8,
                    new List<TrackManager.TrackSectionKey>()
                    {
                        TrackManager.TrackSectionKey.ChannelOne,
                        TrackManager.TrackSectionKey.ChannelTwo,
                        TrackManager.TrackSectionKey.ChannelThree,
                    }
            ));
        }

        static void ApplySpawnMasksForTrainFromBehind(int tick, TrackManager.TrackSectionKey trackSectionKey, SpawnOptionsByType spawnOptionsByType)
        {
            spawnOptionsByType[SpawnableType.TrainFromFront].ApplySpawnMask(new SpawnMask(tick - 6, tick + 7));
            spawnOptionsByType[SpawnableType.TrainFromBehind].ApplySpawnMask(new SpawnMask(tick - 11, tick + 12));

            spawnOptionsByType[SpawnableType.Rat].ApplySpawnMask(
                new SpawnMask(
                    tick - 5,
                    tick + 15,
                    new List<TrackManager.TrackSectionKey>()
                    {
                        TrackManager.TrackSectionKey.ChannelThree,
                        TrackManager.TrackSectionKey.ChannelFour
                    }
            ));
        }

        static void ApplySpawnMasksForRat(int tick, TrackManager.TrackSectionKey trackSectionKey, SpawnOptionsByType spawnOptionsByType)
        {
            // NOTE: only prevent other rats too close on the same track section
            spawnOptionsByType[SpawnableType.Rat].ApplySpawnMask(new SpawnMask(tick - 1, tick + 2, new List<TrackManager.TrackSectionKey>() { trackSectionKey }));
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
        public int AvailableOptionCount { get => availableOptions.Count; }

        public SpawnOptionMap(int numberOfTicks)
        {
            this.numberOfTicks = numberOfTicks;
            this.numberOfTrackSections = TrackManager.TrackSectionKeyList.Count;
            this.initialOptionCount = numberOfTicks * numberOfTrackSections;
            this.availableOptions = new List<int>();
            for (int i = 0; i < initialOptionCount; i++)
            {
                availableOptions.Add(i);
            }
        }

        public (int, TrackManager.TrackSectionKey) GetRandomValidTickAndTrackSection()
        {
            int optionNumber = availableOptions[Random.Range(0, availableOptions.Count)];
            return GetTickAndTrackSectionForOptionNumber(optionNumber);
        }

        public void ApplySpawnMask(SpawnMask spawnMask)
        {
            foreach (TrackManager.TrackSectionKey trackSection in spawnMask.trackSections)
            {
                for (int tick = spawnMask.startTick; tick < spawnMask.endTick; tick++)
                {
                    int optionNumber = GetOptionNumberForTickAndTrackSection(tick, trackSection);
                    availableOptions.Remove(optionNumber);
                }
            }
        }

        public void ApplyAllowedTrackSections(List<TrackManager.TrackSectionKey> allowedTrackSections)
        {
            foreach (TrackManager.TrackSectionKey trackSection in TrackManager.TrackSectionKeyList)
            {
                if (allowedTrackSections.Contains(trackSection))
                {
                    continue;
                }

                for (int tick = 0; tick < numberOfTicks; tick++)
                {
                    int optionNumber = GetOptionNumberForTickAndTrackSection(tick, trackSection);
                    availableOptions.Remove(optionNumber);
                }
            }
        }

        (int, TrackManager.TrackSectionKey) GetTickAndTrackSectionForOptionNumber(int option)
        {
            int tick = option / numberOfTrackSections;
            int trackSectionIndex = option % numberOfTrackSections;
            return (tick, TrackManager.TrackSectionKeyList[trackSectionIndex]);
        }

        int GetOptionNumberForTickAndTrackSection(int tick, TrackManager.TrackSectionKey trackSectionKey)
        {
            return (tick * numberOfTrackSections) + TrackManager.TrackSectionKeyList.FindIndex(t => t == trackSectionKey);
        }

    }

    public class SpawnMask
    {
        public List<TrackManager.TrackSectionKey> trackSections;
        public int startTick;
        public int endTick;

        public SpawnMask(int startTick, int endTick, List<TrackManager.TrackSectionKey> trackSections = null)
        {
            this.trackSections = trackSections == null ? TrackManager.TrackSectionKeyList : trackSections;
            this.startTick = startTick;
            this.endTick = endTick;
        }

    }
}
