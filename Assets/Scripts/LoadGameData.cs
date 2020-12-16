using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class LoadGameData : MonoBehaviour
{
    GameManager gm;
    
    private string gameDataString;
    
    public List<SavedGameData> gameDatas = new List<SavedGameData>();

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        
        gameDataString = PlayerPrefs.GetString("gameDatas");
        if(gameDataString != "")
            gameDatas = JsonConvert.DeserializeObject<List<SavedGameData>>(gameDataString, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
    }

    public void LoadGenerateMap(int isGameOrLoad)
    {
        gm = FindObjectOfType<GameManager>();
        if (gm.gameDatas.Count == 0)
            return;
        var gameData = gm.gameDatas[gm.gameDatas.Count-1];
        
        gm.map.GameStartForLoad(gameData.mapSize, gameData.seed, gameData.startCoord, gameData.targetCoord, gameData.Path);
        if (isGameOrLoad == 1)
        {
            gm.commander.commands = gameData.commands;

            for (int i = 0; i < gm.commander.commands.Count; i++)
            {
                gm.uh.ShowCommand(gm.commander.commands[i]);
            }
            Invoke("GameStart", 1.5f);
        }
    }

    public void RestartGenerateMap()
    {
        gm = FindObjectOfType<GameManager>();
        if (gm.gameDatas.Count == 0)
            return;
        var gameData = gm.gameDatas[gm.gameDatas.Count - 1];

        gm.map.GameStartForLoad(gameData.mapSize, gameData.seed, gameData.startCoord, gameData.targetCoord, gameData.Path);
    }

    void GameStart()
    {
        gm.GameAnimationStart();
    }
}
