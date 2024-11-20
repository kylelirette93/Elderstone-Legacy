using System.IO;
using UnityEngine;

public class MapLoader
{
    public static string LoadPremadeMap(string mapFilePath)
    {
        // Builds a path to file in streaming assets folder.
        string path = Path.Combine(Application.streamingAssetsPath, mapFilePath);

        // Check if the file exists first.
        if (File.Exists(path))
        {
            // Create an instance of StreamReader and assign it the path.
            StreamReader streamReader = new StreamReader(path);

            // Read all characters from map and return as a single string.
            string content = streamReader.ReadToEnd();
            Debug.Log(content);

            // Close the stream after reading the file.
            streamReader.Close();

            // Return the string content.
            return content;
        }
        else
        {
            // If the file doesn't exist, log an error and return an empty string.
            Debug.LogError("Map file not found: " + path);
            return string.Empty;
        }
    }
}