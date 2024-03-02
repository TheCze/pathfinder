using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Mathematics;
using System.Linq;
using System.Collections;
using Unity.Burst;
using Unity.VisualScripting;

public class DOTSPathfinder : MonoBehaviour
{
    NativeArray<Node> nodes;
    int width;
    int height;

    private void ImportLevel(Tile[,] level, int width, int height, int2 target) {
        nodes = new NativeArray<Node>(width*height, Allocator.Temp);
        this.width = width;
        this.height = height;
        foreach (Tile tile in level)
        {
            Node node = new Node();
            node.x = tile.position.x;
            node.y = tile.position.y;
            node.walkable = tile.walkable;
            node.index = GetIndex(new int2(node.x, node.y));
            node.g = int.MaxValue;
            node.f = int.MaxValue;
            node.CalculateHCost(target);
            node.reachedFromIndex = -1;
            nodes[node.index] = node;
        }
    }

    public PathResult FindPath(int2 start, int2 target, Tile[,] level, int width, int height)
    {
        PathResult result = new PathResult();
        result.foundPath = false;
        ImportLevel(level, width, height, target);
        float tiles = width * height;
        NativeList<int> neighbors = new NativeList<int>(4, Allocator.Temp);
        NativeList<int> openList = new NativeList<int>((int)(tiles * 0.25), Allocator.Temp);
        NativeHashSet<int> closedList = new NativeHashSet<int>((int)(tiles * 0.25), Allocator.Temp);
        int targetIndex = GetIndex(target);
        int startIndex = GetIndex(start);
        Node startNode = nodes[startIndex];
        startNode.SetG(0);
        nodes[startIndex] = startNode;


        float startTime = Time.realtimeSinceStartup;

        openList.Add(startIndex);
        while (openList.Length > 0)
        {
            int lowestOpenListIndex = GetLowestFIndex(openList);
            Node currentNode = nodes[openList[lowestOpenListIndex]];
            openList.RemoveAtSwapBack(lowestOpenListIndex);
            closedList.Add(currentNode.index);            
            if (currentNode.index == targetIndex)
            {
                result.foundPath = true;
                break;
            }
            neighbors.Clear();
            if (currentNode.x > 0)
            {
                neighbors.Add(value: GetIndex(new int2(currentNode.x - 1, currentNode.y)));
            }
            if (currentNode.y > 0)
            {
                neighbors.Add(value: GetIndex(new int2(currentNode.x, currentNode.y - 1)));
            }
            if (currentNode.y < height - 1)
            {
                neighbors.Add(value: GetIndex(new int2(currentNode.x, currentNode.y + 1)));
            }
            if (currentNode.x < width - 1)
            { 
                neighbors.Add(value: GetIndex(new int2(currentNode.x + 1, currentNode.y)));
            }
            for (int i = 0;i < neighbors.Length; i++)
            {
                var neighborNode = nodes[neighbors[i]];
                if (neighborNode.walkable && !closedList.Contains(neighborNode.index))
                {
                    if (neighborNode.g > currentNode.g +1)
                    {
                        neighborNode.SetG(currentNode.g + 1);
                        neighborNode.reachedFromIndex = currentNode.index;
                    }
                    if (!openList.Contains(neighborNode.index))
                    {
                        openList.Add(neighborNode.index);
                    }
                    nodes[neighbors[i]] = neighborNode;
                }
            }
        }
        result.openSize = openList.Length;
        result.closedSize = closedList.Count;
        result.timeToFinish = Time.realtimeSinceStartup - startTime;
        result.timeToFinish *= 1000;
        if (result.foundPath)
        {
            result.pathLength = GetPathLength(startIndex, targetIndex);
        }
        openList.Dispose();
        closedList.Dispose();
        neighbors.Dispose();
        nodes.Dispose();
        
        return result;
    }

    private int GetPathLength(int startIndex, int targetIndex)
    {
        int length = 0;
        var currentIndex = targetIndex;
        while (currentIndex != startIndex)
        {
            currentIndex = nodes[currentIndex].reachedFromIndex;
            length++;
        }
        return length;
    }
   
    private int GetLowestFIndex(NativeList<int> list)
    {
        int lowestCostF = int.MaxValue;
        int lowestIndex = 0;
        for (int i = 1; i < list.Length; i++)
        {
            if (nodes[list[i]].f <= lowestCostF)
            {
                lowestCostF = nodes[list[i]].f;
                lowestIndex = i;
            }
        }
        return lowestIndex;
    }

    private int GetIndex(int2 pos)
    {
        return pos.y * width + pos.x;
    }
}

public struct Node
{
    public int x;
    public int y;
    public bool walkable;

    public int f;
    public int g;
    public int h;

    public int index;
    public int reachedFromIndex;

    private void UpdateF()
    {
        f = g + h;
    }

    public void SetG(int g)
    {
        this.g = g;
        UpdateF();
    }

    public void CalculateHCost(int2 target)
    {
        h = math.abs(target.x - x) + math.abs(target.y - y);
        UpdateF();
    }
}