using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using static MapGenerator;
using System;

[Serializable]
public class SavedGameData
{
    public MapGenerator.Coord mapSize;
    public int seed;
    public float obstaclePercentages;
    public List<Command> commands;
    public MapGenerator.Coord startCoord;
    public MapGenerator.Coord targetCoord;
    public List<Coord> Path;
    public int scenarioIndex;
    public SavedGameData(MapGenerator.Coord mapSize, int seed, float obstaclePercentages, List<Command> commands, MapGenerator.Coord startCoord, MapGenerator.Coord targetCoord, List<Coord> Path,int scenarioIndex)
    {
        this.mapSize = mapSize;
        this.seed = seed;
        this.obstaclePercentages = obstaclePercentages;
        this.commands = commands;
        this.startCoord = startCoord;
        this.targetCoord = targetCoord;
        this.Path = Path;
        this.scenarioIndex = scenarioIndex;
    }
}

public class SavedPlayerData
{
    public int succesedLevelCount;
    public int failededLevelCount;
    public int whichScenario;
    public int lastMapSize;
    public int winStreak;
    public int score;
    public bool showedOpeningVideo;

    public SavedPlayerData(int succesedLevelCount, int failededLevelCount, int whichScenario, int lastMapSize, int winStreak, int score, bool showedOpeningVideo)
    {
        this.succesedLevelCount = succesedLevelCount;
        this.failededLevelCount = failededLevelCount;
        this.whichScenario = whichScenario;
        this.lastMapSize = lastMapSize;
        this.winStreak = winStreak;
        this.score = score;
        this.showedOpeningVideo = showedOpeningVideo;
    }
}

public class GameManager : MonoBehaviour
{
    public CharacterMovement character;
    public MapGenerator map;
    public LoadGameData load;
    public UIHandler uh;
    public GetInputs inputs;
    public Commander commander;
    public SoundController sc;
    public bool is3DStarted = false;
    public int lastMapSize = 5;
    public List<SavedGameData> gameDatas = new List<SavedGameData>();
    public SavedPlayerData playerDatas;

    public int isGameOrLoad;
    public int scenarioIndex;

    void Awake()
    {
        uh = FindObjectOfType<UIHandler>();
        map = FindObjectOfType<MapGenerator>();
        inputs = FindObjectOfType<GetInputs>();
        load = FindObjectOfType<LoadGameData>();
        sc = FindObjectOfType<SoundController>();
       
        lastMapSize = PlayerPrefs.GetInt("lastMapSize");
        var gameDataString = PlayerPrefs.GetString("gameDatas");
        var playerDataString = PlayerPrefs.GetString("playerDatas");
        //PlayerPrefs.DeleteAll();
        if (gameDataString != "")
        {
            gameDatas = JsonHelper.FromJson<SavedGameData>(gameDataString);
            scenarioIndex = gameDatas[gameDatas.Count - 1].scenarioIndex;
        }
        else
        {
            scenarioIndex = 1;
        }

        if (playerDataString != "")
        {
            playerDatas = JsonUtility.FromJson<SavedPlayerData>(playerDataString);
        }
        else
        {
            playerDatas = new SavedPlayerData(0,0,1,5,0,0,false);
        }
        if (!playerDatas.showedOpeningVideo)
        {
            uh.ShowVideo(playerDatas.whichScenario + "-" + playerDatas.lastMapSize);
        }
        else
        {
            uh.videoPanel.SetActive(false);
            sc.Play("Theme");
        }

        isGameOrLoad = PlayerPrefs.GetInt("isGameOrLoad");
        
        if (isGameOrLoad == 0) //its mean gameScreen
        {
            SetMapAttributes();
        }
        else // its mean loading one of the previous games or Restart game
        {
            load.LoadGenerateMap(isGameOrLoad);
        }
    }

    void SetMapAttributes()
    {
        if (playerDatas.lastMapSize == 0)
        {
            playerDatas.lastMapSize = 5;
            PlayerPrefs.SetInt("lastMapSize", lastMapSize);
        }
        map.currentMap.mapSize = new Coord(playerDatas.lastMapSize, playerDatas.lastMapSize);
        map.expectedPathLength = playerDatas.score + 2;

        map.GameStart();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            GameAnimationStart();
        }
    }
    public void GameAnimationStart()
    {
        is3DStarted = true;
        character = FindObjectOfType<CharacterMovement>();
        ShowInputsCode.Instance.ShowCodesString();
        uh.StartCoroutine("MiniMapSetStartPosition");
        uh.StartCoroutine("CameraSmoothMovingToTargetPosition");
        //Invoke("ExecuteAnimation", 1.5f);
        commander.ApplyCommands();
    }

    //void ExecuteAnimation()
    //{
    //    character = FindObjectOfType<CharacterMovement>();
    //    character.StartCoroutine("ExecuteAnimation");
    //}

    public void GameOverStatSet(bool isSuccess)
    {
        if (isGameOrLoad == 0 || isGameOrLoad == 2)
        {
            if (isSuccess)
            {
                playerDatas.winStreak++;
                playerDatas.score++;
                playerDatas.succesedLevelCount++;

                if (playerDatas.winStreak == 3)
                {
                    playerDatas.winStreak = 0;
                    playerDatas.showedOpeningVideo = false;
                    sc.Pause("Theme");
                    uh.ShowVideo(scenarioIndex+"-"+ playerDatas.lastMapSize + "-end");

                    if (playerDatas.lastMapSize != 9)
                    {
                        playerDatas.lastMapSize += 2;
                        //playerDatas.lastMapSize = lastMapSize;
                        PlayerPrefs.SetInt("lastMapSize", lastMapSize);
                    }
                    else//New Senario
                    {
                        playerDatas.whichScenario++;
                        playerDatas.lastMapSize = 5;
                        playerDatas.score = 0;
                    }
                }
            }
            else
            {
                playerDatas.winStreak = 0;
                playerDatas.failededLevelCount++;
                playerDatas.score = playerDatas.score > 0 ? (playerDatas.score - 1) : 0;
            }
            
            GameDataSave();
            PlayerDataSave();
        }
    }

    public void GameDataSave()
    {
        var current = map.currentMap;
        gameDatas.Add(new SavedGameData(current.mapSize, current.seed, current.obstaclePercent, commander.commands, current.startPoint, current.targetPoint, map.Path,scenarioIndex));

        string gameDataString = JsonHelper.ToJson<SavedGameData>(gameDatas, true);
        PlayerPrefs.SetString("gameDatas", gameDataString);
    }

    public void PlayerDataSave()
    {
        string playerDataString = JsonUtility.ToJson(playerDatas);
        PlayerPrefs.SetString("playerDatas", playerDataString);
    }
}