﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeMonkey.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class WindowGraph : MonoBehaviour
{
    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;
    private RectTransform pieContainer;
    private RectTransform barLineGraphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;
    public List<GameObject> gameObjectList;
    public List<float> valueList = new List<float>();

    void Awake()
    {
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        barLineGraphContainer = graphContainer.Find("BarAndLineGraphContainer").GetComponent<RectTransform>();
        labelTemplateX = barLineGraphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = barLineGraphContainer.Find("labelTemplateY").GetComponent<RectTransform>();
        dashTemplateX = barLineGraphContainer.Find("dashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = barLineGraphContainer.Find("dashTemplateY").GetComponent<RectTransform>();
        pieContainer = graphContainer.Find("PieChartArea").GetComponent<RectTransform>();

        gameObjectList = new List<GameObject>();
        //List<float> valueList = new List<float>() {5, 98, 56, 45, 30, 22, 17, 15, 13, 17, 25, 37, 40, 36, 33};
        float[] a = new [] {50f, 65.7f, 89f, 12.55f, 44f};
        ShowPieGraph(a);
        //IGraphVisual barGraphVisual = new BarChartVisual(graphContainer, Color.cyan, .8f);


        //FunctionPeriodic.Create(() =>
        //{
        //    valueList.Clear();
        //    for (int i = 0; i < 15; i++)
        //    {
        //        valueList.Add(Random.Range(0,500));
        //    }
        //    ShowGraph(valueList, (int _i) => (_i + 1).ToString(), (float _f) => "$" + Mathf.RoundToInt(_f));
        //}, 5f);

    }

    public void StartShowGraph(List<float> graphValues)
    {
        valueList = graphValues;
        //List<float> valueList = new List<float>() { 50.87f };
        //IGraphVisual graphVisual = new LineGraphVisual(barLineGraphContainer, circleSprite, Color.cyan, new Color(1, 1, 1, .5f));
        IGraphVisual graphVisual = new BarChartVisual(barLineGraphContainer, Color.cyan, .8f);
        ShowGraph(valueList, graphVisual, -1, (int _i) => (_i + 1).ToString(), (float _f) => ""+ Mathf.Round(_f * 100f) / 100f);
    }

    private void ShowGraph(List<float> valueList,IGraphVisual graphVisual, int maxVisibleValueAmount =-1, Func<int, string> getAxisLabelX = null, Func<float, string> getAxisLabelY = null)
    {
        if (getAxisLabelX == null)
        {
            getAxisLabelX = delegate(int _i) { return _i.ToString(); };
        }

        if (getAxisLabelY == null)
        {
            getAxisLabelY = delegate(float _f) { return Mathf.RoundToInt(_f).ToString(); };
        }

        if (maxVisibleValueAmount <= 0)
        {
            maxVisibleValueAmount = valueList.Count;
        }

        foreach (var gameObject in gameObjectList)
        {
            Destroy(gameObject);
        }

        gameObjectList.Clear();

        if (valueList.Count == 0)
        {
            return;
        }

        float graphWidth = barLineGraphContainer.sizeDelta.x;
        float graphHeight = barLineGraphContainer.sizeDelta.y;

        float yMaximum = valueList[0];
        float yMinimum = valueList[0];

        for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++)
        {
            float value = valueList[i];
            if (value > yMaximum)
            {
                yMaximum = value;
            }
            if (value < yMinimum)
            {
                yMinimum = value;
            }
        }

        float yDiffrence = yMaximum - yMinimum;

        if (yDiffrence <= 0)
        {
            yDiffrence = 5;
        }

        yMaximum = 100;//yMaximum + (yDiffrence * 0.2f);
        yMinimum = 0;//yMinimum - (yDiffrence * 0.2f);

        float xSize = graphWidth / (maxVisibleValueAmount + 1);

        int xIndex = 0;

        GameObject lastCircleGameObject = null;

        for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++)
        {
            float xPosition = xSize + xIndex * xSize;
            float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;

            gameObjectList.AddRange(graphVisual.AddGraphVisual(new Vector2(xPosition, yPosition), xSize));


            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(barLineGraphContainer);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPosition - 8f, -7f);
            labelX.localScale = Vector3.one;
            labelX.GetComponent<TextMeshProUGUI>().text = getAxisLabelX(i);
            gameObjectList.Add(labelX.gameObject);

            RectTransform dashX = Instantiate(dashTemplateX);
            dashX.SetParent(barLineGraphContainer);
            dashX.gameObject.SetActive(true);
            dashX.localScale = Vector3.one;
            dashX.anchoredPosition = new Vector2(xPosition, -3f);
            gameObjectList.Add(dashX.gameObject);
            xIndex++;
        }

        int separatorCount = 10;

        for (int i = 0; i <= separatorCount; i++)
        {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(barLineGraphContainer);
            labelY.gameObject.SetActive(true);

            float normalizedValue = i * 1f / separatorCount;
            labelY.anchoredPosition = new Vector2(-55f, normalizedValue * graphHeight - 8f);
            labelY.localScale = Vector3.one;
            labelY.GetComponent<TextMeshProUGUI>().text = getAxisLabelY(yMinimum + (normalizedValue * (yMaximum - yMinimum)));
            gameObjectList.Add(labelY.gameObject);

            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(barLineGraphContainer);
            dashY.gameObject.SetActive(true);
            dashY.sizeDelta = new Vector2(graphWidth, dashY.sizeDelta.y);
            dashY.localScale = Vector3.one;
            dashY.anchoredPosition = new Vector2(-4f, normalizedValue * graphHeight);
            gameObjectList.Add(dashY.gameObject);

        }
    }

    public void ShowPieGraph(float[] fillAmount)
    {
        PieChartVisual a = new PieChartVisual(pieContainer,Color.black, Color.white);
        a.SetPiePercent(fillAmount);
    }
    private interface IGraphVisual
    {
        List<GameObject> AddGraphVisual(Vector2 graphPosition, float graphPositionWith);
    }

    private class BarChartVisual: IGraphVisual
    {
        private RectTransform graphContainer;
        private Color barColor;
        private float barWidthMultiplier;
        public BarChartVisual(RectTransform graphContainer, Color barColor, float barWidthMultiplier)
        {
            this.graphContainer = graphContainer;
            this.barColor = barColor;
            this.barWidthMultiplier = barWidthMultiplier;
        }

        public List<GameObject> AddGraphVisual(Vector2 graphPosition, float graphPositionWith)
        {
            GameObject barGameObject = CreateBar(graphPosition, graphPositionWith);
            return new List<GameObject>(){barGameObject};
        }
        private GameObject CreateBar(Vector2 graphPosition, float barWidth)
        {
            GameObject gameObject = new GameObject("bar", typeof(Image));

            gameObject.transform.SetParent(graphContainer, false);
            gameObject.GetComponent<Image>().color = barColor;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(graphPosition.x, 0);
            rectTransform.sizeDelta = new Vector2(barWidth* barWidthMultiplier, graphPosition.y);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.pivot = new Vector2(.5f, 0f);
            return gameObject;
        }
    }
    private class LineGraphVisual: IGraphVisual
    {
        private RectTransform graphContainer;
        private Sprite circleSprite;
        private GameObject lastCircleGameObject;
        private Color circleColor;
        private Color dotConnectionColor;
        public LineGraphVisual(RectTransform graphContainer,Sprite circleSprite, Color circleColor, Color dotConnectionColor)
        {
            this.graphContainer = graphContainer;
            this.circleSprite = circleSprite;
            lastCircleGameObject = null;
            this.circleColor = circleColor;
            this.dotConnectionColor = dotConnectionColor;
        }

        public List<GameObject> AddGraphVisual(Vector2 graphPosition, float graphPositionWith)
        {
            List<GameObject> gameObjectList = new List<GameObject>();
            GameObject circleGameObject = CreateCircle(graphPosition);
            gameObjectList.Add(circleGameObject);
            if (lastCircleGameObject != null)
            {
                GameObject dotConnectionGameObject = CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition,
                    circleGameObject.GetComponent<RectTransform>().anchoredPosition);

                gameObjectList.Add(dotConnectionGameObject);
            }

            lastCircleGameObject = circleGameObject;
            return gameObjectList;
        }

        private GameObject CreateCircle(Vector2 anchoredPosition)
        {
            GameObject gameObject = new GameObject("circle", typeof(Image));

            gameObject.transform.SetParent(graphContainer, false);
            var gameObjectImage = gameObject.GetComponent<Image>();
            gameObjectImage.sprite = circleSprite;
            gameObjectImage.color = circleColor;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = new Vector2(15, 15);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            return gameObject;
        }
        private GameObject CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
        {
            GameObject gameObject = new GameObject("dotConnection", typeof(Image));
            gameObject.transform.SetParent(graphContainer, false);
            gameObject.GetComponent<Image>().color = dotConnectionColor;
            //gameObject.GetComponent<Image>().raycastTarget = false;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

            Vector2 dir = (dotPositionB - dotPositionA).normalized;
            float distance = Vector2.Distance(dotPositionA, dotPositionB);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(distance, 3f);
            rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
            rectTransform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dir));
            return gameObject;
        }
    }

    private class PieChartVisual
    {
        private RectTransform pieContainer;
        private GameObject[] pieCharts = new GameObject[5];
        private Color pieBackColor;
        private Color pieFrontColor;

        public PieChartVisual(RectTransform pieContainer, Color pieBackColor, Color pieFrontColor)
        {
            this.pieContainer = pieContainer;
            this.pieBackColor = pieBackColor;
            this.pieFrontColor = pieFrontColor;
            for (int i = 0; i < pieContainer.childCount; i++)
            {
                pieCharts[i] = pieContainer.GetChild(i).gameObject;
            }
        }

        public void SetPiePercent(float[] fillAmount)
        {
            for (int i = 0; i < pieCharts.Length; i++)
            {
                pieCharts[i].transform.Find("Image").GetComponent<Image>().color = pieBackColor;
                var percentImage = pieCharts[i].gameObject.transform.Find("Image/Percent").GetComponent<Image>();
                var percentText = pieCharts[i].gameObject.transform.Find("PercentText").GetComponent<TextMeshProUGUI>();

                percentImage.color = pieFrontColor;
                percentImage.fillAmount = fillAmount[i]/100;
                percentText.text = "%" + fillAmount[i] + " tamamlandı.";
            }
        }
    }
}