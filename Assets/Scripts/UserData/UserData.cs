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

    public UserData(int senarioIndex, int levelIndex, int subLevelIndex, float userTime)
    {
        this.senarioIndex = senarioIndex;
        this.levelIndex = levelIndex;
        this.subLevelIndex = subLevelIndex;
        this.userTime = userTime;
    }
}
