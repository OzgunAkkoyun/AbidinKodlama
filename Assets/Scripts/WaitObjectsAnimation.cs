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

    public IEnumerator CleanTile(Coord currentCoord, int seconds, bool currectSecond)
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
        
        yield return new WaitForSeconds(seconds);
        Destroy(currentSparkle);
    }

    public IEnumerator StartCleaningTheTile(int seconds, Vector3 inputVector, CharacterMovement characterMovement)
    {
        var currentCoord = new Coord((int)(inputVector.x / pathGenarator.mapGenerator.tileSize), (int)(inputVector.z / pathGenarator.mapGenerator.tileSize));

        var realCoord = pathGenarator.Path.Find(v => v.x == currentCoord.x && v.y == currentCoord.y);

        if (realCoord.whichDirt != null)
        {
            if (pathGenarator.currentDirts[dirtCount].seconds == getInputs.seconds[dirtCount])
            {
                uiHandler.StartCleaningCountDown(realCoord,seconds);
                yield return CleanTile(currentCoord, seconds,true);
            }
            else
            {
                uiHandler.StartCleaningCountDown(realCoord, seconds);
                yield return CleanTile(currentCoord, seconds,false);
                ShowWrongCleaningTile.instance.wrongCleaningTiles.Add(realCoord);
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
}