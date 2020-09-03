using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static MapGenerator;

public class CharacterMovement : MonoBehaviour
{
    bool isAnimStarted = false;
    public bool isPlayerReachedTarget = false;
   
    private MapGenerator mapGenerate;
    private PathGenarator pathGenarator;
    UIHandler uh;
    GameManager gm;
    public GameObjectsAnimationController animController;

    public float animationSpeed = 1f;
    public float scaleFactor = 1f;
    
    Vector3 inputVector;
    private Animator anim;

    void Start()
    {
        mapGenerate = FindObjectOfType<MapGenerator>();
        uh = FindObjectOfType<UIHandler>();
        gm = FindObjectOfType<GameManager>();
        pathGenarator = FindObjectOfType<PathGenarator>();
        inputVector = transform.position;
        anim = GetComponent<Animator>();
        scaleFactor = mapGenerate.tileSize;
    }

    public IEnumerator ApplyMoveCommand(Direction moveCommand, bool isLastCommand, int i)
    {
        DirectionToVector(moveCommand);
        //Code Inputs coloring
        uh.codeInputsObjects[i].GetComponent<Image>().color = new Color(163 / 255, 255 / 255, 131 / 255);
        if (i != 0)
            uh.codeInputsObjects[i - 1].GetComponent<Image>().color = Color.white;

        yield return StartCoroutine(PlayMoveAnimation(isLastCommand));

        CheckIfReachedTarget(isLastCommand);
    }
    public IEnumerator ApplyForCommand(Direction command, bool isLastCommand,int i)
    {
        DirectionToVector(command);
        yield return StartCoroutine(PlayMoveAnimation(isLastCommand));

        CheckIfReachedTarget(isLastCommand);
    }

    private void CheckIfReachedTarget(bool isLastCommand)
    {
        var currentCoord = new Coord((int)(inputVector.x / mapGenerate.tileSize), (int)(inputVector.z / mapGenerate.tileSize));

        if (pathGenarator.Path.Contains(currentCoord))
        {
            if (mapGenerate.CoordToPosition(mapGenerate.currentMap.targetPoint.x, mapGenerate.currentMap.targetPoint.y) == inputVector.Vector3toXZ())
            {
                isPlayerReachedTarget = true;
                
                CharacterAnimationPlay();
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
            //for (float t = 0f; t < 1f; t += Time.deltaTime * animationSpeed)
            //{
            //    transform.Translate(transform.forward/2); 
            //}
            //transform.position = inputVector;
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
        animController = FindObjectOfType<GameObjectsAnimationController>();
        anim.SetBool("animationStart", false);

        animController.WindTurbineAnimationPlay();
    }
  
    void CharacterAnimationPlay()
    {
        anim.SetBool("animationStart", true);
    }

    
}