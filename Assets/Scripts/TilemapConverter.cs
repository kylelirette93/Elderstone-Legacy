using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapConverter
{
    public static void ConvertMapToTilemap(MapData mapData, Tilemap map, Tile borderTile, Tile groundTile, Tile doorTile,
    Tile manaTile, Tile healthTile, Tile[] houseTiles, Tile entryTile)
    {
        // Create an array of rows, split the string at each new line.
        string[] rows = mapData.Tiles;

        // Offset to determine position of map in Unity.
        Vector3Int offset = new Vector3Int(-10, -7, 0);

        // Iterate through each character.
        for (int y = 0; y < rows.Length; y++)
        {
            for (int x = 0; x < rows[y].Length; x++)
            {
                // Apply offset to position of each character. 
                // Reverse the y-axis to match Unity's tilemap system.
                Vector3Int tilePosition = new Vector3Int(x, rows.Length - y - 1, 0) + offset;  // Reverse y here.

                switch (rows[y][x])
                {
                    // Set tiles where the characters are in text map.
                    case '#':
                        map.SetTile(tilePosition, borderTile);
                        break;
                    case '_':
                        map.SetTile(tilePosition, groundTile);
                        break;
                    case 'D':
                        map.SetTile(tilePosition, doorTile);
                        break;
                    case 'M':
                        map.SetTile(tilePosition, manaTile);
                        break;
                    case 'H':
                        map.SetTile(tilePosition, healthTile);
                        break;
                    case 'E':
                        map.SetTile(tilePosition, entryTile);
                        break;
                    case '@':
                        // Place a random house.
                        int randomHouseIndex = Random.Range(0, houseTiles.Length);
                        Tile houseTile = houseTiles[randomHouseIndex];
                        // Assign name of tile for collision detection.
                        houseTile.name = "HouseTile";
                        map.SetTile(tilePosition, houseTile);
                        break;
                    
     
                }
            }
        }
    }
}