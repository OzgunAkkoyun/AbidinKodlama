using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;
using System;
using System.Collections;
using System.Linq;
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
    public int levelIndex;
    public int subLevelIndex;
    public SavedGameData(MapGenerator.Coord mapSize, int seed, float obstaclePercentages, List<Command> commands, MapGenerator.Coord startCoord, MapGenerator.Coord targetCoord, List<Coord> Path,int scenarioIndex, int levelIndex, int subLevelIndex)
    {
        this.mapSize = mapSize;
        this.seed = seed;
        this.obstaclePercentages = obstaclePercentages;
        this.commands = commands;
        this.startCoord = startCoord;
        this.targetCoord = targetCoord;
        this.Path = Path;
        this.scenarioIndex = scenarioIndex;
        this.levelIndex = levelIndex;
        this.subLevelIndex = subLevelIndex;
    }
}

public class SavedPlayerData
{
    public int succesedLevelCount;
    public int failededLevelCount;
    public int whichScenario;
    public int whichLevel;
    public int whichSubLevel;
    public int lastMapSize;
    public int winStreak;
    public int score;
    public bool showedOpeningVideo;

    public SavedPlayerData(int succesedLevelCount, int failededLevelCount, int whichScenario, int whichLevel, int whichSubLevel, int lastMapSize, int winStreak, int score, bool showedOpeningVideo)
    {
        this.succesedLevelCount = succesedLevelCount;
        this.failededLevelCount = failededLevelCount;
        this.whichScenario = whichScenario;
        this.whichLevel = whichLevel;
        this.whichSubLevel = whichSubLevel;
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
    public PathGenarator pathGenarator;
    public LevelLoader levelLoader;

    public bool is3DStarted = false;
    public bool isGameOver = false;
    public List<SavedGameData> gameDatas = new List<SavedGameData>();
    public SavedPlayerData playerDatas;

    public int isGameOrLoad;
    public int scenarioIndex;
    public LevelStats.Senarios.Levels.SubLevels currentSubLevel;
    public LevelStats.Senarios.Levels currentLevel;
    public LevelStats.Senarios currentSenario;

    public int[] senarioAndLevelIndexs;
    public bool DeleteAllPlayerPrefs;

    void Awake()
    {
        uh = FindObjectOfType<UIHandler>();
        map = FindObjectOfType<MapGenerator>();
        load = FindObjectOfType<LoadGameData>();
        sc = FindObjectOfType<SoundController>();
        uiVideoController = FindObjectOfType<UiVideoController>();
        levelLoader = FindObjectOfType<LevelLoader>();

        if (DeleteAllPlayerPrefs)
        {
            PlayerPrefs.DeleteAll();
        }

        var gameDataString = PlayerPrefs.GetString("gameDatas");
        var playerDataString = PlayerPrefs.GetString("playerDatas");
       
        PlayerDataCheck(playerDataString);
        GameDataCheck(gameDataString);

        sc.PrepareSounds();

        SoundController.instance.Play("Theme");
        uiVideoController.PrepareAllVideos();

        WillVideoShown();

        //yield return new WaitUntil(() => levelController != null );

        levelLoader.SetLevels();

        currentSubLevel = levelLoader.currentLevelStats.GetSubLevel(playerDatas.whichScenario, playerDatas.whichLevel,
            playerDatas.whichSubLevel.ToString());

        currentLevel = levelLoader.currentLevelStats.GetLevel(playerDatas.whichScenario, playerDatas.whichLevel);
        currentSenario = levelLoader.currentLevelStats.GetSenario(playerDatas.whichScenario);
        
        Debug.Log(currentLevel.levelIndex);
        GameorLoadCheck();
    }

    // 0 == Game
    // 1 == Watch previous game
    // 2 == RestartGame
    // 3 == Cliked On The map
    private void GameorLoadCheck()
    {
        isGameOrLoad = PlayerPrefs.GetInt("isGameOrLoad");
        var isRestart = PlayerPrefs.GetInt("isRestart");

        if (isGameOrLoad == 0) //its mean gameScreen
        {
            if (isRestart == 1)
                load.RestartGenerateMap();
            else
                SetMapAttributes();
        }
        else if(isGameOrLoad == 1 /*|| isGameOrLoad == 2*/) // its mean loading one of the previous games or Restart game
        {
            playerDatas.whichScenario = gameDatas[gameDatas.Count - 1].scenarioIndex;
            currentSenario.senarioIndex = gameDatas[gameDatas.Count - 1].scenarioIndex;
            currentLevel.levelIndex = gameDatas[gameDatas.Count - 1].levelIndex;
            currentSubLevel.subLevelIndex = gameDatas[gameDatas.Count - 1].subLevelIndex;
            load.LoadGenerateMap(isGameOrLoad);
        }
        else
        {
            SetSelectedLevelProporties(isRestart);
        }
    }

    private void SetSelectedLevelProporties(int isRestart)
    {
        senarioAndLevelIndexs = PlayerPrefs.GetString("selcetedLevelProps").Split('-').Select(int.Parse).ToArray();

        currentSubLevel = levelLoader.currentLevelStats.GetSubLevel(senarioAndLevelIndexs[0],
            senarioAndLevelIndexs[1],
            senarioAndLevelIndexs[2].ToString());

        currentLevel = levelLoader.currentLevelStats.GetLevel(senarioAndLevelIndexs[0],
            senarioAndLevelIndexs[1]);

        currentSenario = levelLoader.currentLevelStats.GetSenario(senarioAndLevelIndexs[0]);

        playerDatas.whichScenario = senarioAndLevelIndexs[0];
        scenarioIndex = senarioAndLevelIndexs[0];
        playerDatas.lastMapSize = currentLevel.mapSize;
        map.currentMap.mapSize = new Coord(currentLevel.mapSize, currentLevel.mapSize);
        pathGenarator.expectedPathLength = currentSubLevel.pathLenght;

        if (isRestart == 1)
        {
            load.RestartGenerateMap();
        }
        else
        {
            map.GameStart();
        }
    }

    private void WillVideoShown()
    {
        uiVideoController.GetCurrentVideoObject(playerDatas.whichScenario + "-" + playerDatas.lastMapSize);

        if (!uiVideoController.currentVideoObject.isShowed)
        {
            uiVideoController.ShowVideo(playerDatas.whichScenario + "-" + playerDatas.lastMapSize);
        }
        else
        {
            uiVideoController.videoPanel.SetActive(false);
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
            playerDatas = new SavedPlayerData(0, 0, 1,1,1, 5, 0, 0, false);
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
            scenarioIndex = playerDatas.whichScenario;

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
        pathGenarator.expectedPathLength = currentSubLevel.pathLenght;

        map.GameStart();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            GameAnimationStart();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            ScreenShotHandler.TakeScreenShot_Static(512,360);
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
        if ((isGameOrLoad == 0 || isGameOrLoad == 2) && !currentSubLevel.passed)
        {
            if (isSuccess)
            {
                currentSubLevel.passed = true;
                if (playerDatas.whichSubLevel == 3)
                {
                    playerDatas.whichSubLevel = 1;
                    playerDatas.whichLevel++;
                    currentLevel.levelComplated = true;
                    playerDatas.winStreak = 0;
                    playerDatas.showedOpeningVideo = false;

                    uiVideoController.ShowVideo(scenarioIndex + "-" + playerDatas.lastMapSize + "-end");

                    if (playerDatas.lastMapSize != 9)
                    {
                        playerDatas.lastMapSize += 2;
                    }
                    else//New Senario
                    {
                        playerDatas.whichScenario++;
                        playerDatas.whichLevel = 1;
                        currentSenario.senarioComplated = true;
                        playerDatas.lastMapSize = 5;
                        playerDatas.score = 0;
                    }
                }
                else
                {
                    playerDatas.whichSubLevel++;
                }
                
            }
            else
            {
                playerDatas.winStreak = 0;
                playerDatas.failededLevelCount++;
                playerDatas.score = playerDatas.score > 0 ? (playerDatas.score - 1) : 0;
            }
            levelLoader.SaveLevelStats();
            
            PlayerDataSave();
        }
        GameDataSave();
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
        gameDatas.Add(new SavedGameData(current.mapSize, current.seed, current.obstaclePercent, commander.commands, current.startPoint, current.targetPoint, pathGenarator.Path,currentSenario.senarioIndex,currentLevel.levelIndex,currentSubLevel.subLevelIndex));
        
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