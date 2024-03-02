using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIScript : MonoBehaviour
{
    [SerializeField]
    TMP_Text averageUpdateTime;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateAverageUpdateTime(float averageTime)
    {
        averageUpdateTime.text = averageTime.ToString("F7");
    }
}
