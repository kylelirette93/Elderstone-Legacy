using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using TMPro;

public class EnemyController : MonoBehaviour
{
    private Vector3Int currentPosition;
    private Vector3Int initialPosition;
    private Tilemap enemyLayer;
    private Tilemap playerLayer;
    private Tilemap map;
    private Tile enemyTile;
    private Tile playerTile;

    public TextMeshProUGUI healthText;
    public HealthBar healthBar;
    HealthSystem healthSystem = new HealthSystem();

    private float moveInterval = 1.0f;
    private float moveTimer = 0.0f;

    public void Initialize(Tilemap enemyLayer, Tilemap playerLayer, Tile enemyTile, 
        Tile playerTile, Tilemap map, Vector3Int playerPosition, int minDistance)
    {
        this.enemyLayer = enemyLayer;
        this.playerLayer = playerLayer;
        this.enemyTile = enemyTile;
        this.playerTile = playerTile;
        this.map = map;

        initialPosition = FindEmptyPosition(playerPosition, minDistance);
        if (initialPosition != Vector3Int.zero)
        {
            Debug.Log("Found empty position at: " + initialPosition);
            enemyLayer.SetTile(initialPosition, enemyTile);
            currentPosition = initialPosition;
        }
        else
        {
            Debug.Log("No empty position found.");
        }

        enemyLayer.SetTile(initialPosition, enemyTile);
    }

    private Vector3Int FindEmptyPosition(Vector3Int playerPosition, int minDistance)
    {
        // Loop through the entire map dimensions (13 rows by 21 columns)
        for (int y = 0; y < 13; y++) // Loop through rows
        {
            for (int x = 0; x < 21; x++) // Loop through columns
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                // Check if the position is empty on the playerLayer and not a special tile on the map
                if (playerLayer.GetTile(position) == null &&
                    map.GetTile(position) != map.GetComponent<MapGenerator>().borderTile &&
                    map.GetTile(position) != map.GetComponent<MapGenerator>().chestTile &&
                    map.GetTile(position).name != "HouseTile")
                {
                    if (Vector3Int.Distance(position, playerPosition) >= minDistance)
                    {
                        return position; // Return the first empty position found
                    }
                }
            }
        }
        return Vector3Int.zero; // No empty position found
    }

    private void Update()
    {
        if (enemyLayer == null || map == null || enemyTile == null)
        {
            Debug.LogError("Enemy controller is not properly initialized.");
            return;
        }
        moveTimer += Time.deltaTime;
        Debug.Log(moveTimer);

        if (moveTimer >= moveInterval)
        {
            Vector3Int playerPosition = GetPlayerPosition();
            Vector3Int directionToPlayer = GetDirectionToPlayer(currentPosition, playerPosition);
            MoveEnemy(directionToPlayer);
            moveTimer = 0.0f;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            healthSystem.TakeDamage(10);
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
        if (mapTile == map.GetComponent<MapGenerator>().chestTile)
        {
            return false;
        }
        if (mapTile.name == "HouseTile")
        {
            return false;
        }
        // If no conditions are met, player can move.
        return true;
    }

    private void UpdateHealthText()
    {
        healthText.text = "HP: " + healthSystem.health.ToString();
    }
}