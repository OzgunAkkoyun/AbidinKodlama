using System.Collections.Generic;
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
            gameDatas = JsonHelper.FromJson<SavedGameData>(gameDataString);
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

                //if (gm.inputs.inputs[i] == GetInputs.code.Forward)
                //{
                //    gm.uh.ShowKeys(90);
                //}
                //else if (gm.inputs.inputs[i] == GetInputs.code.Left)
                //{
                //    gm.uh.ShowKeys(180);
                //}
                //else if (gm.inputs.inputs[i] == GetInputs.code.Right)
                //{
                //    gm.uh.ShowKeys(0);
                //}
                //else if (gm.inputs.inputs[i] == GetInputs.code.Backward)
                //{
                //    gm.uh.ShowKeys(-90);
                //}
            }
            Invoke("GameStart", 1);
        }
       
    }

    void GameStart()
    {
        gm.GameAnimationStart();
    }
}
