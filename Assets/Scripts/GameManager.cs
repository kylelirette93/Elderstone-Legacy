using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isPlayersTurn = true;
    public bool isEnemiesTurn = false;
    private List<EnemyController> activeEnemies = new List<EnemyController>();
    MapGenerator mapGenerator;
    string mapGeneratorTag = "MapGenerator";
    EnemyController enemyController;
    public GameObject gameOverPanel;
    Tilemap enemyLayer;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        mapGenerator = GameObject.FindWithTag(mapGeneratorTag).GetComponent<MapGenerator>();
    }

    public void RegisterEnemy(EnemyController enemy)
    {
        if (!activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
            enemyController = enemy;
        }
    }

    public void DeregisterEnemy(EnemyController enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }

    public bool AreEnemiesRemaining()
    {
        return activeEnemies.Any(enemy => !enemy.isDead);
    }

    public void OnEnemyDeath(EnemyController enemyController)
    {
        DeregisterEnemy(enemyController);

        if (!AreEnemiesRemaining())
        {
            isPlayersTurn = true;
        }
    }

    public void EndPlayerTurn()
    {
        isPlayersTurn = false;
        isEnemiesTurn = true;
    }

    public void StartEnemyTurn()
    {
        if (AreEnemiesRemaining())
        {
            isPlayersTurn = false;
        }
        else
        {
            isPlayersTurn = true;
        }
    }

    public void EndEnemiesTurn()
    {
        isEnemiesTurn = false;
        isPlayersTurn = true;
    }

    void GameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
    }

    

    public Vector3Int LoadNextLevel(ref Tilemap map, ref Tilemap enemyLayer)
    {
        // Clear all existing tilemaps.
        map.ClearAllTiles();
        enemyLayer.ClearAllTiles();


        // Generate another map.
        mapGenerator.GenerateMap();
        enemyController.SpawnEnemy();
        map = mapGenerator.map;
        enemyLayer = mapGenerator.enemyLayer;
        

        Vector3Int entryTilePosition = FindEntryTilePosition(map);

        // Return the entry tile position
        return entryTilePosition;
    }

    public Vector3Int FindEntryTilePosition(Tilemap map)
    {
        foreach (var pos in map.cellBounds.allPositionsWithin)
        {
            if (map.GetTile(pos) == mapGenerator.entryTile)
            {
                return pos;
            }
        }
        return Vector3Int.zero;
    }
}
