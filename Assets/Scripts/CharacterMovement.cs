using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using static MapGenerator;

public class CharacterMovement : MonoBehaviour
{
    bool isAnimStarted = false;
    public bool isPlayerReachedTarget = false;
   
    public MapGenerator mapGenerate;
    public PathGenarator pathGenarator;
    public CameraMovementForSS cameraMovementForSs;
    UIHandler uh;
    [HideInInspector]
    public GameManager gm;
    [HideInInspector]
    public CheckTargetReached checkTargetReached;
    GetInputs getInputs;
    [HideInInspector]
    public WaitObjectsAnimation waitObjectsAnimation;
    public GameObjectsAnimationController animController;

    public float animationSpeed = 1f;
    public float scaleFactor = 1f;
    [HideInInspector]
    public Vector3 inputVector;
    private Animator anim;
    public Coord currentPath;
    public GameObject currentAnimal;
    public GameObject currentMushroom;

    void Start()
    {
        mapGenerate = FindObjectOfType<MapGenerator>();
        uh = FindObjectOfType<UIHandler>();
        gm = FindObjectOfType<GameManager>();
        pathGenarator = FindObjectOfType<PathGenarator>();
        getInputs = FindObjectOfType<GetInputs>();
        waitObjectsAnimation = FindObjectOfType<WaitObjectsAnimation>();
        cameraMovementForSs = FindObjectOfType<CameraMovementForSS>();
        checkTargetReached = FindObjectOfType<CheckTargetReached>();
        inputVector = transform.position;
        anim = GetComponent<Animator>();
        scaleFactor = mapGenerate.tileSize;
    }

    public IEnumerator ApplyMoveCommand(Direction moveCommand, bool isLastCommand, int i)
    {
        DirectionToVector(moveCommand);
        //Code Inputs coloring
        uh.MarkCurrentCommand(i);

        yield return StartCoroutine(PlayMoveAnimation(isLastCommand));

        checkTargetReached.CheckIfReachedTarget(isLastCommand, this);
    }

    public IEnumerator ApplyIfCommand(string ifObjectName, bool isLastCommand)
    {
        if (gm.currentSenario.senarioIndex == 3)
        {
            yield return IfObjectAnimations.instance.SenarioTreeIfCheck(isLastCommand, this, inputVector);
        }
        else if (gm.currentSenario.senarioIndex == 5)
        {
            yield return IfObjectAnimations.instance.SenarioFiveIfCheck(isLastCommand, this, inputVector);
        }
        //yield return IfObjectAnimations.instance.SenarioTreeIfCheck(isLastCommand, this);
    }

    public IEnumerator ApplyWaitCommand(int seconds, bool isLastCommand, int i)
    {
        //Code Inputs coloring
        uh.MarkCurrentCommand(i);

        yield return StartCoroutine(PlayMoveAnimation(isLastCommand));

        yield return StartCoroutine(WaitObjectsAnimation.instance.StartCleaningTheTile(seconds,inputVector,this));

        checkTargetReached.CheckIfReachedTarget(isLastCommand, this);
    }

    public object CompleteHalfWay()
    {
        transform.DOMove(inputVector, .5f);
        return new WaitForSeconds(.5f);
    }

    private IEnumerator PlayMoveAnimation(bool isLastCommand)
    {
        if (isAnimStarted) yield break; // exit function
        isAnimStarted = true;
        
        yield return Turn();

        if (!isLastCommand)
        {
            if (gm.currentSenario.senarioIndex == 1 || gm.currentSenario.senarioIndex == 2 || gm.currentSenario.senarioIndex == 4 )
            {
                yield return Move();
            }
            else if (gm.currentSenario.senarioIndex == 3)
            {
                yield return SenarioTreeMove();
            }
            else
            {
                yield return SenarioFiveMove();
            }

        }
        else
        {
            HalfWayMove();
            yield return null;
        }
        isAnimStarted = false;
    }

    private object SenarioTreeMove()
    {
        currentPath = pathGenarator.Path.Find(v =>
            (v.x * 2 == inputVector.Vector3toXZ().x) && (v.y * 2 == inputVector.Vector3toXZ().z));

