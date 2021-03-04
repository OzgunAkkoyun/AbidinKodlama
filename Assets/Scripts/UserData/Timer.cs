using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    public static float StartTime;
    public float Duration;

    public Timer()
    {
        StartTime = Time.realtimeSinceStartup;
        Duration = 0;
        Debug.Log(StartTime);
    }

    public void Finish()
    {
        var endTime = Time.realtimeSinceStartup;
        Debug.Log(endTime+" ---" +StartTime);
        Duration = endTime - StartTime;

        Debug.Log($"Calculations took {TimeSpan.FromSeconds(Duration).ToString("g")}");
        
    }
}
