using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static MapGenerator;

public class CharacterMovement : MonoBehaviour
{
    bool isAnimStarted = false;
    GetInputs getInputs;
    private MapGenerator mapGenerate;
    public ParticleSystem winEffect;
    UIHandler uh;
    GameManager gm;
    
    Vector3 inputVector;

    void Start()
    {
        getInputs = FindObjectOfType<GetInputs>();
        mapGenerate = FindObjectOfType<MapGenerator>();
        uh = FindObjectOfType<UIHandler>();
        gm = FindObjectOfType<GameManager>();
        inputVector = transform.position;
    }

    IEnumerator ExecuteAnimation()
    {
        var scaleFactor = mapGenerate.tileSize;
        var isPlayerReachedTarget = false;
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

            for (float t = 0f; t < 1f; t += Time.deltaTime * 1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation , t);
                yield return null;
            }
            for (float t = 0f; t < 1f; t += Time.deltaTime * 1f)
            {
                transform.position = Vector3.Lerp(transform.position, inputVector, t);
                //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);
                yield return null;
            }

            transform.position = inputVector;
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

            if (mapGenerate.currentMap.targetPoint.x * scaleFactor == transform.position.x && mapGenerate.currentMap.targetPoint.y * scaleFactor == transform.position.z)
            {
                isPlayerReachedTarget = true;
                //winEffect.Play();
            }
        }

        uh.OpenGameOverPanel(isPlayerReachedTarget);
        gm.GameOverStatSet(isPlayerReachedTarget);

    }
}
