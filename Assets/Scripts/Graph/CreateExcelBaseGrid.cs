using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateExcelBaseGrid : MonoBehaviour
{
    public RectTransform cellContainer;
    public RectTransform cell;
    public Vector2 cellWidthHeigth = new Vector2(160,50);

    void Start()
    {
        CreateTable();
    }
    public void CreateTable()
    {
        for (int i = 0; i < 20; i++)
        {
            var cell1 = Instantiate(cell);

            cell1.SetParent(cellContainer);
            cell1.anchoredPosition = new Vector2(160*i, 0);
            //labelX.localScale = Vector3.one;
        }
       
    }
}
