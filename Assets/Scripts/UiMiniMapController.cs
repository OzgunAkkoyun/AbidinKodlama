using System.Collections;
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

        var miniMapZPos = 0f;
        if (map.currentMap.mapSize.y == 5)
        {
            miniMapZPos = -5.8f;
        }else if (map.currentMap.mapSize.y == 7)
        {
            miniMapZPos = -3.8f;
        }
        else if (map.currentMap.mapSize.y == 9)
        {
            miniMapZPos = -1.6f;
        }

        miniMapCamera.transform.position = new Vector3(map.currentMap.mapSize.x - 1, miniMapCamera.transform.position.y,
            miniMapZPos);

        miniMapCamera.orthographicSize = miniMapCamera.transform.position.x + 4;
    }
    void MiniMapSizeSet()
    {
        miniMapGraphicsRect.sizeDelta = new Vector2(screenH-50, screenH-50);
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

        //minimapPipBoy.SetActive(false);
        float t = 0;

        while (true)
        {
            t += Time.deltaTime / 10;
            miniMapRect.sizeDelta =
                Vector2.Lerp(miniMapRect.sizeDelta, new Vector2(300, 300), t);

            miniMapGraphics.transform.localScale =
                Vector2.Lerp(miniMapGraphics.transform.localScale, new Vector3(1, 1, 1), t * 2);

            miniMapGraphicsRect.sizeDelta = new Vector2(
                (miniMapRect.sizeDelta.y - 20 - 1 * 30),
                (miniMapRect.sizeDelta.y - 20 - 1 * 30));

            minimapTextureRect.sizeDelta = Vector2.Lerp(minimapTextureRect.sizeDelta, new Vector2(260, 260), t);

            miniMapGraphicsRect.SetLeft(0);
            miniMapGraphicsRect.SetRight(0);
            miniMapGraphicsRect.SetTop(0);
            miniMapGraphicsRect.SetBottom(0);

            miniMapGraphicsRect.SetAnchor(AnchorPresets.StretchAll);

            miniMapGraphicsRect.SetPivot(PivotPresets.MiddleCenter);

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
