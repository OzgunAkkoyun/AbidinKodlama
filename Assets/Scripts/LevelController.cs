using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelController : MonoBehaviour
{
    public LevelLoader levelLoader;
    void Start()
    {
        levelLoader.SetLevels();
    }

    public void LevelButtonClick(int levelIndex)
    {
        var selectedButton = EventSystem.current.currentSelectedGameObject;

        var selectedLevel = levelLoader.currentLevelStats.GetLevel(levelLoader.playerDatas.whichScenario, levelIndex);

        if (selectedLevel.levelComplated)
        {
            //Player wants to play again this level complated
        }
        else
        {
            //Player not complated level so, get which sub level do they remain
        }
    }
}
