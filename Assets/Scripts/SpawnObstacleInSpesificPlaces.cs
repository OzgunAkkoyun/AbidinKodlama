using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObstacleInSpesificPlaces : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public GameObject[] currentObstacles;
    [Serializable]
    public class Places
    {
        [Serializable]
        public class SpawnCoords
        {
            public Vector2 coord;
            public int spawnObjectId;
            public Vector3 rotation;
        }
        public SpawnCoords[] spawnCoords;
    }

    public Places[] places;

    private int levelIndex;
    private Places currentPlaces;
    
    public PathGenarator pathGenarator;
    
    public void SpawnObstacleInSpesificPlace()
    {
        levelIndex = mapGenerator.gm.currentLevel.levelIndex;
        currentPlaces = places[levelIndex - 1];

        mapGenerator.allOpenCoords = new List<MapGenerator.Coord>(mapGenerator.allTileCoords);
        for (int i = 0; i < currentPlaces.spawnCoords.Length; i++)
        {
            var currentPos = mapGenerator.CoordToPosition((int)currentPlaces.spawnCoords[i].coord.x, (int)currentPlaces.spawnCoords[i].coord.y);

            var newObstacle = Instantiate(currentObstacles[currentPlaces.spawnCoords[i].spawnObjectId], currentPos + Vector3.up, Quaternion.identity);
            newObstacle.transform.Rotate(currentPlaces.spawnCoords[i].rotation);
            newObstacle.transform.parent = mapGenerator.mapHolder;
            var randomCoord = new MapGenerator.Coord((int)currentPlaces.spawnCoords[i].coord.x, (int)currentPlaces.spawnCoords[i].coord.y); 

            mapGenerator.obstacleGameObject.Add(newObstacle);
            mapGenerator.allObstacleCoord.Add(randomCoord);
            mapGenerator.allOpenCoords.Remove(randomCoord);
        }
        mapGenerator.shuffledOpenTileCoords = new Queue<MapGenerator.Coord>(Utility.ShuffleArray(mapGenerator.allOpenCoords.ToArray(), mapGenerator.currentMap.seed));

       // pathGenarator.CreatePathForWait();
    }
}
