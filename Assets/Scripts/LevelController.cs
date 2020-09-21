using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public LevelLoader levelLoader;

    [Serializable]
    public class Senario
    {
        public Button[] allLevelButtons;
    }

    public Senario[] allButtons;
    private bool lastLevel;
    public string lastLevelIndexs;


    void Awake()
    {
        levelLoader.SetLevels();
        UnlockAllPassedLevels();
        
        PlayerPrefs.SetInt("isGameOrLoad", 0);
    }

    public void UnlockAllPassedLevels()
    {
        for (int i = 0; i < levelLoader.currentLevelStats.senarios.Length; i++)
        {
            for (int j = 0; j < levelLoader.currentLevelStats.senarios[i].levels.Length; j++)
            {
                var currenLevel = levelLoader.currentLevelStats.GetLevel(i+1,j+1);

                allButtons[i].allLevelButtons[j].interactable = true;

                var count = 0;
                foreach (Transform stars in allButtons[i].allLevelButtons[j].gameObject.transform)
                {
                    if (currenLevel.subLevels[count].passed)
                    {
                        stars.GetComponent<Image>().color = Color.white;
                    }

                    count++;
                }

                if (!currenLevel.levelComplated)
                {
                    lastLevelIndexs = i + "-" + j;
                    lastLevel = true;
                    break;
                }
            }
            if (lastLevel)
            {
                break;
            }
        }
    }

    public void LevelButtonClick(string levelIndexs)
    {
        string[] levelsString = levelIndexs.Split('-');

        var levelsInt = levelsString.Select(int.Parse).ToArray();

        var selectedLevel = levelLoader.currentLevelStats.GetLevel(levelsInt[0], levelsInt[1]);

        var senarioAndLevelIndexs = levelsInt[0].ToString()+"-"+ levelsInt[1].ToString()+"-"+ 1.ToString();

        if (selectedLevel.levelComplated)
        {
            //Player wants to play again this level complated
           
            PlayerPrefs.SetString("selcetedLevelProps", senarioAndLevelIndexs);
            PlayerPrefs.SetInt("isGameOrLoad",3);
            SceneManager.LoadScene("Game");
        }
        else
        {
            PlayerPrefs.SetInt("isGameOrLoad", 0);
            SceneManager.LoadScene("Game");
        }
    }

    public void HomeButton()
    {
        SceneManager.LoadScene(0);
    }
}