        if (currentPath.whichCoord == AnimalsInIfPath.isAnimalCoord && !currentPath.isVisited)
        {
            currentPath.isVisited = true;
            currentAnimal = pathGenarator.animals.Find(v =>
                (v.transform.position.x == inputVector.Vector3toXZ().x) &&
                (v.transform.position.z == inputVector.Vector3toXZ().z));
            QuarterWayMove();
            var Direction = inputVector - transform.position;
            var halfVector = inputVector - (Direction - Direction / 4);
            IfObjectAnimations.instance.RemoveQuestionObjectInAnimal(currentAnimal, halfVector);
            return null;
        }
        else if (currentPath.whichCoord == AnimalsInIfPath.isEmptyAnimalCoord && !currentPath.isVisited)
        {
            currentPath.isVisited = true;
            var currentQuestionMark = pathGenarator.justEmtyQuestionMarks.Find(v =>
                (v.transform.position.x == inputVector.Vector3toXZ().x) && (v.transform.position.z == inputVector.Vector3toXZ().z));
            HalfWayMove();

            IfObjectAnimations.instance.RemoveOnlyQuestionMark(currentQuestionMark);
            return null;
        }
        else
        {
            return Move();
        }
    }
    private object SenarioFiveMove()
    {
        currentPath = pathGenarator.Path.Find(v =>
            (v.x * 2 == inputVector.Vector3toXZ().x) && (v.y * 2 == inputVector.Vector3toXZ().z));
        var Direction = inputVector - transform.position;
        var halfVector = inputVector - (Direction - Direction / 4);

        if (currentPath.whichCoord == AnimalsInIfPath.isAnimalCoord && !currentPath.isVisited)
        {
            currentPath.isVisited = true;
            currentAnimal = pathGenarator.animals.Find(v =>
                (v.transform.position.x == inputVector.Vector3toXZ().x) &&
                (v.transform.position.z == inputVector.Vector3toXZ().z));
            QuarterWayMove();
            IfObjectAnimations.instance.ShowIfObjectAnimation(currentAnimal, halfVector);
            return null;
        }
        else if (currentPath.whichCoord == AnimalsInIfPath.isEmptyAnimalCoord && !currentPath.isVisited)
        {
            currentPath.isVisited = true;
            var currentQuestionMark = pathGenarator.justEmtyQuestionMarks.Find(v =>
                (v.transform.position.x == inputVector.Vector3toXZ().x) &&
                (v.transform.position.z == inputVector.Vector3toXZ().z));

            HalfWayMove();
            IfObjectAnimations.instance.RemoveOnlyQuestionMark(currentQuestionMark);
            return null;
        }
        else
        {
            return Move();
        }
    }

    private IEnumerator Turn()
    {
        var relativePos = new Vector3(inputVector.x, transform.position.y, inputVector.z) - transform.position;
        var targetRotation = Quaternion.LookRotation(relativePos);

        for (float t = 0f; t < 1f; t += Time.deltaTime * animationSpeed)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);
            yield return null;
        }
    }

    private IEnumerator Move()
    {
        for (float t = 0f; t < 1f; t += Time.deltaTime * animationSpeed)
        {
            transform.position = Vector3.Lerp(transform.position, inputVector, t);
            yield return null;
        }

        transform.position = inputVector;
    }

    private void HalfWayMove()
    {
        var Direction = inputVector - transform.position;
        var a = inputVector - Direction / 2;
        transform.DOMove(a, .5f);
    }

    private void QuarterWayMove()
    {
        var Direction = inputVector - transform.position;
        var a = inputVector - (Direction - Direction / 4);
        transform.DOMove(a, .5f);
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

    void TargetObjectAnimationPlay()
    {
        animController = FindObjectOfType<GameObjectsAnimationController>();
        if(anim != null)
            anim.SetBool("animationStart", false);
        animController.GameObjectAnimationPlay();
    }

    public void CharacterAnimationPlay()
    {
        if (anim != null)
        {
            anim.SetBool("animationStart", true);
        }
        else
        {
            TargetObjectAnimationPlay();
        }
    }
}