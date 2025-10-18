using System.IO;
using UnityEngine;

public class MapLoader
{
    public static MapData LoadPremadeMap(string mapFilePath)
    {
        // Read all text from file at path.
        string fullPath = Application.streamingAssetsPath + "/" + mapFilePath;

        if (!File.Exists(fullPath))
        {
            Debug.LogError("Map not found at path...");
            return null;
        }

        string json = File.ReadAllText(fullPath);
        return JsonUtility.FromJson<MapData>(json);      
    }
}

[System.Serializable]
public class MapData
{
    public string[] Tiles;
}