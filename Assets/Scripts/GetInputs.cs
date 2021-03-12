using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum Direction
{
    Left, Right, Forward, Backward,Empty
}

public class GetInputs : MonoBehaviour
{
    public UIHandler uh;
    public RotateToyUi rotateToyUi;

    public bool waitingMoveCommand;

    public int forLoopCount;
    public List<int> seconds = new List<int>();

    public Commander commander;

    public List<Direction> forDirections ;

    private bool isFirstCommand;

    public Timer timer;


    private List<Vector2> touchList = new List<Vector2>();
    public AllToys toolList;

    public bool canUseToy { get; private set; }

    bool buttonPressed;
    int touchCountCorrect = 3;
    public GameObject toyPanel;
    
    private float rotationSensivity = 3;
    public float maxAngle;
    public float rotationAngle;
    public float singleObjectAngle = 30;
    public bool canUseRotate = false;
    private List<Touch> touchListforRotate = new List<Touch>();
    public List<GameObject> rotateObjects = new List<GameObject>();

    private int selectedObjectIndex = 0;
    public TextMeshProUGUI rotatetext;

    public void TryToy()
    {
        touchList.Clear();
        touchList.Add(new Vector2(1019.2f, 333.6f));
        touchList.Add(new Vector2(957.3f, 558.3f));
        touchList.Add(new Vector2(796.4f, 393.5f));

        var p1 = new Vector2(touchList[0].x, touchList[0].y);
        var p2 = new Vector2(touchList[1].x, touchList[1].y);
        var p3 = new Vector2(touchList[2].x, touchList[2].y);

        string toolName = toolList.IdentifyTool(p1, p2, p3);
        
        Vector2 center = (p1 + p2 + p3) / 3.0f;
        UseTool(toolName, center);
    }
    void Update()
    {
#if UNITY_EDITOR
        GetKeys();
#endif
#if UNITY_IOS
      HandleUserInput();
#endif
#if UNITY_ANDROID
      HandleUserInput();
#endif

    }
    private void HandleUserInput()
    {
        float scwidth = Screen.width;
        float scheight = Screen.height;
        int ct = Input.touches.Length;

        if (ct == 0 || ct == 1)
        {
            canUseToy = true;
            canUseRotate = false;
        }
        if (ct == touchCountCorrect)
        {
            touchList.Clear();

            for (int i = touchCountCorrect - 3; i < Input.touches.Length; i++)
            {
                TouchPhase phase = Input.touches[i].phase;

                if (phase != TouchPhase.Ended)
                {
                    touchList.Add(new Vector2(Input.touches[i].position.x, Input.touches[i].position.y));
                }
            }
        }
        
        if (ct == touchCountCorrect && canUseToy)
        {
            canUseToy = false;

            Vector2 p1, p2, p3;

            p1 = new Vector2(touchList[0].x, touchList[0].y);
            p2 = new Vector2(touchList[1].x, touchList[1].y);
            p3 = new Vector2(touchList[2].x, touchList[2].y);

            string toolName = toolList.IdentifyTool(p1, p2, p3);

            Vector2 center = (p1 + p2 + p3) / 3.0f;
            UseTool(toolName, center);
        }

        if (canUseRotate)
        {
            ToyRotate();
        }
    }
    void ToyRotate(bool isCircle = false)
    {
        touchListforRotate = Input.touches.ToList();
        var prevPos1 =
            touchListforRotate[0].position -
            touchListforRotate[0].deltaPosition; // Generate previous frame's finger positions
        var prevPos2 = touchListforRotate[1].position - touchListforRotate[1].deltaPosition;

        var prevDir = prevPos2 - prevPos1;
        var currDir = touchListforRotate[1].position - touchListforRotate[0].position;

        var angle = Vector2.Angle(prevDir, currDir);

        var a = Vector3.Cross(prevDir.ToVector3(), currDir.ToVector3());

        rotationAngle += a.z > 0 ? -angle * rotationSensivity : angle * rotationSensivity;

        rotationAngle = rotationAngle > maxAngle ? maxAngle : rotationAngle;
        rotationAngle = rotationAngle < 0 ? 0 : rotationAngle;

        rotatetext.text = "Rotate: " + rotationAngle.ToString("F2");

        RotationToySelect(rotationAngle);
    }

