using UnityEngine;
using UnityEngine.Tilemaps;
using System.Diagnostics;
using System.IO;
using System;

public class MapGenerator : MonoBehaviour
{
    public Tilemap map;
    public Tilemap playerLayer;
    public Tilemap enemyLayer;
    public Tile borderTile, groundTile, doorTile, manaTile, healthTile, playerTile, enemyTile, entryTile, houseTile;
    public AnimatedTile spellHitTile, swordAttackTile;

    private int potionsToPlace;
    private const int maxPotions = 4;
    private const int maxHouses = 4;

    string playerTag = "Player";
    PlayerController playerController;
    string enemyTag = "Enemy";
    EnemyController enemyController;

    [Header("Map Editing")]
    public string mapPath;
    bool needsToReload = false;
    private DateTime lastWriteTime;
    float checkInterval = 0.5f;
    float nextTimeToCheck;

    private void Start()
    {
        GenerateMap();
        Invoke("Spawn", 0.1f);

        // Store intiial write timestamp.
        string fullPath = Path.Combine(Application.streamingAssetsPath, mapPath);
        lastWriteTime = File.GetLastWriteTime(fullPath);
        nextTimeToCheck = Time.time + checkInterval;
    }
    void Spawn()
    {
        SpawnPlayer();
        SpawnEnemy();
    }
    public void GenerateMap()
    {
        // Load and convert the map data to a tilemap.
        MapData mapData = MapLoader.LoadPremadeMap(mapPath);
        TilemapConverter.ConvertMapToTilemap(mapData, map, borderTile, groundTile, doorTile,
            manaTile, healthTile, houseTile, entryTile);
    }
    // Open map file to edit in external editor.
    public void OpenMapFile()
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, mapPath);

#if UNITY_EDITOR || UNITY_STANDALONE
        Process.Start(fullPath);
#else
        Debug.LogWarning("Opening files not supported on this platform");
#endif
    }
    public void ReloadCurrentMap()
    {
        map.ClearAllTiles();
        GenerateMap();
    }

    private void Update()
    {
        if (needsToReload)
        {
            needsToReload = false;
            ReloadCurrentMap();
        }

        if (Time.time >= nextTimeToCheck)
        {
            nextTimeToCheck = Time.time + checkInterval;
            CheckForFileChange();
        }
    }

    private void CheckForFileChange()
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, mapPath);
        DateTime currentWriteTime = File.GetLastWriteTime(fullPath);

        if (currentWriteTime != lastWriteTime)
        {
            lastWriteTime = currentWriteTime;
            needsToReload = true;
        }
    }


    void SpawnPlayer()
    {
        // Spawn player.
        playerController =
            GameObject.FindGameObjectWithTag
            (playerTag).GetComponent<PlayerController>();
        playerController.Initialize(playerLayer, playerTile, enemyLayer, enemyTile, spellHitTile, swordAttackTile, map);
    }
    void SpawnEnemy()
    {
        // Get player's  position.
        Vector3Int playerPosition = playerController.GetCurrentPosition();
        int minDistance = 5;

        // Spawn enemy.
        enemyController =
            GameObject.FindGameObjectWithTag
            (enemyTag).GetComponent<EnemyController>();
        enemyController.Initialize(enemyLayer, playerLayer, enemyTile, playerTile, map, playerPosition, minDistance);
    }  
}