using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitObjectsAnimation : MonoBehaviour
{
    public static WaitObjectsAnimation instance;
    public PathGenarator pathGenarator;
    public Material currentTileMetarial;
    public GameObject sparkle;
    public int howManyDirtCleaned = 0;
    void Awake()
    {
        instance = this;
    }

    public IEnumerator CleanTile(MapGenerator.Coord currentCoord,int seconds)
    {
        var currentCoordPos = pathGenarator.mapGenerator.CoordToPosition(currentCoord.x, currentCoord.y);
        var currentSparkle = Instantiate(sparkle,currentCoordPos,Quaternion.identity);

        var currentTile =
            pathGenarator.mapGenerator.allTileGameObject.Find(v => v.transform.position == currentCoordPos);
        currentTile.gameObject.GetComponent<Renderer>().material = currentTileMetarial;
        howManyDirtCleaned++;
        yield return new WaitForSeconds(seconds);
        Destroy(currentSparkle);
    }
}