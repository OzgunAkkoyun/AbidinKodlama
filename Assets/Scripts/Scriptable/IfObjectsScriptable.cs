using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Abidin/IfObjects", order = 1)]
public class IfObjectsScriptable : ScriptableObject
{
    [Serializable]
    public class IfObjects
    {
        public string ifName;
        public GameObject ifGameObjects;
        public Sprite ifGameObjectsImage;
    }

    public IfObjects[] ifObjects;

    public IfObjects GetCurrentIfObjects(int currentLevelLevelIndex)
    {
        return ifObjects[currentLevelLevelIndex - 1];
    }

    public IfObjects[] GetAllIfObjects()
    {
        return ifObjects;
    }
}
