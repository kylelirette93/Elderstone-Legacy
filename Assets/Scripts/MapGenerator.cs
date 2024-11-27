using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public Tilemap map;
    public Tilemap playerLayer;
    public Tilemap enemyLayer;
    public Tile borderTile, groundTile, doorTile, manaTile, healthTile, playerTile, enemyTile, openDoorTile;
    public AnimatedTile spellHitTile, swordAttackTile;
    public Tile[] houseTiles;

    private int potionsToPlace;
    private int housesToPlace;
    private const int maxPotions = 4;
    private const int maxHouses = 4;

    private string[] mapPaths = { "level01.txt", "level02.txt" };

    PlayerController playerController;
    string playerTag = "Player";
    EnemyController enemyController;
    string enemyTag = "Enemy";

    private void Awake()
    {
        // Random number of chest and houses to place.
        potionsToPlace = Random.Range(1, maxPotions);
        housesToPlace = Random.Range(1, maxHouses);

        GenerateMap();

        // Spawn player.
        playerController =
            GameObject.FindGameObjectWithTag
            (playerTag).GetComponent<PlayerController>();
        playerController.Initialize(playerLayer, playerTile, enemyLayer, enemyTile, spellHitTile, swordAttackTile, map);

        // Get player's initial position.
        Vector3Int playerPosition = playerController.GetInitialPosition();
        int minDistance = 5;

        // Spawn enemy.
        enemyController =
            GameObject.FindGameObjectWithTag
            (enemyTag).GetComponent<EnemyController>();
        enemyController.Initialize(enemyLayer, playerLayer,
            enemyTile, playerTile, map, playerPosition, minDistance);
    }

    private string GenerateMapString(int width, int height)
    {
        // Array to hold map characters.
        char[,] map = new char[width, height];
        char wall = '#';
        char houseTile = '@';
        char manaTile = 'M';
        char healthTile = 'H';
        char groundTile = '_';

        int potionsPlaced = 0;
        int housesPlaced = 0;

        // These variables are used to determine the chance of placing a house.
        int rollRange = 100;
        int houseChance = 50;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                map[x, y] = groundTile;

                // The border/wall of the map filled with trees.
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = wall;
                }
                // The corners of the map where chests are placed.
                else if ((x == 1 && y == 1) || (x == width - 2 && y == 1) ||
                         (x == 1 && y == height - 2) || (x == width - 2 && y == height - 2))
                {
                    if (potionsPlaced < potionsToPlace)
                    {
                        map[x, y] = manaTile;
                        potionsPlaced++;
                    }
                }
               

                
                // Creates a box inside the map where houses can be placed.
                else if ((y > 3 && y < height - 3) && (x > 3 && x < width - 3))
                {
                    // Determine a random roll between 0 and 100.
                    int randomRoll = Random.Range(0, rollRange);

                    // Player has 50% chance of placing a house, as long as less than max houses are placed.
                    if (housesPlaced < housesToPlace && randomRoll < houseChance)
                    {
                        map[x, y] = houseTile;
                        housesPlaced++;
                    }
                }
            }
        }

        // Create an instance of string builder.
        System.Text.StringBuilder mapBuilder = new System.Text.StringBuilder();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Append current map character to the string builder.
                mapBuilder.Append(map[x, y]);
            }
            // Adds a new line after each row.
            mapBuilder.AppendLine();
        }
        // Return converted string.
        return mapBuilder.ToString();
    }

    public void GenerateMap()
    {
        // Select a random map from the list of map paths.
        string randomMapPath = mapPaths[Random.Range(0, mapPaths.Length)];

        // Load and convert the map data to a tilemap.
        string mapData = MapLoader.LoadPremadeMap(randomMapPath);
        Debug.Log(mapData);
        TilemapConverter.ConvertMapToTilemap(mapData, map, playerLayer, borderTile, groundTile, doorTile,
            manaTile, healthTile, houseTiles, playerTile);
    }

    
}