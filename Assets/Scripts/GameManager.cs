using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    CharacterMovement character;
    private UIHandler uh;
    public bool is3DStarted = false;

    void Start()
    {
        character = FindObjectOfType<CharacterMovement>();
        uh = FindObjectOfType<UIHandler>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            is3DStarted = true;
            //uh.StartCoroutine("MiniMapSetStartPosition");
            character.StartCoroutine("ExecuteAnimation");
        }
    }
}
