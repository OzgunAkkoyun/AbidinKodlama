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

    public float animationSpeed = 1f;
    
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
        
    }

    IEnumerator ExecuteAnimation()
    {
        var scaleFactor = mapGenerate.tileSize;
        
        for (int i = 0; i < getInputs.inputs.Count; i++)
        {
            if (getInputs.inputs[i] == GetInputs.code.Left)
            {
                inputVector.x -= scaleFactor;
            }
            else if (getInputs.inputs[i] == GetInputs.code.Right)
            {
                inputVector.x += scaleFactor;
            }
            else if (getInputs.inputs[i] == GetInputs.code.Forward)
            {
                inputVector.z += scaleFactor;
            }
            else if (getInputs.inputs[i] == GetInputs.code.Backward)
            {
                inputVector.z -= scaleFactor;
            }

           
            //Code Inputs coloring
            uh.codeInputsObjects[i].GetComponent<Image>().color = new Color(163 / 255, 255 / 255, 131 / 255);
            if (i != 0)
                uh.codeInputsObjects[i - 1].GetComponent<Image>().color = Color.white;

            if (isAnimStarted) yield break; // exit function
            isAnimStarted = true;

            var relativePos = new Vector3(inputVector.x, transform.position.y, inputVector.z) - transform.position;
            var targetRotation = Quaternion.LookRotation(relativePos);

            for (float t = 0f; t < 1f; t += Time.deltaTime * animationSpeed)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation , t);
                yield return null;
            }

            if (i != getInputs.inputs.Count - 1)
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

            var currentCoord = new Coord((int)(transform.position.x / mapGenerate.tileSize), (int)(transform.position.z / mapGenerate.tileSize));

            if (mapGenerate.Path.Contains(currentCoord))
            {
                //Debug.Log("inPath");
            }
            else
            {
                isPlayerReachedTarget = false;
                break;
            }

            if (mapGenerate.CoordToPosition(mapGenerate.currentMap.targetPoint.x, mapGenerate.currentMap.targetPoint.y) == inputVector.Vector3toXZ())
            {
                isPlayerReachedTarget = true;
            }
        }

        if (isPlayerReachedTarget)
        {
            CharacterAnimationPlay();
        }
        else
        {
            EndGame();
        }

        //EndGame();
        //uh.OpenGameOverPanel(isPlayerReachedTarget);
        //gm.GameOverStatSet(isPlayerReachedTarget);
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
    }
    void CharacterAnimationPlay()
    {
        anim.SetBool("animationStart", true);
    }
}