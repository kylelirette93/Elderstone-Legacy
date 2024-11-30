using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using TMPro;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    private Vector3Int currentPosition;
    private Vector3Int initialPosition;
    private Tilemap enemyLayer;
    public Tilemap playerLayer;
    private Tilemap map;
    public Tile enemyTile;
    public bool isDead = false;
    private Tile playerTile;

    public TextMeshProUGUI healthText;
    public HealthBar healthBar;
    public HealthSystem healthSystem = new HealthSystem();
    PlayerController playerController;

    private float moveInterval = 1.0f;
    private float moveTimer = 0.0f;
    int minDistance;

    private void Start()
    {
        if (playerLayer != null)
        {
            playerController = playerLayer.GetComponent<PlayerController>();
        }
        else
        {
            Debug.LogError("Player layer is not assigned.");
            return;
        }

        // Add enemy to list made in game manager.
        GameManager.instance.RegisterEnemy(this);
    }

    public void Initialize(Tilemap enemyLayer, Tilemap playerLayer, Tile enemyTile, 
        Tile playerTile, Tilemap map, Vector3Int playerPosition, int minDistance)
    {
        this.enemyLayer = enemyLayer;
        this.playerLayer = playerLayer;
        this.enemyTile = enemyTile;
        this.playerTile = playerTile;
        this.map = map;

        if (enemyLayer == null)
        {
            // Debug.LogError("Enemy layer is not assigned.");
            return;
        }
        if (playerLayer == null)
        {
            // Debug.LogError("Player layer is not assigned.");
            return;
        }
        if (enemyTile == null)
        {
            // Debug.LogError("Enemy tile is not assigned.");
            return;
        }
        if (playerTile == null)
        {
            // Debug.LogError("Player tile is not assigned.");
            return;
        }
        if (map == null)
        {
            // Debug.LogError("Map is not assigned.");
            return;
        }

        initialPosition = FindEmptyPosition(playerPosition, minDistance);

        if (initialPosition != Vector3Int.zero)
        {
            // Debug.Log("Found empty position at: " + initialPosition);
            currentPosition = initialPosition;
            enemyLayer.SetTile(initialPosition, enemyTile);
        }
        else
        {
            Debug.Log("No empty position found.");
        }

        
    }

    public void SpawnEnemy()
    {
        Initialize(enemyLayer, playerLayer, enemyTile, playerTile, map, playerController.GetCurrentPosition(), minDistance);
        GameManager.instance.RegisterEnemy(this);
        isDead = false;
        healthSystem.Revive();
    }


    private Vector3Int FindEmptyPosition(Vector3Int playerPosition, int minDistance)
    {
        List<Vector3Int> validPositions = new List<Vector3Int>();

        if (map == null)
        {
            // Debug.LogError("Map is not assigned.");
            return Vector3Int.zero;
        }

        if (playerLayer == null)
        {
            // Debug.LogError("Player layer is not assigned.");
            return Vector3Int.zero;
        }

        // Loop through the entire map dimensions (13 rows by 21 columns)
        for (int y = 0; y < 13; y++) // Loop through rows
        {
            for (int x = 0; x < 21; x++) // Loop through columns
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                // Check if the position is empty on the playerLayer and not a special tile on the map
                TileBase mapTile = map.GetTile(position);
                TileBase playerLayerTile = playerLayer.GetTile(position);

                if (mapTile == null)
                {
                    // Debug.Log($"Skipping position {position} because mapTile is null.");
                    continue;
                }

                if (playerLayerTile == null &&
                    mapTile != map.GetComponent<MapGenerator>().borderTile &&
                    mapTile != map.GetComponent<MapGenerator>().manaTile &&
                    mapTile.name != "HouseTile")
                {
                    if (Vector3Int.Distance(position, playerPosition) >= minDistance && position != playerPosition)
                    {
                        validPositions.Add(position);
                        // Debug.Log($"Added valid position: {position}");
                    }
                    else
                    {
                        // Debug.Log($"Position {position} is too close to the player or is the player's position.");
                    }
                }
                else
                {
                    // Debug.Log($"Position {position} is not valid due to playerLayerTile or mapTile conditions.");
                }
            }
        }

        if (validPositions.Count > 0)
        {
            // Debug.Log("Selecting random position from valid positions.");
            // Randomly select position from list of valid positions.
            int randomIndex = Random.Range(0, validPositions.Count);
            return validPositions[randomIndex];
        }

        // Debug.LogWarning("No valid positions found.");
        return Vector3Int.zero; // No empty position found
    }

    private void Update()
    {
        if (!isDead)
        {
            if (enemyLayer == null || map == null || enemyTile == null)
            {
                Debug.LogError("Enemy controller is not properly initialized.");
                return;
            }


            if (GameManager.instance.isEnemiesTurn)
            {
                moveTimer += Time.deltaTime;

                if (moveTimer >= moveInterval)
                {
                    Vector3Int playerPosition = GetPlayerPosition();
                    Vector3Int directionToPlayer = GetDirectionToPlayer(currentPosition, playerPosition);

                    MoveEnemy(directionToPlayer);
                    moveTimer = 0.0f;

                    GameManager.instance.EndEnemiesTurn();
                }
            }
        }
    }

    private Vector3Int GetPlayerPosition()
    {
        foreach (var pos in playerLayer.cellBounds.allPositionsWithin)
        {
            if (playerLayer.GetTile(pos) == playerTile)
            {
                return pos;
            }
        }
        return Vector3Int.zero;
    }

    public Vector3Int GetEnemyPosition()
    {
        return currentPosition;
    }

    private Vector3Int GetDirectionToPlayer(Vector3Int enemyPosition, Vector3Int playerPosition)
    {
        Vector3Int direction = playerPosition - enemyPosition;

        direction = new Vector3Int(
            direction.x != 0 ? direction.x / Mathf.Abs(direction.x) : 0,
            direction.y != 0 ? direction.y / Mathf.Abs(direction.y) : 0,
            0
        );
        return direction;
    }
    private void MoveEnemy(Vector3Int direction)
    {
        // Add direction to current position.
        Vector3Int newPosition = currentPosition + direction;
        if (CanMoveTo(newPosition))
        {
            // Set player's current position to new position.
            enemyLayer.SetTile(currentPosition, null);
            currentPosition = newPosition;
            enemyLayer.SetTile(currentPosition, enemyTile);
            GameManager.instance.EndEnemiesTurn();
        }
    }

    private bool CanMoveTo(Vector3Int position)
    {
        // Takes where player moves to as parameter.

        if (map == null || playerLayer == null)
        {
            Debug.LogError("Map or playerLayer is not assigned.");
            return false;
        }

        // Get tile as position on the map.
        TileBase mapTile = map.GetTile(position);
        TileBase playerLayerTile = playerLayer.GetTile(position);

        // Check if the position is empty on both layers.
        if (mapTile == null && playerLayerTile == null)
        {
            // If position is empty player can move.
            return true;
        }

        // Apply rules when comparing player's new position.
        // Player cannot walk through walls, chests or houses.
        if (mapTile == map.GetComponent<MapGenerator>().borderTile)
        {
            return false;
        }
        if (mapTile == map.GetComponent<MapGenerator>().manaTile)
        {
            return false;
        }
        if (mapTile.name == "HouseTile")
        {
            return false;
        }
        if (playerLayerTile == playerController.playerTile)
        {
            AttackPlayer();
            return false;
        }
        // If no conditions are met, enemy can move.
        return true;
    }

    void AttackPlayer() 
    {
        int randomValue = Random.Range(0, 100);
        bool hasMissed = randomValue < 20;
        int attackDamage = Random.Range(5, 10);
        if (playerController != null)
        {
            if (!hasMissed)
            {
                playerController.healthSystem.TakeDamage(attackDamage);

                // Check if enemy is dead, if so deactivate health bar.
                if (playerController.healthSystem.health <= 0f)
                {
                    playerController.Die();
                }
            }
            else
            {
                Debug.Log("Enemy missed the player.");
            }
            playerController.UpdateHealthText();

        }
        else
        {
            Debug.LogError("Player controller is not assigned.");
        }
    }

    

    public void UpdateHealthText()
    {
        healthText.text = "ENEMY HP: " + healthSystem.health.ToString();
    }
}