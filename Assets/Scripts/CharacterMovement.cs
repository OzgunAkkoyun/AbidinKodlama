using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static MapGenerator;

public class CharacterMovement : MonoBehaviour
{
    bool isAnimStarted = false;
    bool isPlayerReachedTarget = false;
    GetInputs getInputs;
    private MapGenerator mapGenerate;
    UIHandler uh;
    GameManager gm;

    [NonSerialized]
    public bool isGameFinished;

    public float animationSpeed = 1f;
    public float scaleFactor = 1f;
    
    Vector3 inputVector;
    private Animator anim;
    private GameObject WindTurbine;

    void Start()
    {
        getInputs = FindObjectOfType<GetInputs>();
        mapGenerate = FindObjectOfType<MapGenerator>();
        uh = FindObjectOfType<UIHandler>();
        gm = FindObjectOfType<GameManager>();
        inputVector = transform.position;
        anim = GetComponent<Animator>();
        scaleFactor = mapGenerate.tileSize;
    }

    public IEnumerator ApplyMoveCommand(Direction moveCommand, bool isLastCommand)
    {
        DirectionToVector(moveCommand);

        //Code Inputs coloring
        //uh.codeInputsObjects[i].GetComponent<Image>().color = new Color(163 / 255, 255 / 255, 131 / 255);
        //if (i != 0)
        //    uh.codeInputsObjects[i - 1].GetComponent<Image>().color = Color.white;

        yield return StartCoroutine(PlayMoveAnimation(isLastCommand));

        CheckIfReachedTarget();
    }
    public IEnumerator ApplyForCommand(Direction command, bool isLastCommand)
    {
        DirectionToVector(command);
        yield return StartCoroutine(PlayMoveAnimation(isLastCommand));

        CheckIfReachedTarget();
    }

    private void CheckIfReachedTarget()
    {
        var currentCoord = new Coord((int)(transform.position.x / mapGenerate.tileSize), (int)(transform.position.z / mapGenerate.tileSize));

        if (mapGenerate.Path.Contains(currentCoord))
        {
            if (mapGenerate.CoordToPosition(mapGenerate.currentMap.targetPoint.x, mapGenerate.currentMap.targetPoint.y) == inputVector.Vector3toXZ())
            {
                isPlayerReachedTarget = true;
                CharacterAnimationPlay();
            }
        }
        else
        {
            isPlayerReachedTarget = false;
            EndGame();
        }
    }

    private IEnumerator PlayMoveAnimation(bool isLastCommand)
    {
        if (isAnimStarted) yield break; // exit function
        isAnimStarted = true;

        var relativePos = new Vector3(inputVector.x, transform.position.y, inputVector.z) - transform.position;
        var targetRotation = Quaternion.LookRotation(relativePos);

        for (float t = 0f; t < 1f; t += Time.deltaTime * animationSpeed)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);
            yield return null;
        }

        //if (i != gm.commander.commands.Count - 1)
        if (!isLastCommand)
        {
            for (float t = 0f; t < 1f; t += Time.deltaTime * animationSpeed)
            {
                transform.position = Vector3.Lerp(transform.position, inputVector, t);
                yield return null;
            }
            transform.position = inputVector;
        }
        else
        {
            yield return null;
        }
        isAnimStarted = false;
    }

    private void DirectionToVector(Direction moveCommand)
    {
        if (moveCommand == Direction.Left)
        {
            inputVector.x -= scaleFactor;
        }
        else if (moveCommand == Direction.Right)
        {
            inputVector.x += scaleFactor;
        }
        else if (moveCommand == Direction.Forward)
        {
            inputVector.z += scaleFactor;
        }
        else if (moveCommand == Direction.Backward)
        {
            inputVector.z -= scaleFactor;
        }
    }

    void WindTurbineAnimationPlay()
    {
        anim.SetBool("animationStart", false);
        gm.sc.Play("Sparkle");
        WindTurbine = GameObject.Find("Target");

        WindTurbine.transform.Find("Fx_Smoke").gameObject.SetActive(false);
        WindTurbine.transform.Find("Fx_Sparkle").gameObject.SetActive(true);

        var sparkleDuration = WindTurbine.transform.Find("Fx_Sparkle").GetChild(0).GetComponent<ParticleSystem>().main.duration;
        WindTurbine.GetComponent<Animator>().SetBool("playAnim",true);
        Invoke("EndGame",sparkleDuration);
    }

    public void EndGame()
    {
        uh.OpenGameOverPanel(isPlayerReachedTarget);
        gm.GameOverStatSet(isPlayerReachedTarget);
        isGameFinished = true;
    }
    void CharacterAnimationPlay()
    {
        anim.SetBool("animationStart", true);
    }

    
}