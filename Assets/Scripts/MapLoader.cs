using System.IO;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class MapLoader
{
    static string currentPath;
    public static MapData LoadPremadeMap(string mapFilePath)
    {
        currentPath = Application.streamingAssetsPath + "/" + mapFilePath;
        Debug.Log($"[MapLoader] Loading JSON from: {currentPath}");
        string jsonString = File.ReadAllText(currentPath);

        MapData mapData = JsonUtility.FromJson<MapData>(jsonString);
        return mapData;
    }

    public static MapData GetData(string mapFilePath)
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, mapFilePath);
        string jsonString = File.ReadAllText(fullPath);
        MapData mapData = JsonUtility.FromJson<MapData>(jsonString);
        return mapData;
    }
}

[System.Serializable]
public class MapData
{
    public string[] Tiles;
}