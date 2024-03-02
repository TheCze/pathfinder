using System.Collections.Generic;
using UnityEngine;

public class OOPathfinder : MonoBehaviour
{
    public List<Tile> openList;
    public List<Tile> closedList;

    public PathResult FindPath(Tile[,] level, Tile start, Tile end)
    {
        PathResult result = new PathResult();
        result.foundPath = false;
        openList = new List<Tile> { start };
        closedList = new List<Tile>();
        float startTime = Time.realtimeSinceStartup;
        while (openList.Count > 0)
        {
            Tile currentTile = GetLowestFCostTile(openList);
            openList.Remove(currentTile);
            if (currentTile == end)
            {
                result.foundPath = true;
                break;
            }
            closedList.Add(currentTile);
            openList.InsertRange(0, FindValidNeighbors(currentTile, end));
        }
        result.timeToFinish = Time.realtimeSinceStartup - startTime;
        result.timeToFinish *= 1000;
        result.openSize = openList.Count;
        result.closedSize = closedList.Count;
        if (result.foundPath)
        {
            result.pathLength = getPathLength(start, end);
        }
        return result;
    }

    public List<Tile> FindValidNeighbors(Tile currentTile, Tile end)
    {
        List<Tile> validNeighbors = new List<Tile>();
        foreach (var neighbor in currentTile.neighbors)
        {
            if (neighbor.walkable && !closedList.Contains(neighbor))
            {
                neighbor.CheckIfCheapestRoute(currentTile);
                if (!openList.Contains(neighbor))
                {
                    neighbor.CalculateHeuristicCost(end.position);
                    validNeighbors.Add(neighbor);
                }
            }
        }
        return validNeighbors;
    }

    public Tile GetLowestFCostTile(List<Tile> list)
    {
        Tile result = null;
        float lowestCost = int.MaxValue;
        foreach (var tile in list)
        {
            if (tile.GetTotalCost() <= lowestCost)
            {
                lowestCost = tile.GetTotalCost();
                result = tile;
            }
        }
        return result;
    }

    public int getPathLength(Tile start, Tile end)
    {
        int length = 0;
        var current = end;
        while (current != start) {
            current = current.reachedFrom;
            length++;
        }
        return length;
    }
}
