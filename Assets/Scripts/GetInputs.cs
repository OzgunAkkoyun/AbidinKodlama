using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GetInputs : MonoBehaviour
{
    [HideInInspector]
    public enum code { Left, Right, Forward, Backward, If, For, Time };

    public List<code> inputs = new List<code>();

    private GameManager gm;

    public bool waitingInput;

    public int forLoopCount;

    public UnityEvent MyEvent;


    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        MyEvent.AddListener(SetForLoopInput);
    }

    void Update()
    {
        GetKeys();
    }

    public void GetKeys()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && !waitingInput)
        {
            inputs.Add(code.Forward);
            gm.uh.ShowKeys(90);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && !waitingInput)
        {
            inputs.Add(code.Left);
            gm.uh.ShowKeys(180);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && !waitingInput)
        {
            inputs.Add(code.Right);
            gm.uh.ShowKeys(0);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && !waitingInput)
        {
            inputs.Add(code.Backward);
            gm.uh.ShowKeys(-90);
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            waitingInput = true;
            gm.uh.forInput.gameObject.SetActive(true);
            
            WaitForInput();
        }
    }

    private void WaitForInput()
    {
        gm.uh.forInput.onValueChanged.AddListener(delegate (string text) {
            if (!EventSystem.current.alreadySelecting)
            {
                SetForLoopCount(EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>().text);
            }
        });
    }

    public void SetForLoopCount(string count)
    {
        forLoopCount = int.Parse(count);
        //SetForLoopInput();
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MyEvent.Invoke();
        }
    }

    private void SetForLoopInput()
    {
        Debug.Log("deneme");
    }
}