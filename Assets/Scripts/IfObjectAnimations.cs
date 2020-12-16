using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class IfObjectAnimations : MonoBehaviour
{
    public static IfObjectAnimations instance;
    public PathGenarator pathGenarator;
    private Vector3 smokeScaleVector;
    void Awake()
    {
        instance = this;
        smokeScaleVector = new Vector3(5,5,5);
    }

    public void RemoveSmokeInAnimal(GameObject currentObject, Vector3 halfVector)
    {
        var smoke = currentObject.transform.Find("SmokeEffectIfObjects");

        var character = pathGenarator.gm.character;
        var characterCam = Camera.main;
        character.transform.DOMoveY(0.3f, .9f).OnComplete(() =>
        {
            smoke.DOScale(smokeScaleVector, .9f).OnComplete(() =>
            {
                smoke.gameObject.SetActive(false);
                var lookPos = new Vector3(halfVector.x, currentObject.transform.position.y, halfVector.z);
                currentObject.transform.DOLookAt(lookPos, 1f);
            });
        });
        var cameraMovePosDirection = currentObject.transform.position - character.transform.position;
        var cameraMovePosTemp = characterCam.transform.position + cameraMovePosDirection*1.3f;
        var cameraMovePos = new Vector3(cameraMovePosTemp.x,3, cameraMovePosTemp.z);
        Debug.Log(cameraMovePos);
        characterCam.transform.DOMove(cameraMovePos, 1);
    }

    public void RemoveOnlySmoke(GameObject currentSmoke)
    {
        currentSmoke.transform.DOScale(smokeScaleVector, .9f).OnComplete(() =>
        {
            currentSmoke.SetActive(false);
        });
    }

    public void AnimalMoveFromPath(GameObject currentAnimal)
    {
        var transformPosition = currentAnimal.transform.position;
        var targetPos = new Vector3(transformPosition.x,transformPosition.y+15,transformPosition.z);
        currentAnimal.transform.DOMove(targetPos, 2).OnComplete(() =>
        {
            currentAnimal.SetActive(false);
        });
    }
}
