using System.Collections.Generic;
using UnityEngine;

public class TileRenderer : MonoBehaviour
{
    public GameObject tilePrefab;
    public Vector2 tileSize = new Vector2(0.1f, 0.1f);
    private List<GameObject> levelObjects;
    private List<GameObject> pathObjects;

    public void RenderLevel(Tile[,] level, bool renderEnabled)
    {
        ClearRenderer(levelObjects);
        levelObjects = new List<GameObject>();
        if (renderEnabled)
        {
            for (int x = 0; x < level.GetLength(0); x++)
            {
                for (int y = 0; y < level.GetLength(1); y++)
                {
                    Vector3 tilePosition = new Vector3(x * tileSize.x, y * tileSize.y, 0);
                    GameObject tileObject = Instantiate(tilePrefab, tilePosition, Quaternion.identity);                  
                    tileObject.GetComponent<SpriteRenderer>().color = level[x, y].walkable ? Color.white : Color.black;
                    levelObjects.Add(tileObject);
                }
            }
        }
    }

    public void RenderDOTSPath(List<int> openList, List<int> closedList, List<Node> world, int target, int start) { 
        var openListConverted = ConvertIntToTileList(openList, world);
        var closedListConverted = ConvertIntToTileHashSet(closedList, world);
        var path = GetBacktracedPath(world, target);
        var targetNode = GetTileFromNodeWorld(world, target);
        var startNode = GetTileFromNodeWorld(world, start);
        RenderPath(openListConverted, closedListConverted, path, startNode, targetNode);
    }

    private List<Tile> GetBacktracedPath(List<Node> world, int target)
    {
        var node = GetNodeFromIndex(world, target);
        List<Tile> path = new List<Tile>();
        int maxIter = 500;
        while (node.reachedFromIndex != -1 && maxIter > 0)
        {
            maxIter--;
            path.Add(ConvertNodeToTile(node));
            node = GetNodeFromIndex(world, node.reachedFromIndex);
        }
        return path;
    }

    private Tile ConvertNodeToTile(Node node)
    {
        var tile = new Tile(new Vector2Int(node.x, node.y), node.walkable);
        tile.SetCostToReach(node.g);
        tile.heuristicCost = node.h;
        return tile;
    }

    private Node GetNodeFromIndex(List<Node> world, int index)
    {
        foreach (var node in world) {
            if (node.index == index) return node;
        }
        return new Node();
    }

    private Tile GetTileFromNodeWorld(List<Node> world, int index)
    {
        return ConvertNodeToTile(GetNodeFromIndex(world, index));
    }

    private List<Tile> ConvertIntToTileList(List<int> indexes, List<Node> world)
    {
        List<Tile> list = new List<Tile>();
        foreach (int i in indexes)
        {
            list.Add(GetTileFromNodeWorld(world, i));
        }
        return list;
    }

    private HashSet<Tile> ConvertIntToTileHashSet(List<int> indexes, List<Node> world)
    {
        HashSet<Tile> list = new HashSet<Tile>();
        foreach (int i in indexes)
        {
            list.Add(GetTileFromNodeWorld(world, i));
        }
        return list;
    }

    public void RenderPath(List<Tile> openList, HashSet<Tile> closedList, List<Tile> path, Tile start, Tile end)
    {
        ClearRenderer(pathObjects);
        pathObjects = new List<GameObject>();
        RenderTileList(openList, Color.yellow, -0.01f);
        RenderTileList(closedList, Color.red, -0.01f);
        RenderTileList(path, Color.green, -0.02f);
        RenderTile(start, Color.blue, - 0.03f);
        RenderTile(end, Color.magenta, -0.03f);
    }

    private void RenderTileList(List<Tile> tileList, Color color, float zOffset = 0) {
        foreach (Tile tile in tileList)
        {
            RenderTile(tile, color, zOffset);
        }
    }

    private void RenderTileList(HashSet<Tile> tileList, Color color, float zOffset = 0)
    {
        foreach (Tile tile in tileList)
        {
            RenderTile(tile, color, zOffset);
        }
    }

    private void RenderTileList(SortedSet<Tile> tileList, Color color, float zOffset = 0)
    {
        foreach (Tile tile in tileList)
        {
            RenderTile(tile, color, zOffset);
        }
    }

    public void RenderHeuristicHeatMap(List<Tile> tileList)
    {

        pathObjects = new List<GameObject>();
        float minCost = float.MaxValue;
        float maxCost = float.MinValue;

        foreach (Tile tile in tileList)
        {
            float cost = tile.heuristicCost; 
            if (cost < minCost)
            {
                minCost = cost;
            }
            if (cost > maxCost)
            {
                maxCost = cost;
            }
        }

        foreach (Tile tile in tileList)
        {
            float normalizedCost = (tile.heuristicCost - minCost) / (maxCost - minCost);
            Color tileColor = Color.Lerp(Color.green, Color.red, normalizedCost);
            RenderTile(tile, tileColor, -0.5f);
        }
    }

    private void RenderTile(Tile tile, Color color, float zOffset = 0) {
        if (tile == null)
        {
            print("Tile is null");
            return;
        }
        Vector3 tilePosition = new Vector3(tile.position.x * tileSize.x, tile.position.y * tileSize.y, zOffset);
        GameObject tileObject = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
        if (tileObject.GetComponent<TileDebugger>() == null)
        {
            print("TileDebugger nicht gefunden");
        }
        tileObject.GetComponent<TileDebugger>().SetTile(tile);
        tileObject.GetComponent<SpriteRenderer>().color = color;
        pathObjects.Add(tileObject);
    }

    private void ClearRenderer(List<GameObject> objects)
    {
        if (objects == null)
        {
            return;
        }
        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }
    }
}
