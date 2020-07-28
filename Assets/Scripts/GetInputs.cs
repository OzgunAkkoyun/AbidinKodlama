using System.Collections.Generic;
using UnityEngine;

public class GetInputs : MonoBehaviour
{
    [HideInInspector]
    public enum code { Left, Right, Forward, Backward, If, For, Time };

    public List<code> inputs = new List<code>();

    private GameManager gm;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        GetKeys();
    }

    public void GetKeys()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            inputs.Add(code.Forward);
            gm.uh.ShowKeys(90);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            inputs.Add(code.Left);
            gm.uh.ShowKeys(180);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            inputs.Add(code.Right);
            gm.uh.ShowKeys(0);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            inputs.Add(code.Backward);
            gm.uh.ShowKeys(-90);
        }
    }
}