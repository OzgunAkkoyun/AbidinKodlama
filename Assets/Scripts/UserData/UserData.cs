using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserData
{
    public int senarioIndex;
    public int levelIndex;
    public int subLevelIndex;
    public float userTime;
    public bool levelSuccessed;

    public UserData(int senarioIndex, int levelIndex, int subLevelIndex, float userTime, bool isSuccess)
    {
        this.senarioIndex = senarioIndex;
        this.levelIndex = levelIndex;
        this.subLevelIndex = subLevelIndex;
        this.userTime = userTime;
        this.levelSuccessed = isSuccess;
    }
}
