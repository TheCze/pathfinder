using Unity.Mathematics;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;


public struct PathResult
{
    public float timeToFinish;
    public bool foundPath;
    public int openSize;
    public int closedSize;
    public int pathLength;
}

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    DOTSPathfinder dotsPathfinder;

    [SerializeField]
    OOPathfinder oopPathfinder;

    [SerializeField]
    LevelCreator levelCreator;

    public Tile[,] tiles;

    [SerializeField]
    TextMeshProUGUI statusText;

    List<String> oopStats;
    List<String> dodStats;
    int levelsPerSize = 50;
    int currentDimension = 10;
    int increasePerStep = 10;
    int maxDimension = 100;

    int scenario = 0;
    int maxScenario = 2;
    string[] runDescription = { "LowDensity", "MediumDensity", "HighDensity" };
    float[] wallLikelyness = { 0.0f, 0.1f, 0.15f };
    float[] roomDensity = { 0.1f, 0.1f, 0.2f };
    int[] minDoors = { 0, 0, 4 };
    int[] maxDoors = { 4, 4, 10 };

    bool rejectFailures = false;

    int currentLevel = 1;
    float startTime;
    List<PathResult> oopResults;
    List<PathResult> dodResults;

    private void Start()
    {
        startTime = Time.realtimeSinceStartup;
        oopResults = new List<PathResult>();
        dodResults = new List<PathResult>();
        oopStats = new List<String>();
        dodStats = new List<String>();
        setUpLevelCreator();
    }

    private void setUpLevelCreator()
    {
        levelCreator.wallLikelyness = wallLikelyness[scenario];
        levelCreator.roomDensity = roomDensity[scenario];
        levelCreator.minDoors = minDoors[scenario];
        levelCreator.maxDoors = maxDoors[scenario];
    }

    private void Update()
    {
        float passedTime = Time.realtimeSinceStartup - startTime;
        statusText.text = "Running " + runDescription[scenario] + "\nCurrent Dimension: " + currentDimension + "/"+ currentDimension + "\nCurrent Iteration: " + currentLevel + "\nPassed seconds: " + passedTime + "\nIn minutes: " + passedTime/60;
        CreateAndFindPathsIterative();
    }

    private void CreateAndFindPathsIterative()
    {
        if (currentLevel > levelsPerSize)
        {
            levelSizeHasFinished();
        } else
        {
            findPathForDODAndOOP();
        }
    }

    private void levelSizeHasFinished()
    {
        printAndSaveStats("DOD", dodResults);
        printAndSaveStats("OOP", oopResults);
        oopResults.Clear();
        dodResults.Clear();
        currentLevel = 1;
        currentDimension += increasePerStep;
        if (currentDimension > maxDimension)
        {
            scenarioFinished();
        }
    }

    private void scenarioFinished()
    {
        saveStatsToFile(dodStats, "statsDOD.txt");
        saveStatsToFile(oopStats, "statsOOP.txt");
        dodStats.Clear();
        oopStats.Clear();
        if (scenario >= maxScenario)
        {
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
        else
        {
            switchToNextScenario();
        }
    }

    private void switchToNextScenario()
    {
        currentDimension = 10;
        currentLevel = 1;
        scenario++;
        setUpLevelCreator();
    }

    private void findPathForDODAndOOP()
    {
        levelCreator.CreateLevel(currentDimension, currentDimension);
        var level = levelCreator.tiles;
        var dodResult = findPathDOD(level);
        var oopResult = findPathOOP(level);
        if (dodResult.foundPath || rejectFailures == false)
        {
            dodResults.Add(dodResult);
            oopResults.Add(oopResult);
            currentLevel++;
        }
    }

    private void printAndSaveStats(String type, List<PathResult> times)
    {
        float total = 0;
        float openListSize = 0;
        float closedListSize = 0;
        int finished = 0;
        int pathLength = 0;
        foreach (PathResult stat in times)
        {
            total += stat.timeToFinish;
            openListSize += stat.openSize;
            closedListSize += stat.closedSize;
            if (stat.foundPath)
            {
                pathLength += stat.pathLength;
                finished++;
            }
        }
        float averageOpenList = openListSize / times.Count;
        float averageClosedList = closedListSize / times.Count;
        float averagePathLength = pathLength / finished;
        float average = total / times.Count;
        float foundPercentage = (float)finished / times.Count;
        String statsLine = type + ";" + scenario + ";" + currentDimension + ";" + average + ";" + foundPercentage + ";" + averageOpenList + ";" + averageClosedList + ";" + averagePathLength;
        saveStats(type, statsLine);
    }

    private void saveStats(String type, String stats)
    {
        if (type == "OOP")
        {
            oopStats.Add(stats);
        } else
        {
            dodStats.Add(stats);
        }
    }

    private void saveStatsToFile(List<String> stats, String filename)
    {
        string path = Path.Combine(Application.persistentDataPath, runDescription[scenario] + filename);
        using (StreamWriter writer = new StreamWriter(path))
        {
            foreach (string stat in stats)
            {
                writer.WriteLine(stat);
            }
            var passedTime = Time.realtimeSinceStartup - startTime;
            writer.WriteLine("Total runtime: " + passedTime);
        }
    }

    private PathResult findPathOOP(Tile[,] level)
    {
        var dimensions = getTilesWidthAndHeight(level);
        var start = getTileAtPosition(level, 0, 0);
        start.SetCostToReach(0);
        var target = getTileAtPosition(level, dimensions.x, dimensions.y);
        return oopPathfinder.FindPath(level, start, target);
    }

    private PathResult findPathDOD(Tile[,] level)
    {
        var dimensions = getTilesWidthAndHeight(level);
        int2 start = new int2(0, 0);
        int2 target = new int2(dimensions.x, dimensions.y);
        return dotsPathfinder.FindPath(start, target, level, currentDimension, currentDimension);
    }

    private Tile getTileAtPosition(Tile[,] tiles, int x, int y)
    {
        foreach (Tile tile in tiles)
        {
            if (tile.position.x == x && tile.position.y == y)
            {
                return tile;
            }
        }
        return null;
    }

    private int2 getTilesWidthAndHeight(Tile[,] tiles)
    {
        int x = 0;
        int y = 0;
        foreach (Tile t in tiles)
        {
            if (t.position.x > x)
            {
                x = t.position.x;
            }
            if (t.position.y > y)
            {
                y = t.position.y;
            }
        }
        return new int2(x, y);
    }
}
