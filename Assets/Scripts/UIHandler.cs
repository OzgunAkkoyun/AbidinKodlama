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
    public GetInputs GetInputs;
    private MapGenerator map;
    public GameObject minimap;
    private GameObject minimapTexture;
    private GameObject miniMapGraphics;
    public GameObject gameOverPanel;
    public GameObject videoPanel;
    public GameObject codeStringPanel;
    private GameObject video;
    public AllVideos allVideos;

    public TMP_InputField forInput;

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
    public GameObject codeMoveObject;
    public GameObject codeForObject;

    public TextMeshProUGUI codeString;

    private float screenW;
    private float screenH;

    void Awake()
    {
        codePaneleWidth = Mathf.Abs(codePanel.transform.position.x);
        gm = FindObjectOfType<GameManager>();
        map = FindObjectOfType<MapGenerator>();
        minimapTexture = minimap.transform.Find("MiniMapGraphics/Texture").gameObject;
        miniMapGraphics = minimap.transform.Find("MiniMapGraphics").gameObject;
        screenW = Screen.width;
        screenH = Screen.height;
        forInput.gameObject.SetActive(false);
        MiniMapSizeSet();
        miniMapCamera.transform.position = new Vector3( map.currentMap.mapSize.x-1, miniMapCamera.transform.position.y, map.currentMap.mapSize.y-1);
        miniMapCamera.orthographicSize = miniMapCamera.transform.position.x + 4;
    }

    void Start()
    {
       gm.commander.OnNewCommand.AddListener(OnNewCommand);
    }

    void OnDestroy()
    {
        gm.commander.OnNewCommand.RemoveListener(OnNewCommand);
    }

    private void OnNewCommand()
    {
        var newCommand = gm.commander.commands.Last();
        
        ShowCommand(newCommand);
    }

    #region Video
    public void ShowVideo(string videoName)
    {
        gm = FindObjectOfType<GameManager>();

        var pickedVideo = allVideos.GetVideo(gm.playerDatas.whichScenario, videoName);

        //var pickedVideo = Array.Find(allVideos.senarioVideos[gm.playerDatas.whichScenario - 1].videos,
        //    element => element.name == videoName).video;

        if (pickedVideo == null)
            return;
        videoPanel.SetActive(true);
        video = videoPanel.transform.Find("VideoPlayer").gameObject;
        video.GetComponent<VideoPlayer>().clip = pickedVideo;
        video.GetComponent<VideoPlayer>().loopPointReached += CloseVideo;
    }

    public void CloseVideo(VideoPlayer vp)
    {
        StartCoroutine("VideoFadeOut");
        gm.sc.Play("Theme");
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

            videoRawImage.GetComponent<RawImage>().color = curColor;

            yield return new WaitForSeconds(0);
        }

        videoPanel.SetActive(false);
    }

    #endregion

    #region MiniMap

    void MiniMapSizeSet()
    {
        miniMapGraphics.GetComponent<RectTransform>().sizeDelta = new Vector2(screenW - 100, screenH );
        RatiosForMiniMap();
    }

    void RatiosForMiniMap()
    {
        var textureSize = (float) (miniMapGraphics.GetComponent<RectTransform>().sizeDelta.y - miniMapGraphics.GetComponent<RectTransform>().sizeDelta.y * 0.2);
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
            miniMapGraphics.GetComponent<RectTransform>().sizeDelta = new Vector2(
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
                minimap.transform.localScale = minimap.transform.localScale + new Vector3(mapSizeMultiplier, mapSizeMultiplier, mapSizeMultiplier);
                yield return new WaitForSeconds(0f);
            }
            else
            {
                minimap.transform.localScale = minimap.transform.localScale - new Vector3(mapSizeMultiplier, mapSizeMultiplier, mapSizeMultiplier);
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

    public void ShowCommand(Command command)
    {
        Debug.Log(command);
        var type = command.GetType();

        if (type == typeof(MoveCommand))
        {
            var moveCommand = (MoveCommand)command;
            ShowKey(moveCommand.direction);
        }
        else if(type == typeof(ForCommand))
        {
            var forCommand = (ForCommand) command;
            ShowKeyForLoop(forCommand.direction,forCommand.loopCount);
        }
    }
    private void ShowKeyForLoop(Direction direction,int loopCount)
    {
        Debug.Log("for");
        int keyRotate = SetDirectionRotate(direction);

        var panel = GameObject.Find("CodePanel/Scroll");

        var codeInputFor = Instantiate(codeForObject, codeForObject.transform.position, Quaternion.identity);

        var codeInput = Instantiate(codeMoveObject, codeMoveObject.transform.position, Quaternion.identity);
        codeInput.transform.parent = codeInputFor.transform.Find("CodeInputArea").transform;

        codeInputFor.transform.Find("LoopCountText").gameObject.GetComponent<TextMeshProUGUI>().text = loopCount.ToString();
        codeInput.transform.localScale = new Vector3(1, 1, 1);

        codeInputsObjects.Add(codeInputFor);
        codeInputFor.transform.localScale = new Vector3(1, 1, 1);
        codeInputFor.transform.parent = panel.transform;

        var arrow = codeInput.transform.Find("Image");
        arrow.gameObject.transform.Rotate(new Vector3(0, 0, keyRotate));
        GameObject.Find("CodePanel").GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 0);
    }
    private void ShowKey(Direction direction)
    {
        int keyRotate = SetDirectionRotate(direction);

        var panel = GameObject.Find("CodePanel/Scroll");
        var codeInput = Instantiate(codeMoveObject, codeMoveObject.transform.position, Quaternion.identity);

        codeInputsObjects.Add(codeInput);
        codeInput.transform.localScale = new Vector3(1, 1, 1);
        codeInput.transform.parent = panel.transform;
        var arrow = codeInput.transform.Find("Image");
        arrow.gameObject.transform.Rotate(new Vector3(0, 0, keyRotate));
        GameObject.Find("CodePanel").GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 0);
    }

    private int SetDirectionRotate(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                return 180;
                break;
            case Direction.Right:
                return 0;
                break;
            case Direction.Forward:
                return 90;
                break;
            case Direction.Backward:
                return 270;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    #endregion

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

    public void HomeButton()
    {
        SceneManager.LoadScene(0);
    }
}
