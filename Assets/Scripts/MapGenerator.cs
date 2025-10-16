using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public Tilemap map;
    public Tilemap playerLayer;
    public Tilemap enemyLayer;
    public Tile borderTile, groundTile, doorTile, manaTile, healthTile, playerTile, enemyTile, entryTile;
    public AnimatedTile spellHitTile, swordAttackTile;
    public Tile[] houseTiles;

    private int potionsToPlace;
    private int housesToPlace;
    private const int maxPotions = 4;
    private const int maxHouses = 4;

    private string[] mapPaths = { "level01.txt", "level02.txt", "level03.txt", "level04.txt", "level05.txt" };

    string playerTag = "Player";
    PlayerController playerController;
    string enemyTag = "Enemy";
    EnemyController enemyController;

    private void Awake()
    {
        // Random number of chest and houses to place.
        potionsToPlace = Random.Range(1, maxPotions);
        housesToPlace = Random.Range(1, maxHouses);
    }

    private void Start()
    {
        GenerateMap();
        Invoke("Spawn", 0.1f);
    }

    void Spawn()
    {
        SpawnPlayer();
        SpawnEnemy();
    }
    public void GenerateMap()
    {
        // Select a random map from the list of map paths.
        string randomMapPath = mapPaths[Random.Range(0, mapPaths.Length)];

        // Load and convert the map data to a tilemap.
        string mapData = MapLoader.LoadPremadeMap(randomMapPath);
        Debug.Log(mapData);
        TilemapConverter.ConvertMapToTilemap(mapData, map, borderTile, groundTile, doorTile,
            manaTile, healthTile, houseTiles, entryTile);
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