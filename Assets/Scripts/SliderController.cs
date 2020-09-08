using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.PlayerLoop;

public class SliderController : MonoBehaviour
{
    public GameObject mapsContainer;
    public float duration = 1;
    [SerializeField] Ease easeType;

    RectTransform rectTransform;

    private int clikCount;
    public SavedPlayerData playerDatas;

    void Start()
    {
        rectTransform = mapsContainer.GetComponent<RectTransform>();
        rectTransform.DOAnchorPosX(0, 0f);
        var playerDataString = PlayerPrefs.GetString("playerDatas");
        if (playerDataString != "")
        {
            playerDatas = JsonUtility.FromJson<SavedPlayerData>(playerDataString);
        }
        else
        {
            playerDatas = new SavedPlayerData(0, 0, 1,1,1, 5, 0, 0, false);
        }

        clikCount = -1;//playerDatas.whichScenario*-1;
       
        Invoke("Left",1f);

    }
    public void Right() { clikCount--; MoveMaps(); }
    public void Left() { clikCount++; MoveMaps(); }

    void MoveMaps()
    {
        if (clikCount>=-4 )
        {
            if (clikCount <= 0)
            {
                rectTransform.DOAnchorPosX(rectTransform.rect.width * clikCount, 0.5f).SetDelay(0.1f);
            }
            else
            {
                clikCount--;
            }
        }
        else
        {
            clikCount++;
        }
        
    }

    public void OpenSubLevels()
    {
        var clikedLevel = EventSystem.current.currentSelectedGameObject;
        StartCoroutine(OpenSubLevelsAnim(clikedLevel));
    }

  
    private Vector2 subLevelsPos;
    private float subLevelAnimSpeed = 0.15f;
    private IEnumerator OpenSubLevelsAnim(GameObject clikedLevel)
    {
        foreach (Transform subLevel in clikedLevel.transform)
        {
            subLevel.gameObject.SetActive(true);
            var subLevelRect = subLevel.gameObject.GetComponent<RectTransform>();
            var subLevelSiblingIndex = subLevel.GetSiblingIndex();
            if (subLevelSiblingIndex == 0)
            {
                subLevelsPos = new Vector2(-130, -130);
            }
            else if (subLevelSiblingIndex == 1)
            {
                subLevelsPos = new Vector2(0, -175);
            }
            else
            {
                subLevelsPos = new Vector2(130, -130);
            }

            subLevelRect.DOAnchorPos(subLevelsPos, subLevelAnimSpeed);
            yield return new WaitForSeconds(subLevelAnimSpeed);

        }

    }
}
