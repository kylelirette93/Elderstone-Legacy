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

    public void RegisterEnemy(EnemyController enemy)
    {
        if (!activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
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

    
}
