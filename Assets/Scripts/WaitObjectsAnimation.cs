using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

public class WaitObjectsAnimation : MonoBehaviour
{
    public static WaitObjectsAnimation instance;
    public PathGenarator pathGenarator;
    public Material currentTileMetarial;
    public GameObject sparkle;
    public GameObject wrongSparkle;
    public int howManyDirtCleaned = 0;
    private int dirtCount;
    public GetInputs getInputs;
    public UIHandler uiHandler;
    void Awake()
    {
        instance = this;
    }

    public IEnumerator CleanTile(Coord currentCoord,Coord realCoord, int seconds, bool currectSecond)
    {
        var currentCoordPos = pathGenarator.mapGenerator.CoordToPosition(currentCoord.x, currentCoord.y);
        GameObject currentSparkle;

        if (currectSecond)
            currentSparkle = Instantiate(sparkle, currentCoordPos, Quaternion.identity);
        else
            currentSparkle = Instantiate(wrongSparkle, currentCoordPos, Quaternion.identity);

        var currentTile =
            pathGenarator.mapGenerator.allTileGameObject.Find(v => v.transform.position == currentCoordPos);
        if (currectSecond)
        {
            currentTile.gameObject.GetComponent<Renderer>().material = currentTileMetarial;
            howManyDirtCleaned++;
        }

        var expectedSecond = realCoord.whichDirt.seconds;
        var waitSecond = (expectedSecond - seconds) >= 0 ? expectedSecond : seconds;
        yield return new WaitForSeconds(waitSecond + 0.5f);
        Destroy(currentSparkle);
    }

    public IEnumerator StartCleaningTheTile(int seconds, Vector3 inputVector, CharacterMovement characterMovement)
    {
        var currentCoord = new Coord((int)(inputVector.x / pathGenarator.mapGenerator.tileSize), (int)(inputVector.z / pathGenarator.mapGenerator.tileSize));

        var realCoord = pathGenarator.Path.Find(v => v.x == currentCoord.x && v.y == currentCoord.y);

        if (realCoord.whichDirt != null)
        {
            var expectedSecond = pathGenarator.currentDirts[dirtCount].seconds;
            if (expectedSecond == getInputs.seconds[dirtCount])
            {
                uiHandler.StartCleaningCountDown(realCoord,seconds);
                if(pathGenarator.gm.currentSenario.senarioIndex == 4)
                    yield return CleanTile(currentCoord, realCoord, seconds,true);
                else if(pathGenarator.gm.currentSenario.senarioIndex == 5)
                    yield return IrrigateTile(currentCoord, realCoord, seconds, true);
            }
            else
            {
                uiHandler.StartCleaningCountDown(realCoord, seconds);
                if (pathGenarator.gm.currentSenario.senarioIndex == 4)
                    yield return CleanTile(currentCoord, realCoord, seconds, false);
                else if (pathGenarator.gm.currentSenario.senarioIndex == 5)
                    yield return IrrigateTile(currentCoord, realCoord, seconds, false);
                ShowWrongCleaningTile.instance.wrongWaitTiles.Add(realCoord);
            }
            dirtCount++;
        }
        else
        {
            yield return new WaitForSeconds(1f);
            characterMovement.isPlayerReachedTarget = false;
            characterMovement.gm.EndGame();
        }
    }

    private IEnumerator IrrigateTile(Coord currentCoord, Coord realCoord, int seconds, bool currectSecond)
    {
        var currentCoordPos = pathGenarator.mapGenerator.CoordToPosition(currentCoord.x, currentCoord.y);
        GameObject currentSparkle;

        if (currectSecond)
            currentSparkle = Instantiate(sparkle, currentCoordPos, Quaternion.identity);
        else
            currentSparkle = Instantiate(wrongSparkle, currentCoordPos, Quaternion.identity);

        var currentTile =
            pathGenarator.mapGenerator.allTileGameObject.Find(v => v.transform.position == currentCoordPos);

        //if (currectSecond)
        //{
        //    currentTile.gameObject.GetComponent<Renderer>().material = currentTileMetarial;
        //    howManyDirtCleaned++;
        //}

        var expectedSecond = realCoord.whichDirt.seconds;
        var waitSecond = (expectedSecond - seconds) >= 0 ? expectedSecond : seconds;
        yield return new WaitForSeconds(waitSecond + 0.5f);
        Destroy(currentSparkle);
    }
}