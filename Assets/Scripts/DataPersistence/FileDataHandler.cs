using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public static class FileDataHandler
{
    static string dataDirectoryPath = Application.persistentDataPath;
    static string dataFileName = "data.game";

    static string FullPath { get => Path.Combine(dataDirectoryPath, dataFileName); }




    public static List<(string, int)> Load()
    {
        HighScoreData loadedData = null;
        if (File.Exists(FullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(FullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadedData = JsonUtility.FromJson<HighScoreData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error Occured while trying to load data from file: " + FullPath + "\n" + e);
            }
        }
        return loadedData == null ? null : loadedData.GetRawHighScores();
    }

    public static void Save(List<(string, int)> highScores)
    {

        try
        {

            HighScoreData serializableData = new HighScoreData(highScores);

            // Create directory if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(FullPath));
            string dataToStore = JsonUtility.ToJson(serializableData, true);
            using (FileStream stream = new FileStream(FullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    Debug.Log(dataToStore);
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error Occured while trying to save data to file: " + FullPath + "\n" + e);
        }
    }

    public static void ClearHighScores()
    {
        try
        {

            HighScoreData serializableData = new HighScoreData(new List<(string, int)>());

            // Create directory if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(FullPath));
            string dataToStore = JsonUtility.ToJson(serializableData, true);
            using (FileStream stream = new FileStream(FullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    Debug.Log(dataToStore);
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error Occured while trying to save data to file: " + FullPath + "\n" + e);
        }
    }
}
