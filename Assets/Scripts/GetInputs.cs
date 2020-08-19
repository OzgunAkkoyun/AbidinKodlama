using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum Direction
{
    Left, Right, Forward, Backward
}

public class GetInputs : MonoBehaviour
{
    //[HideInInspector]
    //public enum code { Left, Right, Forward, Backward, If, For, Time };

    //public List<Direction> inputs = new List<Direction>();

    //private GameManager gm;
    public UIHandler uh;

    public bool waitingMoveCommand;

    public int forLoopCount;

    //public UnityEvent MyEvent;

    public Commander commander;

    //public UnityEvent OnNewInput = new UnityEvent();

    private void Start()
    {
        //gm = FindObjectOfType<GameManager>();
        //MyEvent.AddListener(SetForLoopInput);
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
            
            //inputs.Add(Direction.Forward);
            //OnNewInput.Invoke();
            //gm.uh.ShowKeys(90);
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
            //inputs.Add(Direction.Left);
            //OnNewInput.Invoke();
            //gm.uh.ShowKeys(180);
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
            
            //inputs.Add(Direction.Right);
            //OnNewInput.Invoke();
            //gm.uh.ShowKeys(0);
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
            //inputs.Add(Direction.Backward);
            //OnNewInput.Invoke();
            //gm.uh.ShowKeys(-90);
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            waitingMoveCommand = true;
            WaitForInput();
            //commander.AddForCommand(Direction.Backward,3);
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

    //private void SetForLoopInput()
    //{
    //    Debug.Log("deneme");
    //}
}