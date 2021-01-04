using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    private GameManager gm;
    private MapGenerator map;
    public CoinsManager cm;
    public PathGenarator pathGenarator;
   
    public GameObject gameOverPanel;
    
    public GameObject codeStringPanel;
    
    public TMP_InputField forInput;
    public TMP_InputField waitInput;

    public GameObject mainCamera;
    public Transform cameraTarget;
    
    public GameObject codePanel;
    public GameObject codePanelOpenButton;
    private bool codePanelOpened = false;
    private float codePaneleWidth = 0;

    [HideInInspector] public List<GameObject> codeInputsObjects = new List<GameObject>();
    public GameObject codeMoveObject;
    public GameObject codeForObject;
    public GameObject codeIfObject;
    public GameObject codeWaitObject;
    public GameObject codeIfObjectChild;
    public GameObject panel;
    public GameObject gameOverReplay;

    public TextMeshProUGUI codeString;
    public TextMeshProUGUI countDownText;

    public int commandIndex = 0;
    public Sprite[] gameOverScoreImages;
    public Image gameOverScoreObject;

    void Awake()
    {
        codePaneleWidth = Mathf.Abs(codePanel.transform.position.x);

        gm = FindObjectOfType<GameManager>();
        map = FindObjectOfType<MapGenerator>();
       
        panel = GameObject.Find("CodePanel/Scroll");

        forInput.gameObject.SetActive(false);

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


    #region CodePanel

    public void CodePanelOpen()
    {
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

        codePanelOpenButton.transform.Find("Button/Arrow").transform.Rotate(new Vector3(0, 0, 180));
        codePanelOpened = !codePanelOpened;
    }

    public void ShowCommand(Command command)
    {
        var type = command.GetType();

        if (type == typeof(MoveCommand))
        {
            var moveCommand = (MoveCommand) command;
            ShowKey(moveCommand.direction, commandIndex);
            commandIndex++;
        }
        else if (type == typeof(ForCommand))
        {
            var forCommand = (ForCommand) command;
            ShowKeyForLoop(forCommand.directions, forCommand.loopCount, commandIndex);
            commandIndex++;
        }
        else if (type == typeof(PickIfAnyObjectExistsCommand))
        {
            var ifCommand = (PickIfAnyObjectExistsCommand)command;
            ShowKeyForIf(ifCommand.animalName);
            commandIndex++;
        }
        else if (type == typeof(WaitCommand))
        {
            var waitCommand = (WaitCommand)command;
            ShowKeyForWait(waitCommand.seconds);
            commandIndex++;
        }
    }

    private void ShowKeyForWait(int waitCommandSeconds)
    {
        if (panel == null)
        {
            panel = GameObject.Find("CodePanel/Scroll");
        }

        var codeInputWait = Instantiate(codeWaitObject, codeWaitObject.transform.position, Quaternion.identity);
        codeInputWait.transform.Find("seconds").gameObject.GetComponent<TextMeshProUGUI>().text =
            waitCommandSeconds.ToString();

        codeInputWait.transform.parent = panel.transform;
        codeInputWait.transform.name = commandIndex.ToString();

        codeInputWait.transform.localScale = new Vector3(2.5f, 1f, 1f);

        codeInputsObjects.Add(codeInputWait);
    }

    private void ShowKeyForIf(string ifCommandAnimalName)
    {
        if (panel == null)
        {
            panel = GameObject.Find("CodePanel/Scroll");
        }

        var codeInputIf = Instantiate(codeIfObject, codeIfObject.transform.position, Quaternion.identity);
        codeInputIf.transform.Find("ifObjectName").gameObject.GetComponent<TextMeshProUGUI>().text =
            ifCommandAnimalName;
        codeInputIf.transform.parent = panel.transform;
        codeInputIf.transform.name = commandIndex.ToString();

        codeInputIf.transform.localScale = new Vector3(2.5f, 1, 1);


        var codeInput = Instantiate(codeIfObjectChild, codeIfObjectChild.transform.position, Quaternion.identity);

        codeInput.transform.parent = codeInputIf.transform.Find("CodeInputArea").transform;
        Destroy(codeInput.GetComponent<DeleteCommand>());

        codeInputIf.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(codeInputIf.transform.GetComponent<RectTransform>().sizeDelta.x, codeInputIf.transform.GetComponent<RectTransform>().sizeDelta.y + 75);
        codeInput.transform.localScale = new Vector3(1, 1, 1);

        codeInputsObjects.Add(codeInputIf);

        var image = codeInput.transform.Find("Image").GetComponent<Image>();

        image.sprite = pathGenarator.currentAnimals.animals.ToList().Find(v => v.ifName == ifCommandAnimalName).animalsGameObjectsImage;
        GameObject.Find("CodePanel").GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 0);
        
    }

    private void ShowKeyForLoop(List<Direction> direction, int loopCount, int commandIndex)
    {
        if (panel == null)
        {
            panel = GameObject.Find("CodePanel/Scroll");
        }
        var codeInputFor = Instantiate(codeForObject, codeForObject.transform.position, Quaternion.identity);
        codeInputFor.transform.Find("LoopCountText").gameObject.GetComponent<TextMeshProUGUI>().text =
            loopCount.ToString();
        codeInputFor.transform.parent = panel.transform;
        codeInputFor.transform.name = commandIndex.ToString();
        codeInputFor.transform.localScale = new Vector3(2.5f, 1, 1);
        for (int i = 0; i < direction.Count; i++)
        {
            int keyRotate = SetDirectionRotate(direction[i]);

            var codeInput = Instantiate(codeMoveObject, codeMoveObject.transform.position, Quaternion.identity);
            codeInput.transform.parent = codeInputFor.transform.Find("CodeInputArea").transform;
            Destroy(codeInput.GetComponent<DeleteCommand>());

            codeInputFor.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(codeInputFor.transform.GetComponent<RectTransform>().sizeDelta.x, codeInputFor.transform.GetComponent<RectTransform>().sizeDelta.y+75);
            codeInput.transform.localScale = new Vector3(1, 1, 1);

            codeInputsObjects.Add(codeInputFor);

            var arrow = codeInput.transform.Find("Image");
            arrow.gameObject.transform.Rotate(new Vector3(0, 0, keyRotate));
            GameObject.Find("CodePanel").GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 0);
        }
       
    }

    private void ShowKey(Direction direction, int commandIndex)
    {
        if (panel == null)
        {
            panel = GameObject.Find("CodePanel/Scroll");
        }
        int keyRotate = SetDirectionRotate(direction);

        var codeInput = Instantiate(codeMoveObject, codeMoveObject.transform.position, Quaternion.identity);

        codeInputsObjects.Add(codeInput);
        codeInput.transform.localScale = new Vector3(1, 1, 1);
        codeInput.transform.parent = panel.transform;
        codeInput.transform.name = commandIndex.ToString();
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
        if (gm.isGameOrLoad == 3)
        {
            PlayerPrefs.SetInt("isGameOrLoad", 3);
            if (isGameOrLoad == 0)//In game over panel pressed play button
            {
                PlayerPrefs.SetInt("isRestart", 0);
                var levelsInt = gm.senarioAndLevelIndexs;

                if (levelsInt[2] != 3 )
                {
                    if (gm.character.isPlayerReachedTarget)
                    {
                        levelsInt[2]++;
                        var senarioAndLevelIndexs = levelsInt[0].ToString() + "-" + levelsInt[1].ToString() + "-" + levelsInt[2].ToString();
                        PlayerPrefs.SetString("selcetedLevelProps", senarioAndLevelIndexs);
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    }
                    else
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    }
                    
                }
                else
                {
                    if (gm.character.isPlayerReachedTarget)
                    {
                        SceneManager.LoadScene(1);
                    }
                    else
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    }
                }
            }
            else//In game over panel pressed restart button
            {
                PlayerPrefs.SetInt("isRestart", 1);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            
        }
        else if (gm.isGameOrLoad == 1)
        {
            if (isGameOrLoad == 0) //In game over panel pressed play button
            {
                PlayerPrefs.SetInt("isRestart", 0);
                SceneManager.LoadScene(1); //level maps scene
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        else
        {
            if (isGameOrLoad == 0) //In game over panel pressed play button
            {
                PlayerPrefs.SetInt("isRestart", 0);
                if (gm.currentSubLevel.subLevelName == "3")
                {
                    if (gm.character.isPlayerReachedTarget)
                    {
                        SceneManager.LoadScene(1);//level maps scene
                    }
                    else
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    }
                }
                else
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
            }
            else
            {
                PlayerPrefs.SetInt("isRestart", 1);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    public void OpenGameOverPanel(bool isSuccess)
    {
        gameOverPanel.SetActive(true);
        
        if (isSuccess)
        {
            var index = 0;
            if (gm.currentSubLevel.subLevelName == "1")
                index = 0;
            else if (gm.currentSubLevel.subLevelName == "2")
                index = 1;
            else if (gm.currentSubLevel.subLevelName == "3")
                index = 2;
            gameOverScoreObject.sprite = gameOverScoreImages[index];
            gameOverReplay.GetComponent<Button>().interactable = false;
            cm.StartAnimateCoins();
        }
        else
        {
            gameOverScoreObject.sprite = gameOverScoreImages[3];
        }
    }

    public void HomeButton()
    {
        SceneManager.LoadScene(1);
    }

    public void ShowWrongCommand(int wrongCommandIndex)
    {
        codeInputsObjects[wrongCommandIndex].GetComponent<Image>().color = Color.red;
        if (!codePanelOpened)
        {
            CodePanelOpen();
        }
    }

    private GameObject hintObject;
    public void ShowHintCommand(int nextCommandIndex)
    {
        int keyRotate = SetDirectionRotate(pathGenarator.Path[nextCommandIndex].pathDirection);
        
        hintObject = Instantiate(codeMoveObject, codeMoveObject.transform.position, Quaternion.identity);

        hintObject.transform.localScale = new Vector3(1, 1, 1);
        hintObject.transform.parent = GameObject.Find("Canvas").transform;

        Destroy(hintObject.GetComponent<DeleteCommand>());

        var arrow = hintObject.transform.Find("Image");
        arrow.gameObject.transform.Rotate(new Vector3(0, 0, keyRotate));

        hintObject.GetComponent<RectTransform>().SetAnchor(AnchorPresets.MiddleCenter);

        hintObject.GetComponent<RectTransform>().SetPivot(PivotPresets.MiddleCenter);

        Invoke("DeleteHintCommandStarter",2f);
    }

    private void DeleteHintCommandStarter()
    {
        //StartCoroutine(DeleteHintCommand());
        var arrow = hintObject.transform.Find("Image").GetComponent<Image>();
        arrow.DOFade(0f, 1f);
        hintObject.GetComponent<Image>().DOFade(0f, 1f).OnComplete(() =>
        {
            Destroy(hintObject);
        });
    }

    public void MarkCurrentCommand(int i)
    {
        codeInputsObjects[i].GetComponent<Image>().color = new Color(163 / 255, 255 / 255, 131 / 255);
        if (i != 0)
            codeInputsObjects[i - 1].GetComponent<Image>().color = Color.white;
    }

    public void StartCleaningCountDown(MapGenerator.Coord realCoord, int seconds)
    {
        countDownText.color = Color.white;
        countDownText.gameObject.SetActive(true);
        var expectedSeconds = realCoord.whichDirt.seconds;
        StartCoroutine(CountDownStart(seconds, expectedSeconds));
    }

    public IEnumerator CountDownStart(int expectedSeconds, int seconds)
    {
        var diffrenceSeconds = Mathf.Abs(expectedSeconds - seconds);
        for (int i = seconds; i >= diffrenceSeconds; i--)
        {
            countDownText.text = i.ToString();
            if ( i == diffrenceSeconds && diffrenceSeconds != 0)
            {
                countDownText.color = Color.red;
            }
            yield return new WaitForSeconds(1);
        }

        countDownText.gameObject.SetActive(false);
    }
}