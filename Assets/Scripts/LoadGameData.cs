using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGameData : MonoBehaviour
{
    GameManager gm;
    MapGenerator map;
    public string gameDataString;
    
    public List<SavedGameData> gameDatas = new List<SavedGameData>();

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        gameDataString = PlayerPrefs.GetString("playerDatas");

        gameDatas = JsonHelper.FromJson<SavedGameData>(gameDataString);
        
        //LoadGenerateMap();
    }

    public void LoadGenerateMap()
    {
        var gameData = gameDatas[0];
        Debug.Log(gameData.Path[0].x);
        map.GenerateMapFromLoad(gameData.mapSize, gameData.seed, gameData.startCoord, gameData.targetCoord, gameData.Path);
    }
}
