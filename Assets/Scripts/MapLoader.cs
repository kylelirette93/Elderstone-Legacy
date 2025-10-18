using System.IO;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class MapLoader
{
    public static MapData LoadPremadeMap(string mapFilePath)
    {
        string fullPath = Application.streamingAssetsPath + "/" + mapFilePath;
        Debug.Log($"[MapLoader] Loading JSON from: {fullPath}");
        string jsonString = File.ReadAllText(fullPath);

        // Deserialize the string into map data.
        MapData mapData = JsonUtility.FromJson<MapData>(jsonString);
        return mapData;
    }
}

[System.Serializable]
public class MapData
{
    public string[] Tiles;
}