using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class IfObjectAnimations : MonoBehaviour
{
    public static IfObjectAnimations instance;
    public PathGenarator pathGenarator;
    private Vector3 smokeScaleVector;
    public int collectedIfObjects;

    public Material[] mushroomMetarials;
    void Awake()
    {
        instance = this;
        smokeScaleVector = new Vector3(5,5,5);
    }

    public void RemoveQuestionObjectInAnimal(GameObject currentObject, Vector3 halfVector)
    {
        var questionMark = currentObject.transform.Find("EmtyQuestionMark");

        var character = pathGenarator.gm.character;
        var characterCam = Camera.main;

        var transformPosition = questionMark.transform.position;
        var targetPos = new Vector3(transformPosition.x, transformPosition.y + 15, transformPosition.z);

        if (pathGenarator.selectedAnimals[0].ifName == pathGenarator.currentIfObject.ifName)
        {
            character.transform.DOMoveY(0.3f, .9f).OnComplete(() =>
            {
                questionMark.transform.DOMove(targetPos, 2).OnComplete(() =>
                {
                    questionMark.gameObject.SetActive(false);
                    var lookPos = new Vector3(halfVector.x, currentObject.transform.position.y, halfVector.z);
                    currentObject.transform.DOLookAt(lookPos, 1f);
                });
            });

            var cameraMovePosDirection = currentObject.transform.position - character.transform.position;

            var cameraMovePosTemp = characterCam.transform.position + cameraMovePosDirection * 1.3f;

            var cameraMovePos = new Vector3(cameraMovePosTemp.x, 3, cameraMovePosTemp.z);

            characterCam.transform.DOMove(cameraMovePos, 1);
        }
        else
        {
            character.transform.DOMoveY(0.3f, .9f).OnComplete(() =>
            {
                questionMark.transform.DOMove(targetPos, 2).OnComplete(() =>
                {
                    questionMark.gameObject.SetActive(false);
                    var lookPos = new Vector3(halfVector.x, currentObject.transform.position.y, halfVector.z);
                    currentObject.transform.DOLookAt(lookPos, 1f);
                });
            });
        }

    }

    public void RemoveOnlyQuestionMark(GameObject currentQuestionMark)
    {
        var transformPosition = currentQuestionMark.transform.position;
        var targetPos = new Vector3(transformPosition.x, transformPosition.y + 15, transformPosition.z);
        currentQuestionMark.transform.DOMove(targetPos, 2).OnComplete(() =>
        {
            currentQuestionMark.SetActive(false);
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

    public void WrongAnimalMoveFromPath(GameObject currentAnimal)
    {
        var transformPosition = currentAnimal.transform.position;
        var targetPos = new Vector3(transformPosition.x, transformPosition.y + 15, transformPosition.z);
        currentAnimal.transform.DOMove(targetPos, 2).OnComplete(() =>
        {
            currentAnimal.SetActive(false);
        });
    }

    public IEnumerator SenarioTreeIfCheck(bool isLastCommand, CharacterMovement characterMovement, Vector3 inputVector)
    {
        if (characterMovement.currentPath.whichCoord == AnimalsInIfPath.isAnimalCoord)
        {
            var currentAnimal = pathGenarator.animals.Find(v =>
                (v.transform.position.x == characterMovement.inputVector.Vector3toXZ().x) &&
                (v.transform.position.z == characterMovement.inputVector.Vector3toXZ().z));

            if (pathGenarator.selectedAnimals[0].ifName == pathGenarator.currentIfObject.ifName)
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
                var currentCoord = new MapGenerator.Coord((int)(inputVector.x / pathGenarator.mapGenerator.tileSize), (int)(inputVector.z / pathGenarator.mapGenerator.tileSize));

                var realCoord = pathGenarator.Path.Find(v => v.x == currentCoord.x && v.y == currentCoord.y);

                ShowWrongCleaningTile.instance.wrongIfTiles.Add(realCoord);
                yield return new WaitForSeconds(3f);
                AnimalMoveFromPath(currentAnimal);
                //yield return new WaitForSeconds(1f);
                //characterMovement.isPlayerReachedTarget = false;

                //characterMovement.gm.EndGame();
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

    public IEnumerator SenarioFiveIfCheck(bool isLastCommand, CharacterMovement characterMovement, Vector3 inputVector)
    {
        if (characterMovement.currentPath.whichCoord == AnimalsInIfPath.isAnimalCoord)
        {
            var currentAnimal = pathGenarator.animals.Find(v =>
                (v.transform.position.x == characterMovement.inputVector.Vector3toXZ().x) &&
                (v.transform.position.z == characterMovement.inputVector.Vector3toXZ().z));

            if (pathGenarator.selectedAnimals[0].ifName == pathGenarator.currentIfObject.ifName)
            {
                pathGenarator.selectedAnimals.RemoveAt(0);
                collectedIfObjects++;
                yield return new WaitUntil(() => characterMovement.currentAnimal.activeSelf == false);
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
    public void ShowIfObjectAnimation(GameObject currentObject, Vector3 halfVector)
    {
        var questionMark = currentObject.transform.Find("EmtyQuestionMark");

        var character = pathGenarator.gm.character;
        var characterCam = Camera.main;

        var transformPosition = questionMark.transform.position;
        var targetPos = new Vector3(transformPosition.x, transformPosition.y + 15, transformPosition.z);

        questionMark.transform.DOMove(targetPos, 2).OnComplete(() =>
        {
            questionMark.gameObject.SetActive(false);

            var transformPosition1 = currentObject.transform.position;
            var targetPos1 = new Vector3(transformPosition1.x, transformPosition1.y + 1, transformPosition1.z);

            var sparkle = currentObject.transform.Find("GFX/Fx_PlantSparkle/Particle System");
            sparkle.GetComponent<ParticleSystem>().Play();
            currentObject.transform.DOMove(targetPos1, 4).OnComplete(() =>
            {
                currentObject.SetActive(false);
            });
        });
    }
}
