using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapContainer : MonoBehaviour
{
    static string[] mapPaths = {"level01.txt", "map2.txt",
        "map3.txt", "map4.txt", "map5.txt"};

    public static string randomMapPath;
    void Start()
    {
        randomMapPath = mapPaths[Random.Range(0, mapPaths.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
