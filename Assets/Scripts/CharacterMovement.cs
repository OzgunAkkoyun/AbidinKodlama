using DG.Tweening;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using static MapGenerator;

public class CharacterMovement : MonoBehaviour
{
    bool isAnimStarted = false;
    public bool isPlayerReachedTarget = false;
   
    private MapGenerator mapGenerate;
    private PathGenarator pathGenarator;
    public CameraMovementForSS cameraMovementForSs;
    UIHandler uh;
    GameManager gm;
    GetInputs getInputs;
    WaitObjectsAnimation waitObjectsAnimation;
    public GameObjectsAnimationController animController;

    public float animationSpeed = 1f;
    public float scaleFactor = 1f;
    
    Vector3 inputVector;
    private Animator anim;
    Coord currentPath;
    GameObject currentAnimal;
    void Start()
    {
        mapGenerate = FindObjectOfType<MapGenerator>();
        uh = FindObjectOfType<UIHandler>();
        gm = FindObjectOfType<GameManager>();
        pathGenarator = FindObjectOfType<PathGenarator>();
        getInputs = FindObjectOfType<GetInputs>();
        waitObjectsAnimation = FindObjectOfType<WaitObjectsAnimation>();
        cameraMovementForSs = FindObjectOfType<CameraMovementForSS>();
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

        CheckIfReachedTarget(isLastCommand);
    }

    public IEnumerator ApplyIfCommand(string ifObjectName, bool isLastCommand)
    {
        if (currentPath.whichCoord == AnimalsInIfPath.isAnimalCoord)
        {
            var currentAnimal = pathGenarator.animals.Find(v =>
                (v.transform.position.x == inputVector.Vector3toXZ().x) &&
                (v.transform.position.z == inputVector.Vector3toXZ().z));

            if (pathGenarator.selectedAnimals[0].ifName == pathGenarator.currentAnimal.ifName)
            {
                pathGenarator.selectedAnimals.RemoveAt(0);
                cameraMovementForSs.OpenSSLayout();
                yield return new WaitUntil(() => ScreenShotHandler.instance.isSSTaken);
                yield return new WaitForSeconds(1f);
                IfObjectAnimations.instance.AnimalMoveFromPath(currentAnimal);
                yield return new WaitForSeconds(1f);
                yield return CompleteHalfWay();
            }
            else
            {
                yield return new WaitForSeconds(1f);
                isPlayerReachedTarget = false;

                gm.EndGame();
            }
            
        }
        else if (currentPath.whichCoord == AnimalsInIfPath.isEmptyAnimalCoord)
        {
            yield return new WaitForSeconds(1f);
            yield return CompleteHalfWay();
        }
        else
        {
            yield return new WaitForSeconds(1f);
            isPlayerReachedTarget = false;

            gm.EndGame();
        }

        CheckIfReachedTarget(isLastCommand);

    }

    public IEnumerator ApplyWaitCommand(int seconds, bool isLastCommand, int i)
    {
        //Code Inputs coloring
        uh.MarkCurrentCommand(i);

        yield return StartCoroutine(PlayMoveAnimation(isLastCommand));

        yield return StartCoroutine(StartCleaningTheTile(seconds));

        CheckIfReachedTarget(isLastCommand);
    }

    private int dirtCount;
    private IEnumerator StartCleaningTheTile(int seconds)
    {
        var currentCoord = new Coord((int)(inputVector.x / mapGenerate.tileSize), (int)(inputVector.z / mapGenerate.tileSize));

        var realCoord = pathGenarator.Path.Find(v => v.x == currentCoord.x && v.y == currentCoord.y);

        if (realCoord.whichDirt != null)
        {
            if (pathGenarator.currentDirts[dirtCount].seconds == getInputs.seconds)
            {
                yield return WaitObjectsAnimation.instance.CleanTile(currentCoord, seconds);
            }
            else
            {
                yield return new WaitForSeconds(1f);
                isPlayerReachedTarget = false;

                gm.EndGame();
            }
            dirtCount++;
        }
        else
        {
            yield return new WaitForSeconds(1f);
            isPlayerReachedTarget = false;

            gm.EndGame();
        }

    }

    private object CompleteHalfWay()
    {
        transform.DOMove(inputVector, .5f);
        return new WaitForSeconds(.5f);
    }

    private void CheckIfReachedTarget(bool isLastCommand)
    {
        var currentCoord = new Coord((int)(inputVector.x / mapGenerate.tileSize), (int)(inputVector.z / mapGenerate.tileSize));

        if (pathGenarator.Path.Contains(currentCoord))
        {
            if (mapGenerate.CoordToPosition(mapGenerate.currentMap.targetPoint.x, mapGenerate.currentMap.targetPoint.y) == inputVector.Vector3toXZ())
            {
                if (gm.currentSenario.senarioIndex == 1 || gm.currentSenario.senarioIndex == 2)
                {
                    isPlayerReachedTarget = true;
                    CharacterAnimationPlay();
                }
                else if (gm.currentSenario.senarioIndex == 3)
                {
                    CheckIfObjectCount();
                }
                else if (gm.currentSenario.senarioIndex == 4)
                {
                    CheckWaitObjectsCount();
                }
            }
            else
            {
                if (isLastCommand)
                {
                    isPlayerReachedTarget = false;
                    gm.EndGame();
                }
            }
        }
        else
        {
            isPlayerReachedTarget = false;
            gm.EndGame();
        }
    }

    private void CheckWaitObjectsCount()
    {
        if (waitObjectsAnimation.howManyDirtCleaned == gm.currentSubLevel.dirtCount)
        {
            isPlayerReachedTarget = true;
            CharacterAnimationPlay();
        }
        else
        {
            isPlayerReachedTarget = false;
            gm.EndGame();
        }
    }

    private void CheckIfObjectCount()
    {
        if (ScreenShotHandler.instance.collectedAnimalPhoto == gm.currentSubLevel.ifObjectCount)
        {
            isPlayerReachedTarget = true;
            CharacterAnimationPlay();
        }
        else
        {
            isPlayerReachedTarget = false;
            gm.EndGame();
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

        if (!isLastCommand)
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
                IfObjectAnimations.instance.RemoveSmokeInAnimal(currentAnimal, halfVector);
                yield return null;
            }
            else if (currentPath.whichCoord == AnimalsInIfPath.isEmptyAnimalCoord && !currentPath.isVisited)
            {
                currentPath.isVisited = true;
                var currentSmoke = pathGenarator.justSmoke.Find(v =>
                    (v.transform.position.x == inputVector.Vector3toXZ().x) &&
                    (v.transform.position.z == inputVector.Vector3toXZ().z));
                HalfWayMove();
              
                IfObjectAnimations.instance.RemoveOnlySmoke(currentSmoke);
                yield return null;
            }
            else
            {
                for (float t = 0f; t < 1f; t += Time.deltaTime * animationSpeed)
                {
                    transform.position = Vector3.Lerp(transform.position, inputVector, t);
                    yield return null;
                }
                transform.position = inputVector;
            }
        }
        else
        {
            HalfWayMove();
            yield return null;
        }
        isAnimStarted = false;
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

    void CharacterAnimationPlay()
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