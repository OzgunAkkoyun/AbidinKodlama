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

    public bool waitingMoveCommand;

    public int forLoopCount;

    public Commander commander;

    private void Start()
    {
        
    }

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
                commander.AddForCommand(Direction.Forward, forLoopCount);
                waitingMoveCommand = false;
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
                commander.AddForCommand(Direction.Left, forLoopCount);
                waitingMoveCommand = false;
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
                commander.AddForCommand(Direction.Right, forLoopCount);
                waitingMoveCommand = false;
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
                commander.AddForCommand(Direction.Backward, forLoopCount);
                waitingMoveCommand = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            waitingMoveCommand = true;
            WaitForInput();
        }
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

    public void SetForLoopCount(string count)
    {
        forLoopCount = int.Parse(count);
        uh.forInput.gameObject.SetActive(false);
    }
}