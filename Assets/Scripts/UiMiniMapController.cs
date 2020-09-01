﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiMiniMapController : MonoBehaviour
{
    public MapGenerator map;
    public GameManager gm;

    public GameObject minimap;
    private GameObject minimapTexture;
    private GameObject minimapPipBoy;
    private GameObject miniMapGraphics;

    public Camera miniMapCamera;

    private bool mapZoomed = false;
    private float mapSizeMultiplier = 0.15f;

    private RectTransform miniMapRect;
    private RectTransform miniMapGraphicsRect;
    private RectTransform minimapTextureRect;

    private float screenW;
    private float screenH;
    void Start()
    {
        screenW = Screen.width;
        screenH = Screen.height;
        

        minimapTexture = minimap.transform.Find("MiniMapGraphics/Texture").gameObject;

        minimapPipBoy = minimap.transform.Find("MiniMapGraphics/PipBoy").gameObject;
        miniMapGraphics = minimap.transform.Find("MiniMapGraphics").gameObject;

        miniMapRect = minimap.GetComponent<RectTransform>();
        miniMapGraphicsRect = miniMapGraphics.GetComponent<RectTransform>();
        minimapTextureRect = minimapTexture.GetComponent<RectTransform>();
        MiniMapSizeSet();

        miniMapCamera.transform.position = new Vector3(map.currentMap.mapSize.x - 1, miniMapCamera.transform.position.y,
            map.currentMap.mapSize.y - 1);
        miniMapCamera.orthographicSize = miniMapCamera.transform.position.x + 4;
    }
    void MiniMapSizeSet()
    {
        miniMapGraphicsRect.sizeDelta = new Vector2(screenW - 100, screenH);
        RatiosForMiniMap();
    }

    void RatiosForMiniMap()
    {
        var textureSize = (float)(miniMapGraphicsRect.sizeDelta.y - miniMapGraphicsRect.sizeDelta.y * 0.2);
        minimapTextureRect.sizeDelta = new Vector2(textureSize, textureSize);
    }

    public void MiniMapZoom()
    {
        if (gm.is3DStarted)
            StartCoroutine(MiniMapSizeChange());
    }

    public IEnumerator MiniMapSetStartPosition()
    {
        miniMapRect.SetAnchor(AnchorPresets.TopRight);
        miniMapRect.SetPivot(PivotPresets.TopRight);
        miniMapRect.sizeDelta = new Vector2(screenW, screenH);
        minimapPipBoy.SetActive(false);

        float t = 0;
        while (true)
        {
            t += Time.deltaTime / 10;
            miniMapRect.sizeDelta =
                Vector2.Lerp(miniMapRect.sizeDelta, new Vector2(300, 300), t);

            minimap.transform.GetChild(0).localScale =
                Vector2.Lerp(minimap.transform.GetChild(0).localScale, new Vector3(1, 1, 1), t * 2);

            miniMapGraphicsRect.sizeDelta = new Vector2(
                (miniMapRect.sizeDelta.y - 20 - 1 * 30),
                (miniMapRect.sizeDelta.y - 20 - 1 * 30));

            miniMapGraphicsRect.SetLeft(0);
            miniMapGraphicsRect.SetRight(0);
            miniMapGraphicsRect.SetTop(0);
            miniMapGraphicsRect.SetBottom(0);

            miniMapGraphicsRect.SetAnchor(AnchorPresets.StretchAll);

            miniMapGraphicsRect.SetPivot(PivotPresets.MiddleCenter);

            minimapTextureRect.SetAnchor(AnchorPresets.StretchAll);

            minimapTextureRect.SetPivot(PivotPresets.MiddleCenter);

            minimapTextureRect.SetLeft(-15);
            minimapTextureRect.SetRight(-15);
            minimapTextureRect.SetTop(-15);
            minimapTextureRect.SetBottom(-15);

            RatiosForMiniMap();

            if (Mathf.Round(miniMapRect.sizeDelta.x) == 300)
            {

                yield break;
            }

            yield return new WaitForSeconds(0f);
        }
    }

    public IEnumerator MiniMapSizeChange()
    {
        for (int i = 0; i < 10; i++)
        {
            if (!mapZoomed)
            {
                minimap.transform.localScale = minimap.transform.localScale +
                                               new Vector3(mapSizeMultiplier, mapSizeMultiplier, mapSizeMultiplier);
                yield return new WaitForSeconds(0f);
            }
            else
            {
                minimap.transform.localScale = minimap.transform.localScale -
                                               new Vector3(mapSizeMultiplier, mapSizeMultiplier, mapSizeMultiplier);
                yield return new WaitForSeconds(0f);
            }
        }

        mapZoomed = !mapZoomed;
    }
}
