using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapConverter
{
    public static void ConvertMapToTilemap(MapData mapData, Tilemap map, Tile borderTile, Tile groundTile, Tile doorTile,
    Tile manaTile, Tile healthTile, Tile houseTile, Tile entryTile)
    {
        // Create an array of rows, split the string at each new line.
        string[] rows = mapData.Tiles;
        int mapWidth = rows[0].Length;
        int mapHeight = rows.Length;
        Vector3Int offset = new Vector3Int(-mapWidth / 2, -mapHeight / 2, 0);

        // Iterate through each character.
        for (int y = 0; y < rows.Length; y++)
        {
            for (int x = 0; x < rows[y].Length; x++)
            {
                // Apply offset to position of each character. 
                // Reverse the y-axis to match Unity's tilemap system.
                Vector3Int tilePosition = new Vector3Int(x, rows.Length - y - 1, 0) + offset;

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
                        map.SetTile(tilePosition, houseTile);
                        break;   
                }
            }
        }
    }
}