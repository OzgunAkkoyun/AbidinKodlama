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
        if (gameDatas.Count == 0)
            return;
        var gameData = gameDatas[gameDatas.Count-1];
        gm.map.GenerateMapFromLoad(gameData.mapSize, gameData.seed, gameData.startCoord, gameData.targetCoord, gameData.Path);
        if (isGameOrLoad == 1)
        {
            gm.inputs.inputs = gameData.keyCodes;

            for (int i = 0; i < gm.inputs.inputs.Count; i++)
            {
                if (gm.inputs.inputs[i] == GetInputs.code.Forward)
                {
                    gm.uh.ShowKeys(90);
                }
                else if (gm.inputs.inputs[i] == GetInputs.code.Left)
                {
                    gm.uh.ShowKeys(180);
                }
                else if (gm.inputs.inputs[i] == GetInputs.code.Right)
                {
                    gm.uh.ShowKeys(0);
                }
                else if (gm.inputs.inputs[i] == GetInputs.code.Backward)
                {
                    gm.uh.ShowKeys(-90);
                }
            }
            Invoke("GameStart", 1);
        }
       
    }

    void GameStart()
    {
        gm.GameAnimationStart();
    }
}
