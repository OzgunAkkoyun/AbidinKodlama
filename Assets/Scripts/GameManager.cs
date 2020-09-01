using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;
using System;
using Newtonsoft.Json;

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
    public UiVideoController uiVideoController;
    public LoadGameData load;
    public UIHandler uh;
    public UiMiniMapController miniMapController;
    public Commander commander;
    public SoundController sc;
    public bool is3DStarted = false;
    public bool isGameOver = false;
    public List<SavedGameData> gameDatas = new List<SavedGameData>();
    public SavedPlayerData playerDatas;

    public int isGameOrLoad;
    public int scenarioIndex;

    void Awake()
    {
        uh = FindObjectOfType<UIHandler>();
        map = FindObjectOfType<MapGenerator>();
        load = FindObjectOfType<LoadGameData>();
        sc = FindObjectOfType<SoundController>();
        uiVideoController = FindObjectOfType<UiVideoController>();
        //PlayerPrefs.DeleteAll();
        var gameDataString = PlayerPrefs.GetString("gameDatas");
        var playerDataString = PlayerPrefs.GetString("playerDatas");
        
        GameDataCheck(gameDataString);

        PlayerDataCheck(playerDataString);

        WillVideoShown();

        GameorLoadCheck();
    }

    private void GameorLoadCheck()
    {
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

    private void WillVideoShown()
    {
        if (!playerDatas.showedOpeningVideo)
        {
            uiVideoController.ShowVideo(playerDatas.whichScenario + "-" + playerDatas.lastMapSize);
        }
        else
        {
            uiVideoController.videoPanel.SetActive(false);
            sc.Play("Theme");
        }
    }

    private void PlayerDataCheck(string playerDataString)
    {
        if (playerDataString != "")
        {
            playerDatas = JsonUtility.FromJson<SavedPlayerData>(playerDataString);
        }
        else
        {
            playerDatas = new SavedPlayerData(0, 0, 1, 5, 0, 0, false);
        }
    }

    private void GameDataCheck(string gameDataString)
    {
        if (gameDataString != "")
        {
            gameDatas = JsonConvert.DeserializeObject<List<SavedGameData>>(gameDataString, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            //gameDatas = JsonHelper.FromJson<SavedGameData>(gameDataString);
            scenarioIndex = gameDatas[gameDatas.Count - 1].scenarioIndex;
        }
        else
        {
            scenarioIndex = 1;
        }
    }

    void SetMapAttributes()
    {
        if (playerDatas.lastMapSize == 0)
        {
            playerDatas.lastMapSize = 5;
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
        miniMapController.StartCoroutine("MiniMapSetStartPosition");
        uh.StartCoroutine("CameraSmoothMovingToTargetPosition");
        commander.ApplyCommands();
    }

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
                   
                    uiVideoController.ShowVideo(scenarioIndex+"-"+ playerDatas.lastMapSize + "-end");

                    if (playerDatas.lastMapSize != 9)
                    {
                        playerDatas.lastMapSize += 2;
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

    public void EndGame()
    {
        uh.OpenGameOverPanel(character.isPlayerReachedTarget);
        GameOverStatSet(character.isPlayerReachedTarget);
        isGameOver = true;
    }

    public void GameDataSave()
    {
        var current = map.currentMap;
        gameDatas.Add(new SavedGameData(current.mapSize, current.seed, current.obstaclePercent, commander.commands, current.startPoint, current.targetPoint, map.Path,scenarioIndex));
        
        string gameDataString = JsonConvert.SerializeObject(gameDatas, Formatting.Indented, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });

        //string gameDataString = JsonHelper.ToJson<SavedGameData>(gameDatas, true);
        PlayerPrefs.SetString("gameDatas", gameDataString);
    }

    public void PlayerDataSave()
    {
        string playerDataString = JsonUtility.ToJson(playerDatas);
        PlayerPrefs.SetString("playerDatas", playerDataString);
    }
}