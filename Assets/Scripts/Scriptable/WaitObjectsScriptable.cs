using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abidin/WaitObjects", order = 1)]
public class WaitObjectsScriptable : ScriptableObject
{
    [Serializable]
    public class WaitObjects
    {
        [Serializable]
        public class DirtsForLevel
        {
            public string waitName;
            public int seconds;
        }
        public WaitObjects.DirtsForLevel[] dirts;
    }
    [Header("Wait Objects")]
    public WaitObjects[] waitObjects;

    [Serializable]
    public class WaitMetarials
    {
        public string waitName;
        public Material dirtMaterial;
        public GameObject dirtGameObject;
    }
    [Space(5f)]
    [Header("Wait Game Objects / Metarials")]
    public WaitMetarials[] waitMetarials;

    public WaitObjects GetCurrentWaitObject(int levelIndex)
    {
        return waitObjects[levelIndex - 1];
    }

    public int GetDirtMetarialIndex(int levelIndex)
    {
        var index = -1;

        if (levelIndex == 1)
        {
            index = 0;
        }
        else if (levelIndex == 2)
        {
            index = UnityEngine.Random.Range(0, 2);
        }
        else if (levelIndex == 3)
        {
            index = UnityEngine.Random.Range(0, 3);
        }
        return index;
    }
}
