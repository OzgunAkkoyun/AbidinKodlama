using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class UIHandler : MonoBehaviour
{
    private GameManager gm;
    private MapGenerator map;
    public GameObject minimap;
    private GameObject minimapTexture;
    private GameObject pipBoy;
    public GameObject gameOverPanel;
    public GameObject videoPanel;
    public GameObject codeStringPanel;
    private GameObject video;
    public VideoContainer allVideos;

    public Camera miniMapCamera;
    public GameObject mainCamera;
    public Transform cameraTarget;
    private bool mapZoomed = false;
    private float mapSizeMultiplier = 0.15f;

    public GameObject codePanel;
    public GameObject codePanelOpenButton;
    private bool codePanelOpened = false;
    private float codePaneleWidth = 0;

    [HideInInspector]
    public List<GameObject> codeInputsObjects = new List<GameObject>();
    public GameObject codeInputObject;

    public TextMeshProUGUI codeString;

    private float screenW = Screen.width;
    private float screenH = Screen.height;

    private void Start()
    {
        codePaneleWidth = Mathf.Abs(codePanel.transform.position.x);
        gm = FindObjectOfType<GameManager>();
        map = FindObjectOfType<MapGenerator>();
        minimapTexture = minimap.transform.Find("MiniMapGraphics/Texture").gameObject;
        pipBoy = minimap.transform.Find("MiniMapGraphics").gameObject;

        

        //allVideos.videos.Find(v => v.name == "1-1");

        MiniMapSizeSet();
        miniMapCamera.transform.position = new Vector3( map.currentMap.mapSize.x-1, miniMapCamera.transform.position.y, map.currentMap.mapSize.y-1);
        miniMapCamera.orthographicSize = miniMapCamera.transform.position.x + 4;
    }

    public void ShowVideo(string videoName)
    {
        videoPanel.SetActive(true);
        video = videoPanel.transform.Find("VideoPlayer").gameObject;
        video.GetComponent<VideoPlayer>().clip = Array.Find(allVideos.videos, element => element.name == videoName).video;
        video.GetComponent<VideoPlayer>().loopPointReached += CloseVideo;
    }

    public void CloseVideo(VideoPlayer vp)
    {
        StartCoroutine("VideoFadeOut");
        gm.playerDatas.showedOpeningVideo = true;
        gm.PlayerDataSave();
    }

    IEnumerator VideoFadeOut()
    {
        var videoRawImage = videoPanel.transform.Find("RawImage");
        Color curColor = videoRawImage.GetComponent<RawImage>().color;
        var targetAlpha = 0;
        while (curColor.a > 0)
        {
            curColor = videoRawImage.GetComponent<RawImage>().color;
            float alphaDiff = Mathf.Abs(curColor.a - targetAlpha);
            //curColor.a = Mathf.Lerp(curColor.a, targetAlpha, 5 * Time.deltaTime);
            curColor.a -= 1.1f * Time.deltaTime;
            Debug.Log(curColor.a);
            videoRawImage.GetComponent<RawImage>().color = curColor;

            yield return new WaitForSeconds(0);
        }
        
        videoPanel.SetActive(false);
    }

    public IEnumerator CameraSmoothMovingToTargetPosition()
    {
        float t = 0;
        while (Vector3.Distance(mainCamera.transform.position, cameraTarget.position) > 0.1f)
        {
            t += Time.deltaTime / 30;
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraTarget.position, t);
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, cameraTarget.rotation, t);

            yield return new WaitForSeconds(0f);
        }
    }

    #region MiniMap

    void MiniMapSizeSet()
    {
        pipBoy.GetComponent<RectTransform>().sizeDelta = new Vector2(screenH - 150, screenH - 150);
        RatiosForMiniMap();
    }

    void RatiosForMiniMap()
    {
        var textureSize = (float)(pipBoy.GetComponent<RectTransform>().sizeDelta.y - pipBoy.GetComponent<RectTransform>().sizeDelta.y * 0.3);
        minimapTexture.GetComponent<RectTransform>().sizeDelta = new Vector2(textureSize, textureSize);
    }

    public void MiniMapZoom()
    {
        if (gm.is3DStarted)
            StartCoroutine(MiniMapSizeChange());
    }

    public IEnumerator MiniMapSetStartPosition()
    {
        minimap.GetComponent<RectTransform>().SetAnchor(AnchorPresets.TopRight);
        minimap.GetComponent<RectTransform>().SetPivot(PivotPresets.TopRight);
        minimap.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);

        float t = 0;
        while (true)
        {
            t += Time.deltaTime / 10;
            minimap.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(minimap.GetComponent<RectTransform>().sizeDelta, new Vector2(300, 300), t);
            minimap.transform.GetChild(0).localScale = Vector2.Lerp(minimap.transform.GetChild(0).localScale, new Vector3(1, 1, 1), t * 2);
            pipBoy.GetComponent<RectTransform>().sizeDelta = new Vector2(
                (minimap.GetComponent<RectTransform>().sizeDelta.y - 20 - 1 * 30),
                (minimap.GetComponent<RectTransform>().sizeDelta.y - 20 - 1 * 30));
            RatiosForMiniMap();
            if (Mathf.Round(minimap.GetComponent<RectTransform>().sizeDelta.x) == 300)
                yield break;
            yield return new WaitForSeconds(0f);
        }

    }

    public IEnumerator MiniMapSizeChange()
    {
        for (int i = 0; i < 10; i++)
        {
            if (!mapZoomed)
            {
                minimap.transform.localScale = minimap.transform.localScale +
                                               new Vector3(mapSizeMultiplier, mapSizeMultiplier, mapSizeMultiplier);
                yield return new WaitForSeconds(0f);
            }
            else
            {
                minimap.transform.localScale = minimap.transform.localScale -
                                               new Vector3(mapSizeMultiplier, mapSizeMultiplier, mapSizeMultiplier);
                yield return new WaitForSeconds(0f);
            }
        }

        mapZoomed = !mapZoomed;
    }

    #endregion

    #region CodePanel

    public void CodePanelOpen()
    {
        //var clickedObject = EventSystem.current.currentSelectedGameObject;
        StartCoroutine(CodePanel());
    }

    public void CodeStringPanelOpen()
    {
        codeStringPanel.SetActive(!codeStringPanel.activeSelf);
    }

    public IEnumerator CodePanel()
    {
        for (int i = 0; i < 10; i++)
        {
            if (!codePanelOpened)
            {
                codePanel.transform.localPosition =
                    codePanel.transform.localPosition + new Vector3(codePaneleWidth / 10, 0, 0);
                codePanelOpenButton.transform.localPosition =
                    codePanelOpenButton.transform.localPosition + new Vector3(codePaneleWidth / 10, 0, 0);

                yield return new WaitForSeconds(0f);
            }
            else
            {
                codePanel.transform.localPosition =
                    codePanel.transform.localPosition - new Vector3(codePaneleWidth / 10, 0, 0);
                codePanelOpenButton.transform.localPosition =
                    codePanelOpenButton.transform.localPosition - new Vector3(codePaneleWidth / 10, 0, 0);
                yield return new WaitForSeconds(0f);
            }
        }

        codePanelOpenButton.transform.Rotate(new Vector3(0, 0, 180));
        codePanelOpened = !codePanelOpened;
    }

    public void ShowKeys(int keyRotate)
    {
        var panel = GameObject.Find("CodePanel/Scroll");
        var codeInput = Instantiate(codeInputObject, codeInputObject.transform.position, Quaternion.identity);
        codeInputsObjects.Add(codeInput);
        codeInput.transform.localScale = new Vector3(1, 1, 1);
        codeInput.transform.parent = panel.transform;
        var arrow = codeInput.transform.Find("Image");
        arrow.gameObject.transform.Rotate(new Vector3(0, 0, keyRotate));
        GameObject.Find("CodePanel").GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 0);
    }

    #endregion

    public void RestartOrNewGame(int isGameOrLoad)
    {
        PlayerPrefs.SetInt("isGameOrLoad",isGameOrLoad);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OpenGameOverPanel(bool isSuccess)
    {
        gameOverPanel.SetActive(true);
        var gameOverText = gameOverPanel.transform.Find("Text");
        var gameOverReplay = gameOverPanel.transform.Find("RestartButton");

        if (isSuccess)
        {
            gameOverText.GetComponent<TextMeshProUGUI>().text = "Başarılı!";
            gameOverReplay.GetComponent<Button>().interactable = false;
        }
        else
        {
            gameOverText.GetComponent<TextMeshProUGUI>().text = "Başarısız!";
        }
    }
}
