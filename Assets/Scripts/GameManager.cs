using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using static MapGenerator;
using System;

// TODO :

// MapSize
// expectedPathLength

[Serializable]
public class SavedGameData
{
    public MapGenerator.Coord mapSize;
    public int seed;
    public float obstaclePercentages;
    public List<KeyCode> keyCodes;
    public MapGenerator.Coord startCoord;
    public MapGenerator.Coord targetCoord;
    public List<Coord> Path;

    public SavedGameData(MapGenerator.Coord mapSize, int seed, float obstaclePercentages, List<KeyCode> keyCodes, MapGenerator.Coord startCoord, MapGenerator.Coord targetCoord, List<Coord> Path)
    {
        this.mapSize = mapSize;
        this.seed = seed;
        this.obstaclePercentages = obstaclePercentages;
        this.keyCodes = keyCodes;
        this.startCoord = startCoord;
        this.targetCoord = targetCoord;
        this.Path = Path;
    }
}

public class GameManager : MonoBehaviour
{
    CharacterMovement character;
    MapGenerator map;
    LoadGameData load;
    private UIHandler uh;
    private GetInputs inputs;
    public bool is3DStarted = false;
    public int winStreak;
    public int lastMapSize = 5;
    public int playerScore = 0;
    public List<SavedGameData> gameDatas =new List<SavedGameData>();

    void Start()
    {
        character = FindObjectOfType<CharacterMovement>();
        uh = FindObjectOfType<UIHandler>();
        map = FindObjectOfType<MapGenerator>();
        inputs= FindObjectOfType<GetInputs>();
        load= FindObjectOfType<LoadGameData>();

        winStreak = PlayerPrefs.GetInt("winStreak");
        lastMapSize = PlayerPrefs.GetInt("lastMapSize");
        playerScore = PlayerPrefs.GetInt("playerScore");
        var gameDataString = PlayerPrefs.GetString("playerDatas");
        gameDatas = JsonHelper.FromJson<SavedGameData>(gameDataString);

        var isGameOrLoad = PlayerPrefs.GetInt("isGameOrLoad");

        if(isGameOrLoad == 0) //its mean gameScreen
        {
            SetMapAttributes();
        }
        else // its mean loading a previous game
        {
            load.LoadGenerateMap();
        }

    }

    void SetMapAttributes()
    {
        if (lastMapSize == 0)
        {
            lastMapSize = 5;
            PlayerPrefs.SetInt("lastMapSize", lastMapSize);
        }

        map.currentMap.mapSize = new Coord(lastMapSize, lastMapSize);
        map.expectedPathLength = playerScore + 1;
        //PlayerPrefs.DeleteAll();
        map.GenerateMap();
    
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
        uh.StartCoroutine("MiniMapSetStartPosition");
        uh.StartCoroutine("CameraSmothMovingToTargetPosition");
        Invoke("ExecuteAnimation", 1.5f);
    }


    void ExecuteAnimation()
    {
        character.StartCoroutine("ExecuteAnimation");
    }

    public void GameOverStatSet(bool isSuccess)
    {
        if (isSuccess)
        {
            winStreak++;
            playerScore++;

            if (winStreak == 3)
            {
                if (lastMapSize != 9)
                {
                    lastMapSize += 2;
                    PlayerPrefs.SetInt("lastMapSize", lastMapSize);
                }
                
            }
        }
        else
        {
            winStreak = 0;
            playerScore = playerScore > 0 ? (playerScore-1) : 0;
        }

        PlayerPrefs.SetInt("winStreak", winStreak);
        PlayerPrefs.SetInt("playerScore", playerScore);

        GameSave();
    }

    public void GameSave()
    {
        var current = map.currentMap;
        gameDatas.Add( new SavedGameData(current.mapSize,current.seed,current.obstaclePercent, inputs.inputs,current.startPoint,current.targetPoint,map.Path) );

        string gameDataString = JsonHelper.ToJson<SavedGameData>(gameDatas, true);
        PlayerPrefs.SetString("playerDatas", gameDataString);
    }
}
