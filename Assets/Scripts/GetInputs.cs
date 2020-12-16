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
    public int seconds;

    public Commander commander;

    public List<Direction> forDirections ;

    void Update()
    {
        GetKeys();
    }

    public void GetKeys()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) )
        {
            if (!waitingMoveCommand)
            {
                commander.AddMoveCommand(Direction.Forward);
            }
            else
            {
                //commander.AddForCommand(Direction.Forward, forLoopCount);
                forDirections.Add(Direction.Forward);
                //waitingMoveCommand = false;
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
                //commander.AddForCommand(Direction.Left, forLoopCount);
                forDirections.Add(Direction.Left);
                //waitingMoveCommand = false;
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
                //commander.AddForCommand(Direction.Right, forLoopCount);
                forDirections.Add(Direction.Right);
                //waitingMoveCommand = false;
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
                //commander.AddForCommand(Direction.Backward, forLoopCount);
                forDirections.Add(Direction.Backward);
                //waitingMoveCommand = false;
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
            seconds = currentSecond;
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