using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public Vector2Int position;
    public float costToReach = -1;
    public float heuristicCost = 0;
    public bool walkable;
    public Tile reachedFrom;
    public List<Tile> neighbors;

    public Tile(Vector2Int position, bool walkable)
    {
        this.position = position;
        this.walkable = walkable;
        neighbors = new List<Tile>();
    }

    public float GetTotalCost()
    {
        return costToReach + heuristicCost;
    }

    public void SetCostToReach(int costToReach)
    {
        this.costToReach = costToReach;
    }

    public void CheckIfCheapestRoute(Tile source)
    {
        if (costToReach == -1 || source.costToReach + 1 < costToReach) {
            costToReach = source.costToReach + 1;
            reachedFrom = source;
        }
    }

    public void CalculateHeuristicCost(Vector2Int target)
    {
        heuristicCost = ManhattanDistance(position, target);
    }

    public List<Tile> GetBackwardsPath()
    {
        List<Tile> path = new List<Tile>();
        path.Add(this);
        if (reachedFrom != null)
        {
            path.AddRange(reachedFrom.GetBackwardsPath());
        }
        return path;
    }

    private int ManhattanDistance(Vector2Int start, Vector2Int target)
    {
        return Mathf.Abs(start.x - target.x) + Mathf.Abs(start.y - target.y);
    }
}
