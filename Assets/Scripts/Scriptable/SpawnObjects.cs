using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Abidin/SpawnObjects/SpawnObjects", order = 1)]
public class SpawnObjects : ScriptableObject
{
    public GameObject startGameObject;
    public GameObject targetGameObject;
    public GameObject targetNewGameObject;
    public GameObject tileGameObject;
    public GameObject[] environmentGameObjects;
    public GameObject vehicleGameObject;
}
