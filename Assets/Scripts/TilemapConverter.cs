using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapConverter
{
    public static void ConvertMapToTilemap(string mapData, Tilemap map, Tilemap playerLayer, Tile borderTile, Tile groundTile, Tile chestTile, Tile[] houseTiles, Tile playerTile)
    {
        // Create an array of rows, split the string at each new line.
        string[] rows = mapData.Split('\n');

        // Offset to determine position of map in unity.
        Vector3Int offset = new Vector3Int(-10, -7, 0);

        // Iterate through each character.
        for (int y = 0; y < rows.Length; y++)
        {
            for (int x = 0; x < rows[y].Length; x++)
            {
                // Apply offset to position of each character. 
                Vector3Int tilePosition = new Vector3Int(x, y, 0) + offset;
                switch (rows[y][x])
                {
                    // Set tiles where the characters are in text map.
                    case '#':
                        map.SetTile(tilePosition, borderTile);
                        break;
                    case '_':
                        map.SetTile(tilePosition, groundTile);
                        break;
                    case '$':
                        map.SetTile(tilePosition, chestTile);
                        break;
                    case '@':
                        // Place a random house.
                        int randomHouseIndex = Random.Range(0, houseTiles.Length);
                        Tile houseTile = houseTiles[randomHouseIndex];

                        // Assign name of tile for collision detection.
                        houseTile.name = "HouseTile";
                        map.SetTile(tilePosition, houseTile);
                        break;
                    case 'P':
                        playerLayer.SetTile(tilePosition, playerTile);
                        break;
                }
            }
        }
    }
}