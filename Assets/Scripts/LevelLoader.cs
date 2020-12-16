using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public LevelStats levelStats;
    public LevelStats currentLevelStats;
    public SavedPlayerData playerDatas;

    public void SetLevels()
    {
        var playerDataString = PlayerPrefs.GetString("playerDatas");
        var levelStatsString = PlayerPrefs.GetString("levelStats");

        LevelStatCheck(levelStatsString);
        PlayerDataCheck(playerDataString);
    }

    public void SaveLevelStats()
    {
        string levelStatsJsonString = JsonConvert.SerializeObject(currentLevelStats, Formatting.Indented,
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        PlayerPrefs.SetString("levelStats", levelStatsJsonString);
    }

    private void LevelStatCheck(string levelStatsString)
    {
        if (levelStatsString != "")
        {
            currentLevelStats = JsonConvert.DeserializeObject<LevelStats>(levelStatsString, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }
        else
        {
            string levelStatsJsonString1 = JsonConvert.SerializeObject(levelStats, Formatting.Indented,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });

            PlayerPrefs.SetString("levelStats", levelStatsJsonString1);
            currentLevelStats = levelStats;
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
}