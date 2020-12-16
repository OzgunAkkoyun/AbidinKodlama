using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening.Plugins.Core.PathCore;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityScript.Steps;

public class DeleteCommand : MonoBehaviour, IPointerClickHandler
{
    public GameManager gm;
    public UIHandler uh;
    public PathGenarator pathGenarator;

    public void Start()
    {
        gm = FindObjectOfType<GameManager>();
        uh = FindObjectOfType<UIHandler>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var commandIndex = FindIndexOfChild(this.gameObject.name);
        gm.commander.commands.RemoveAt(commandIndex);
        uh.codeInputsObjects.RemoveAt(commandIndex);
        Destroy(gameObject);
    }

    public void OnBackButton()
    {
        var commandIndex = gm.commander.commands.Count-1;
        if (commandIndex<0) return;

        gm.commander.commands.RemoveAt(commandIndex);
        Destroy(uh.codeInputsObjects[commandIndex]);
        uh.codeInputsObjects.RemoveAt(commandIndex);
    }

    public void DeleteAllCommands()
    {
        var commands = gm.commander.commands;
        var commandCount = gm.commander.commands.Count;
        if (commandCount == 0) return;
        
        for (int i = 0; i < commandCount; i++)
        {
            Destroy(uh.codeInputsObjects[i]);
        }
        gm.commander.commands.Clear();
        uh.codeInputsObjects.Clear();
        pathGenarator.selectedAnimals.Clear();
    }

    private int FindIndexOfChild(string parse)
    {
        for (int i = 0; i < uh.panel.transform.childCount; i++)
        {
            if (uh.panel.transform.GetChild(i).name == parse)
            {
                return i-1;
                break;
            }
        }
        return -1;
    }
}