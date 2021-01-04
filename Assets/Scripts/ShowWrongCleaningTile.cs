using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ShowWrongCleaningTile : MonoBehaviour
{
    public static ShowWrongCleaningTile instance;
    public List<MapGenerator.Coord> wrongCleaningTiles = new List<MapGenerator.Coord>();
    public UiMiniMapController miniMapController;
    public MapGenerator mapGenerator;
    void Awake()
    {
        instance = this;
    }

    public void ShowWrongCleaningTiles()
    {
        miniMapController.MiniMapFullSize();

        for (int i = 0; i < wrongCleaningTiles.Count; i++)
        {
            var wrongTileIndex = mapGenerator.allTileCoords.FindIndex(v =>
                (v.x == wrongCleaningTiles[i].x) && (v.y == wrongCleaningTiles[i].y));
            mapGenerator.allTileGameObject[wrongTileIndex].gameObject.GetComponent<Renderer>().material.DOColor(new Color(217/255f,33/255f,57/255f), 2).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        }
    }
}
