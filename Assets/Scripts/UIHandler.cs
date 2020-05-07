using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIHandler : MonoBehaviour
{
    private GameManager gm;
    public GameObject minimap;
    private bool mapZoomed = false;
    private float mapSizeMultiplier = 0.15f;

    public GameObject codePanel;
    private bool codePanelOpened = false;
    private float codePaneleWidth = 0;

    private void Awake()
    {
        codePaneleWidth = Mathf.Abs(codePanel.transform.position.x);
        gm = FindObjectOfType<GameManager>();
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
            minimap.GetComponent<RectTransform>().sizeDelta =
                Vector2.Lerp(minimap.GetComponent<RectTransform>().sizeDelta, new Vector2(300, 300), t);
            minimap.transform.GetChild(0).localScale =
                Vector2.Lerp(minimap.transform.GetChild(0).localScale, new Vector3(1, 1, 1), t * 2);
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
}
