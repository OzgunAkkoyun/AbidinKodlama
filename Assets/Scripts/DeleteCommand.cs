using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityScript.Steps;

public class DeleteCommand : MonoBehaviour, IPointerClickHandler
{
    public GameManager gm;
    public UIHandler uh;

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