using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class IfObjectAnimations : MonoBehaviour
{
    public static IfObjectAnimations instance;
    public PathGenarator pathGenarator;
    private Vector3 smokeScaleVector;

    public Material[] mushroomMetarials;
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

    public IEnumerator SenarioTreeIfCheck(bool isLastCommand, CharacterMovement characterMovement)
    {
        if (characterMovement.currentPath.whichCoord == AnimalsInIfPath.isAnimalCoord)
        {
            var currentAnimal = pathGenarator.animals.Find(v =>
                (v.transform.position.x == characterMovement.inputVector.Vector3toXZ().x) &&
                (v.transform.position.z == characterMovement.inputVector.Vector3toXZ().z));

            if (pathGenarator.selectedAnimals[0].ifName == pathGenarator.currentAnimal.ifName)
            {
                pathGenarator.selectedAnimals.RemoveAt(0);
                characterMovement.cameraMovementForSs.OpenSSLayout();
                yield return new WaitUntil(() => ScreenShotHandler.instance.isSSTaken);
                yield return new WaitForSeconds(1f);
                AnimalMoveFromPath(currentAnimal);
                yield return new WaitForSeconds(1f);
                yield return characterMovement.CompleteHalfWay();
            }
            else
            {
                yield return new WaitForSeconds(1f);
                characterMovement.isPlayerReachedTarget = false;

                characterMovement.gm.EndGame();
            }
        }
        else if (characterMovement.currentPath.whichCoord == AnimalsInIfPath.isEmptyAnimalCoord)
        {
            yield return new WaitForSeconds(1f);
            yield return characterMovement.CompleteHalfWay();
        }
        else
        {
            yield return new WaitForSeconds(1f);
            characterMovement.isPlayerReachedTarget = false;

            characterMovement.gm.EndGame();
        }
        characterMovement.checkTargetReached.CheckIfReachedTarget(isLastCommand, characterMovement);
    }

    public IEnumerator SenarioFiveIfCheck(bool isLastCommand, CharacterMovement characterMovement)
    {
        if (characterMovement.currentPath.whichCoord == AnimalsInIfPath.isAnimalCoord)
        {
            var currentAnimal = pathGenarator.animals.Find(v =>
                (v.transform.position.x == characterMovement.inputVector.Vector3toXZ().x) &&
                (v.transform.position.z == characterMovement.inputVector.Vector3toXZ().z));

            if (pathGenarator.selectedMushrooms[0] == AnimalsInIfPath.isAnimalCoord)
            {
                Debug.Log("first if if");
                pathGenarator.selectedMushrooms.RemoveAt(0);
                //characterMovement.cameraMovementForSs.OpenSSLayout();
                //yield return new WaitUntil(() => ScreenShotHandler.instance.isSSTaken);
                yield return new WaitForSeconds(1f);
                //AnimalMoveFromPath(currentAnimal);
                yield return new WaitForSeconds(1f);
                yield return characterMovement.CompleteHalfWay();
            }
            else
            {
                Debug.Log("first else");
                yield return new WaitForSeconds(1f);
                characterMovement.isPlayerReachedTarget = false;

                characterMovement.gm.EndGame();
            }
        }
        else if (characterMovement.currentPath.whichCoord == AnimalsInIfPath.isEmptyAnimalCoord)
        {
            Debug.Log("second if");
            pathGenarator.selectedMushrooms.RemoveAt(0);
            yield return new WaitForSeconds(1f);
            yield return characterMovement.CompleteHalfWay();
        }
        else
        {
            Debug.Log("third if");
            yield return new WaitForSeconds(1f);
            characterMovement.isPlayerReachedTarget = false;

            characterMovement.gm.EndGame();
        }
        characterMovement.checkTargetReached.CheckIfReachedTarget(isLastCommand, characterMovement);
    }

    public void ChangeMetarialInMushroom(GameObject currentMushroom, Vector3 halfVector, bool b)
    {
        if (b)
        {
            currentMushroom.transform.Find("GFX/amanita_a").GetComponent<MeshRenderer>().material = mushroomMetarials[0];
        }
        else
        {
            currentMushroom.transform.Find("GFX/amanita_a").GetComponent<MeshRenderer>().material = mushroomMetarials[1];
        }
        
    }
}
