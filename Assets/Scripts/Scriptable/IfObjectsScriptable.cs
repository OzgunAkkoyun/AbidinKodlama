using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abidin/IfObjects", order = 1)]
public class IfObjectsScriptable : ScriptableObject
{
    [Serializable]
    public class IfObjects
    {
        [Serializable]
        public class IfObjectsForLevel
        {
            public string ifName;
            public GameObject ifGameObjects;
            public Sprite ifGameObjectsImage;
        }
        public IfObjectsForLevel[] ifObjectsForLevels;
    }

    public IfObjects[] ifObjects;


    public IfObjects.IfObjectsForLevel GetCurrentIfObjects(int currentLevelLevelIndex, int subLevelIndex)
    {
        return ifObjects[currentLevelLevelIndex - 1].ifObjectsForLevels[subLevelIndex - 1];
    }

    public IfObjects GetAllIfObjects(int currentLevelLevelIndex)
    {
        return ifObjects[currentLevelLevelIndex - 1];
    }
}
