using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGameData : MonoBehaviour
{
    GameManager gm;
    MapGenerator map;
    GetInputs inputs;
    public string gameDataString;
    
    public List<SavedGameData> gameDatas = new List<SavedGameData>();

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        map = FindObjectOfType<MapGenerator>();
        inputs = FindObjectOfType<GetInputs>();
        gameDataString = PlayerPrefs.GetString("playerDatas");

        gameDatas = JsonHelper.FromJson<SavedGameData>(gameDataString);
        
        //LoadGenerateMap();
    }

    public void LoadGenerateMap()
    {
        var gameData = gameDatas[gameDatas.Count-1];
        map.GenerateMapFromLoad(gameData.mapSize, gameData.seed, gameData.startCoord, gameData.targetCoord, gameData.Path);

        inputs.inputs = gameData.keyCodes;

        for (int i = 0; i < inputs.inputs.Count; i++)
        {
            if (inputs.inputs[i]==KeyCode.UpArrow)
            {

                inputs.ShowKeys(90);
            }
            else if (inputs.inputs[i] == KeyCode.LeftArrow)
            {
                inputs.ShowKeys(180);
            }
            else if (inputs.inputs[i] == KeyCode.RightArrow)
            {
                inputs.ShowKeys(0);

            }
            else if (inputs.inputs[i] == KeyCode.DownArrow)
            {
                inputs.ShowKeys(-90);

            }
        }
        Invoke("GameStart", 1);
        
        //get inputs
        // character movement
        // set inputs to do panel
        // ui handler map diminish
        //
    }

    void GameStart()
    {
        gm.GameAnimationStart();
    }
}
