using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

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
    void Update()
    {
        GetKeys();
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
        var currentSecond =
                int.Parse(EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>().text);
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