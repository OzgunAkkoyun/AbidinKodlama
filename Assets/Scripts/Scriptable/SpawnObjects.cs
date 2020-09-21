using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Abidin/SpawnObjects/SpawnObjects", order = 1)]
public class SpawnObjects : ScriptableObject
{
    public GameObject startGameObject;
    public GameObject[] targetGameObject;
    public GameObject[] targetNewGameObject;
    [Space(20f)]
    public GameObject tileGameObject;
    public GameObject pathGameObject;
    public GameObject[] environmentGameObjects;
    public GameObject vehicleGameObject;
}