    private void RotationToySelect(float angle)
    {
        selectedObjectIndex = (int)Mathf.Floor(angle / singleObjectAngle);
        rotatetext.text += " " + selectedObjectIndex;
        rotateObjects[selectedObjectIndex].GetComponent<Image>().color = Color.red;

        foreach (var item in rotateObjects.Where((v, index) => index != selectedObjectIndex).ToList())
        {
            item.GetComponent<Image>().color = Color.white;
        }

    }
    private void UseTool(string toolName, Vector2 toolPosition)
    {
        SoundController.instance.Play("touch");
        if (toolName == "Forward")
        {
            if (!waitingMoveCommand)
            {
                commander.AddMoveCommand(Direction.Forward);
            }
            else
            {
                forDirections.Add(Direction.Forward);
            }
        }
        else if (toolName == "Left")
        {
            if (!waitingMoveCommand)
            {
                commander.AddMoveCommand(Direction.Left);
            }
            else
            {
                forDirections.Add(Direction.Left);
            }
        }
        else if (toolName == "Right")
        {
            if (!waitingMoveCommand)
            {
                commander.AddMoveCommand(Direction.Right);
            }
            else
            {
                forDirections.Add(Direction.Right);
            }
        }
        else if (toolName == "Backward")
        {
            if (!waitingMoveCommand)
            {
                commander.AddMoveCommand(Direction.Backward);
            }
            else
            {
                forDirections.Add(Direction.Backward);
            }
        }
        else if (toolName == "Loop")
        {
            forDirections = new List<Direction>();
            waitingMoveCommand = true;
            WaitForInput();
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            if (commander != null) commander.AddForCommand(forDirections, forLoopCount);
            waitingMoveCommand = false;
        }
        else if (toolName == "Condition")
        {
            //rotateToyUi.OpenIfObjectContainer();
            canUseRotate = true;
            rotateToyUi.OpenIfObjectWheel();
        }
        else if (toolName == "Wait")
        {
            //rotateToyUi.OpenIfObjectContainer();
            canUseRotate = true;
            WaitWaitInput();
        }

    }
    public void GetKeys()
    {
        if (Input.anyKey)
        {
            if (!isFirstCommand)
            {
                isFirstCommand = true;
                timer = new Timer();
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) )
        {
            if (!waitingMoveCommand)
            {
                commander.AddMoveCommand(Direction.Forward);
            }
            else
            {
                forDirections.Add(Direction.Forward);
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (!waitingMoveCommand)
            {
                commander.AddMoveCommand(Direction.Left);
            }
            else
            {
                forDirections.Add(Direction.Left);
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) )
        {
            if (!waitingMoveCommand)
            {
                commander.AddMoveCommand(Direction.Right);
            }
            else
            {
                forDirections.Add(Direction.Right);
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (!waitingMoveCommand)
            {
                commander.AddMoveCommand(Direction.Backward);
            }
            else
            {
                forDirections.Add(Direction.Backward);
            }
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            forDirections = new List<Direction>();
            waitingMoveCommand = true;
            WaitForInput();
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            if (commander != null) commander.AddForCommand(forDirections, forLoopCount);
            waitingMoveCommand = false;
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            //rotateToyUi.OpenIfObjectContainer();
            rotateToyUi.OpenIfObjectWheel();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            //rotateToyUi.OpenIfObjectContainer();
            WaitWaitInput();
        }
    }

    private void WaitWaitInput()
    {
        uh.waitInput.gameObject.SetActive(true);
    }

    public void WaitListener()
    {
        var currentSecond = int.Parse(EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>().text);

        commander.AddWaitCommand(currentSecond);
        seconds.Add(currentSecond);
        uh.waitInput.gameObject.SetActive(false);
    }

    public void GetIfInput(string animalName)
    {
        commander.AddIfCommand(animalName);
    }

    private void WaitForInput()
    {
        uh.forInput.gameObject.SetActive(true);
        uh.forInput.onValueChanged.AddListener(delegate (string text)
        {
            if (!EventSystem.current.alreadySelecting)
            {
                SetForLoopCount(EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>().text);
            }
        });
    }

    private void SetForLoopCount(string count)
    {
        forLoopCount = int.Parse(count);
        uh.forInput.gameObject.SetActive(false);
    }
}