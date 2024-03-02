using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelStorer : MonoBehaviour
{
    public Tile[] myTiles; // Your array of Tile objects
    int width = 120;
    int height = 40;

    public void SaveTilesToJson(Tile[,] tiles, String filename)
    {
        width = tiles.GetLength(0);
        height = tiles.GetLength(1);

        MinimalTileData[] minimalTiles = new MinimalTileData[width * height];
        int index = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                minimalTiles[index++] = new MinimalTileData
                {
                    position = tiles[x, y].position,
                    walkable = tiles[x, y].walkable
                };
            }
        }
        string json = JsonUtility.ToJson(new MinimalTileArray { tiles = minimalTiles });
        string path = Application.persistentDataPath + "/"+ filename+".json";
        File.WriteAllText(path, json);
    }

    public List<Tile[,]> LoadTilesFromJson()
    {
        string[] files = Directory.GetFiles(Application.persistentDataPath, "*.json");
        List<Tile[,]> tileList = new List<Tile[,]>();

        foreach (string file in files)
        {
            string json = File.ReadAllText(file);
            MinimalTileArray tileArray = JsonUtility.FromJson<MinimalTileArray>(json);
            Tile[,] tiles = ConvertToTileArray(tileArray);
            tileList.Add(tiles);
        }
        return tileList;
    }


    Tile[,] ConvertToTileArray(MinimalTileArray minimalTileArray)
    {
        Tile[,] tiles = new Tile[width, height];

        foreach (MinimalTileData tileData in minimalTileArray.tiles)
        {
            tiles[tileData.position.x, tileData.position.y] = new Tile(tileData.position, tileData.walkable);
        }
        tiles = CalculateNeighbors(tiles);
        return tiles;
    }

    private Tile[,] CalculateNeighbors(Tile[,] tiles)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y].neighbors = GetNeighbours(tiles, tiles[x, y]);
            }
        }
        return tiles;
    }

    private List<Tile> GetNeighbours(Tile[,] tiles, Tile t)
    {
        List<Tile> neighbours = new List<Tile>();

        int[] dx = { -1, 0, 1, 0 };
        int[] dy = { 0, 1, 0, -1 };
        int tX = t.position.x;
        int tY = t.position.y;

        for (int i = 0; i < 4; i++)
        {
            int newX = tX + dx[i];
            int newY = tY + dy[i];
            var neighbour = GetTileAtPosition(tiles, new Vector2Int(newX, newY));
            if (neighbour != null)
            {
                neighbours.Add(neighbour);
            }
        }

        return neighbours;
    }

    private Tile GetTileAtPosition(Tile[,] tiles, Vector2Int position)
    {
        if (position.x >= 0 && position.x < width && position.y >= 0 && position.y < height)
        {
            return tiles[position.x, position.y];
        }
        return null;
    }
}


[Serializable]
public class MinimalTileData
{
    public Vector2Int position;
    public bool walkable;
}

[Serializable]
public class MinimalTileArray
{
    public MinimalTileData[] tiles;
}
