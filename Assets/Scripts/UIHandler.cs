using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIHandler : MonoBehaviour
{
    private GameManager gm;
    private MapGenerator map;
    public GameObject minimap;
    private GameObject minimapTexture;
    private GameObject pipBoy;
    public GameObject gameOverPanel;

    public Camera miniMapCamera;
    public GameObject mainCamera;
    public Transform cameraTarget;
    private bool mapZoomed = false;
    private float mapSizeMultiplier = 0.15f;

    public GameObject codePanel;
    private bool codePanelOpened = false;
    private float codePaneleWidth = 0;

    private float screenW = Screen.width;
    private float screenH = Screen.height;

    private void Start()
    {
        codePaneleWidth = Mathf.Abs(codePanel.transform.position.x);
        gm = FindObjectOfType<GameManager>();
        map = FindObjectOfType<MapGenerator>();
        minimapTexture = minimap.transform.Find("MiniMapGraphics/Texture").gameObject;
        MiniMapSizeSet();
        miniMapCamera.transform.position = new Vector3( map.currentMap.mapSize.x-1, miniMapCamera.transform.position.y, map.currentMap.mapSize.y-1);
        miniMapCamera.orthographicSize = miniMapCamera.transform.position.x + 4;
    }

    public IEnumerator CameraSmothMovingToTargetPosition()
    {
        float t = 0;
        while (Vector3.Distance(mainCamera.transform.position, cameraTarget.position) > 0.1f)
        {
            t += Time.deltaTime / 30;
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraTarget.position, t);
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, cameraTarget.rotation, t);

            yield return new WaitForSeconds(0f);
        }
    }

    void MiniMapSizeSet()
    {
        pipBoy = minimap.transform.Find("MiniMapGraphics").gameObject;

        pipBoy.GetComponent<RectTransform>().sizeDelta = new Vector2(screenH, screenH);
        RatiosForMiniMap();
    }

    void RatiosForMiniMap()
    {
        var textureSize = (float)(pipBoy.GetComponent<RectTransform>().sizeDelta.y - pipBoy.GetComponent<RectTransform>().sizeDelta.y * 0.3);
        minimapTexture.GetComponent<RectTransform>().sizeDelta = new Vector2(textureSize, textureSize);
    }

    public void MiniMapZoom()
    {
        if (gm.is3DStarted)
            StartCoroutine(MiniMapSizeChange());
    }

    public IEnumerator MiniMapSetStartPosition()
    {
        minimap.GetComponent<RectTransform>().SetAnchor(AnchorPresets.TopRight);
        minimap.GetComponent<RectTransform>().SetPivot(PivotPresets.TopRight);
        minimap.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);

        float t = 0;
        while (true)
        {
            t += Time.deltaTime / 10;
            minimap.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(minimap.GetComponent<RectTransform>().sizeDelta, new Vector2(300, 300), t);
            minimap.transform.GetChild(0).localScale = Vector2.Lerp(minimap.transform.GetChild(0).localScale, new Vector3(1, 1, 1), t * 2);
            pipBoy.GetComponent<RectTransform>().sizeDelta = minimap.GetComponent<RectTransform>().sizeDelta - Vector2.one*30;
            RatiosForMiniMap();
            if (Mathf.Round(minimap.GetComponent<RectTransform>().sizeDelta.x) == 300)
                yield break;
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

    //CodePanel

    public void CodePanelOpen()
    {
        var clickedObject = EventSystem.current.currentSelectedGameObject;
        StartCoroutine(CodePanel(clickedObject));
    }

    public IEnumerator CodePanel(GameObject clickedObject)
    {
        for (int i = 0; i < 10; i++)
        {
            if (!codePanelOpened)
            {
                codePanel.transform.localPosition =
                    codePanel.transform.localPosition + new Vector3(codePaneleWidth / 10, 0, 0);
                clickedObject.transform.localPosition =
                    clickedObject.transform.localPosition + new Vector3(codePaneleWidth / 10, 0, 0);

                yield return new WaitForSeconds(0f);
            }
            else
            {
                codePanel.transform.localPosition =
                    codePanel.transform.localPosition - new Vector3(codePaneleWidth / 10, 0, 0);
                clickedObject.transform.localPosition =
                    clickedObject.transform.localPosition - new Vector3(codePaneleWidth / 10, 0, 0);
                yield return new WaitForSeconds(0f);
            }
        }

        clickedObject.transform.Rotate(new Vector3(0, 0, 180));
        codePanelOpened = !codePanelOpened;
        Debug.Log(codePanelOpened);
    }

    public void OpenGameOverPanel(bool isSuccess)
    {
        gameOverPanel.SetActive(true);
        var gameOverText = gameOverPanel.transform.Find("Text");

        if (isSuccess)
        {
            gameOverText.GetComponent<TextMeshProUGUI>().text = "Başarılı!";
        }
        else
        {
            gameOverText.GetComponent<TextMeshProUGUI>().text = "Başarısız!";
        }

    }
}
