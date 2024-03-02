using System;
using UnityEngine;


public class StatCategory
{
    public string name;
    static int sampleSize = 1000;
    float[] data = new float[sampleSize];
    int dataIndex = 0;

    public StatCategory(string name)
    {
        this.name = name;
    }

    public void AddDataPoint(float dataPoint)
    {
        data[dataIndex%sampleSize] = dataPoint;
        dataIndex++;
    }

    public float GetAverage()
    {
        float sum = 0;
        float average;
        for (int i = 0; i < Math.Min(sampleSize, dataIndex); i++)
        {
            sum += data[i];
        }
        average = sum / Math.Min(sampleSize, dataIndex);
        return average;
    }
}

public class Stats : MonoBehaviour
{
    [SerializeField]
    UIScript uiScript;

    [SerializeField]
    StatsDisplay[] statsDisplays = new StatsDisplay[categoriesSize];
    public StatCategory[] statCategories = new StatCategory[categoriesSize];

    static int categoriesSize = 4;

    private void Start()
    {
        statCategories[0] = new StatCategory("Update Time");
        statCategories[1] = new StatCategory("Solve Time");
        statCategories[2] = new StatCategory("Fail Time");
        statCategories[3] = new StatCategory("Time per 1000 steps");
        for (int i = 0; i < statsDisplays.Length; i++)
        {
            statsDisplays[i].category = statCategories[i];
            print(statCategories[i].name);
        }
    }

    public void AddTimePerUpdate(float time)
    {
        statCategories[0].AddDataPoint(time);
    }

    public void AddSolveTime(float time)
    {
        statCategories[1].AddDataPoint(time);
    }

    public void AddFailTime(float time)
    {
        statCategories[2].AddDataPoint(time);
    }

    public void AddStepTime(int steps, float time)
    {
        time = time / steps;
        time *= 1000;
        statCategories[3].AddDataPoint(time);
    }
}
