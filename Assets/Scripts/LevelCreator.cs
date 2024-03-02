using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{

    public int width = 120;


    public int height = 80;


    public float wallLikelyness = 0.1f;
    public float roomDensity = 0.1f;
    public int averageRoomSize = 5;
    public int minDoors = 0;
    public int maxDoors = 4;


    float roomSizeVariation = 1f;

    public Tile[,] tiles;

    public Tile StartTile()
    {
        var tile = GetRandomTile();
        tile.walkable = true;
        return tile;
    }

    public Tile EndTile()
    {
        var tile = GetRandomTile();
        tile.walkable = true;
        return tile;
    }

    public Tile GetRandomTile()
    {
        int randomX = Random.Range(0, width);
        int randomY = Random.Range(0, height);
        Tile randomTile = tiles[randomX, randomY];
        return randomTile;
    }

    public List<Tile> GetNeighbours(Tile t)
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
            var neighbour = GetTileAtPosition(new Vector2Int(newX, newY));
            if (neighbour != null)
            {
                neighbours.Add(neighbour);
            }
        }

        return neighbours;
    }

    public Tile GetTileAtPosition(int x, int y)
    {
        return GetTileAtPosition(new Vector2Int((int)x, (int)y));
    }

    public Tile GetTileAtPosition(Vector2Int position)
    {
        if (position.x >= 0 && position.x < width && position.y >= 0 && position.y < height)
        {
            return tiles[position.x, position.y];
        }
        return null;
    }

    public void CreateLevel(int w, int h)
    {
        width = w; height = h;
        tiles = new Tile[width, height];
        float wallIndex = 0.0f;
        bool hasWall = false;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                wallIndex = Random.Range(0.0f, 1.0f);
                if (hasWall)
                {
                    wallIndex *= 2;
                }
                hasWall = wallIndex <= wallLikelyness;
                tiles[x, y] = new Tile(new Vector2Int(x, y), !hasWall);
            }
        }
        AddRooms();
        tiles[0,0].walkable = true;
        tiles[width-1, height-1].walkable = true;
        CalculateNeighbors();
    }

    private void AddRooms()
    {
        int roomsToAdd = (int)(roomDensity * width * height * 0.1f);

        while (roomsToAdd > 0)
        {
            AddRoom();
            roomsToAdd--;
        }
    }

    private void AddRoom()
    {
        int roomX = Random.Range(0, width);
        int roomY = Random.Range(0, height);
        int roomWidth = averageRoomSize + (int)Random.Range(-averageRoomSize * roomSizeVariation, averageRoomSize * roomSizeVariation);
        int roomHeight = averageRoomSize + (int)Random.Range(-averageRoomSize * roomSizeVariation, averageRoomSize * roomSizeVariation);
        int doors = Random.Range(minDoors, maxDoors);
        List<Tile> roomWalls = new List<Tile>();
        for (int x = roomX; x <= roomX + roomWidth; x++)
        {
            Tile tileBottom = GetTileAtPosition(new Vector2Int(x, roomY));
            if (tileBottom != null)
            {
                tileBottom.walkable = false;
                roomWalls.Add(tileBottom);
            }
            Tile tileTop = GetTileAtPosition(new Vector2Int(x, roomY + roomHeight));
            if (tileTop != null)
            {
                tileTop.walkable = false;
                roomWalls.Add(tileTop);
            }
        }


        for (int y = roomY; y < roomY + roomHeight; y++)
        {
            Tile tileLeft = GetTileAtPosition(new Vector2Int(roomX, y));
            if (tileLeft != null)
            {
                tileLeft.walkable = false;
                roomWalls.Add(tileLeft);
            }
            Tile tileRight = GetTileAtPosition(new Vector2Int(roomX + roomWidth, y));
            if (tileRight != null)
            {
                tileRight.walkable = false;
                roomWalls.Add(tileRight);
            }
        }

        while (doors > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, roomWalls.Count);
            Tile randomTile = roomWalls[randomIndex];
            Vector2Int pos = randomTile.position;
            randomTile.walkable = true;
            doors--;
        }

    }

    private void CalculateNeighbors()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y].neighbors = GetNeighbours(tiles[x, y]);
            }
        }
    }
}
