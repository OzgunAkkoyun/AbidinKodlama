using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ToyIdentifier 
{
    private List<Vector2> touchList = new List<Vector2>();
    List<Vector2> sortedTouchList = new List<Vector2>();
    //public AllToys toys;
    public int whichToy = 0;

    public GameObject alldots;

    private float circleCount = 15;

    public List<GameObject> circlePoints = new List<GameObject>();

    public ToyIdentifier(List<Vector2> touchList)
    {
        this.touchList = touchList;
        alldots = GameObject.Find("Alldots");
    }

    public void FindTriangleLefttoRight(List<Vector2> _touchList)
    {
        alldots.transform.rotation = Quaternion.identity;
        circleIndex.Clear();
        dotsIndex.Clear();
        sortedTouchList.Clear();
        nearestObjectIndex = 0;
        Debug.Log(_touchList.ListPrint());
        var touchListTemp = _touchList;
        var sortedVectorsX = touchListTemp.OrderBy(v => v.x).ToArray<Vector2>();
        var firstX = sortedVectorsX[0];
        var lastX = sortedVectorsX[2];

        var P = touchListTemp.Where(v => (v != firstX && v != lastX)).ToList().First();

        var k = lastX - firstX;
        var l = P - firstX;

        var kontrol = k.x * l.y - k.y * l.x;

        if (kontrol > 0)
        {
            sortedTouchList.Add(firstX);
            sortedTouchList.Add(P);
            sortedTouchList.Add(lastX);
        }
        else if (kontrol < 0)
        {
            sortedTouchList.Add(firstX);
            sortedTouchList.Add(lastX);
            sortedTouchList.Add(P);
        }
        else
        {
            Debug.Log("üzerinde");
        }

        List<float> dist = new List<float>();

        dist.Add(Vector2.Distance(sortedTouchList[0], sortedTouchList[1]));
        dist.Add(Vector2.Distance(sortedTouchList[1], sortedTouchList[2]));
        dist.Add(Vector2.Distance(sortedTouchList[0], sortedTouchList[2]));

        ShowCircle();
    }

    public float x_c = 0f; //Circle's center coordinate (X Axis).
    public float y_c = 0f; //Circle's center coordinate (Y Axis).
    public float _r = 0f; //Circle's radius.
    public int nearestObjectIndex = 0;
    public void ShowCircle()
    {
        BisectCircleToCoordinates(x_c, y_c, _r, sortedTouchList[0], sortedTouchList[1], sortedTouchList[2]);
        var circleStartPos = new Vector2(x_c, y_c);

        alldots.transform.position = circleStartPos;

        var t = 360 * Mathf.Deg2Rad;

        var angle = 360 / circleCount;

        circlePoints.Clear();

        for (int i = 0; i < GameObject.Find("Alldots").transform.childCount; i++)
        {
            circlePoints.Add(alldots.transform.GetChild(i).gameObject);
            //if (i >= circleCount)
            //{
            //    alldots.transform.GetChild(i).transform.position = new Vector3(-1000,0,0);
            //}
        }

        for (int i = 0; i < circleCount; i++)
        {
            float x = _r * Mathf.Cos(t) + circleStartPos.x;
            float y = _r * Mathf.Sin(t) + circleStartPos.y;
            circlePoints[i].transform.position = new Vector2(x, y);
            t -= angle * Mathf.Deg2Rad;
        }

        SnapOneDot();
    }

    void BisectCircleToCoordinates(float _x, float _y, float __r, Vector2 _A, Vector2 _B, Vector2 _C)
    {
        var b1 = new Vector2((_A.x + _B.x) / 2f, (_A.y + _B.y) / 2f); //Find center point between A and B
        var b2 = new Vector2((_B.x + _C.x) / 2f, (_B.y + _C.y) / 2f); //Find center point between A and B

        var m1 = -1 / ((_B.y - _A.y + 0.01f) / (_B.x - _A.x + 0.01f)); //find the slope and it's inverse signed reciprocal to convert direction to perpendicular
        var m2 = -1 / ((_C.y - _B.y + 0.01f) / (_C.x - _B.x + 0.01f)); //find the slope and it's inverse signed reciprocal to convert direction to perpendicular

        //Results:
        x_c = _x = (-(m1 * b1.x) + b1.y - b2.y + (m2 * b2.x)) / (m2 - m1);
        y_c = _y = m1 * _x - m1 * b1.x + b1.y;

        _r = __r = Vector2.Distance(new Vector2(_x, _y), _A);
    }

    List<Vector2> dotsPositions = new List<Vector2>();
    List<int> circleIndex = new List<int>();
    List<int> dotsIndex = new List<int>();
    public void SnapOneDot()
    {
        dotsPositions = sortedTouchList;
        List<GameObject> nearestList = new List<GameObject>();

        for (int i = 0; i < dotsPositions.Count; i++)
        {
            nearestList.Add(circlePoints.OrderBy(x => Vector2.Distance(x.transform.position, dotsPositions[i])).First());
        }

        List<float> nearestDistance = new List<float>();

        for (int i = 0; i < nearestList.Count; i++)
        {
            nearestDistance.Add(Vector2.Distance(nearestList[i].transform.position, dotsPositions[i]));
        }

        var a = nearestDistance.OrderBy(x => x).ToList();

        var nearestIndex = nearestDistance.FindIndex(x => x == a[nearestObjectIndex]);

        var cosa = (2 * _r * _r - a[nearestObjectIndex] * a[nearestObjectIndex]) / (2 * _r * _r);


        var angle = Mathf.Acos(cosa) * Mathf.Rad2Deg;

        var firstX = new Vector2(x_c, y_c);
        var lastX = nearestList[nearestIndex].transform.position.Vector3ToVector2();

        var P = dotsPositions[nearestIndex];

        var k = lastX - firstX;
        var l = P - firstX;

        var kontrol = k.x * l.y - k.y * l.x;

        if (kontrol > 0)
        {
            alldots.transform.Rotate(new Vector3(0, 0, angle));
        }
        else if (kontrol < 0)
        {
            alldots.transform.Rotate(new Vector3(0, 0, -angle));
        }
        else
        {
            Debug.Log("üzerinde");
        }

        if (nearestObjectIndex == 0)
        {
            nearestObjectIndex++;
            //circleIndex.Add(circlePoints.FindIndex(point => MathTools.Equal(Camera.main.ScreenToWorldPoint(point.transform.position), Camera.main.ScreenToWorldPoint(nearestList[nearestIndex].transform.position), new Vector2(0.1f, 0.1f))));
            circleIndex.Add(circlePoints.FindIndex(point => MathTools.Equal(point.transform.position, nearestList[nearestIndex].transform.position, new Vector2(0.1f, 0.1f))));
           
            //Debug.Log(circlePoints.Select(v=>v.transform.position).ToList().ListPrint());
            //Debug.Log(circlePoints.Select(v => Camera.main.ScreenToWorldPoint(v.transform.position + new Vector3(0,0,1f))).ToList().ListPrint());


            //Debug.Log(nearestList[nearestIndex].transform.position);
            //Debug.Log(Camera.main.ScreenToWorldPoint(nearestList[nearestIndex].transform.position));

            //dotsIndex.Add(sortedTouchList.ToList().FindIndex(point => MathTools.Equal(Camera.main.ScreenToWorldPoint(point), Camera.main.ScreenToWorldPoint(circlePoints[circleIndex[0]].transform.position), new Vector2(0.1f, 0.1f))));
            dotsIndex.Add(sortedTouchList.ToList().FindIndex(point => MathTools.Equal(point, circlePoints[circleIndex[0]].transform.position, new Vector2(0.1f, 0.1f))));
            SnapOneDot();
        }
        else if (nearestObjectIndex == 1)
        {
            sortedTouchList[dotsIndex[0]] = circlePoints[circleIndex[0]].transform.position.Vector3ToVector2();

            //circleIndex.Add(circlePoints.FindIndex(point => MathTools.Equal(Camera.main.ScreenToWorldPoint(point.transform.position), Camera.main.ScreenToWorldPoint(nearestList[nearestIndex].transform.position), new Vector2(0.1f, 0.1f))));
            circleIndex.Add(circlePoints.FindIndex(point => MathTools.Equal(point.transform.position, nearestList[nearestIndex].transform.position, new Vector2(0.1f, 0.1f))));

            //dotsIndex.Add(sortedTouchList.ToList().FindIndex(point => MathTools.Equal(Camera.main.ScreenToWorldPoint(point), Camera.main.ScreenToWorldPoint(circlePoints[circleIndex[1]].transform.position), new Vector2(0.1f, 0.1f))));
            dotsIndex.Add(sortedTouchList.ToList().FindIndex(point => MathTools.Equal(point, circlePoints[circleIndex[1]].transform.position, new Vector2(0.1f, 0.1f))));

            nearestObjectIndex++;
            SnapOneDot();
        }
        else if (nearestObjectIndex == 2)
        {
            sortedTouchList[dotsIndex[0]] = circlePoints[circleIndex[0]].transform.position.Vector3ToVector2();
            sortedTouchList[dotsIndex[1]] = circlePoints[circleIndex[1]].transform.position.Vector3ToVector2();

            //circleIndex.Add(circlePoints.FindIndex(point => MathTools.Equal(Camera.main.ScreenToWorldPoint(point.transform.position), Camera.main.ScreenToWorldPoint(nearestList[nearestIndex].transform.position), new Vector2(0.1f, 0.1f))));
            circleIndex.Add(circlePoints.FindIndex(point => MathTools.Equal(point.transform.position, nearestList[nearestIndex].transform.position, new Vector2(0.1f, 0.1f))));

            //dotsIndex.Add(sortedTouchList.ToList().FindIndex(point => MathTools.Equal(Camera.main.ScreenToWorldPoint(point), Camera.main.ScreenToWorldPoint(circlePoints[circleIndex[2]].transform.position), new Vector2(0.1f, 0.1f))));
            dotsIndex.Add(sortedTouchList.ToList().FindIndex(point => MathTools.Equal(point, circlePoints[circleIndex[2]].transform.position, new Vector2(0.1f, 0.1f))));
            var maxOffset = new Vector2(0.1f, 0.1f);

            circleIndex.Sort();

            while (circleIndex[0] != 0)
            {
                for (int i = 0; i < circleIndex.Count; i++)
                {
                    circleIndex[i] = circleIndex[i] + 1;
                    if (circleIndex[i] == 15)
                    {
                        circleIndex[i] = 0;
                    }
                }
            }

            //FindZeroOneTriangle();
        }
    }

    public string FindZeroOneTriangle(ToyIdentifier toy1, List<Toy> toys)
    {
        FindTriangleLefttoRight(toy1.touchList);

        var fark1 = (float)Mathf.Abs(circleIndex[1] - circleIndex[0]) - 1;
        var fark2 = (float)Mathf.Abs(circleIndex[2] - circleIndex[1]) - 1;
        var fark3 = (float)(12 - (fark1 + fark2));

        List<float> currentSpaces = new List<float>();
        currentSpaces.Add(fark1);
        currentSpaces.Add(fark2);
        currentSpaces.Add(fark3);

        var lowestDist = currentSpaces.OrderBy(v => v).First();
        var lowestDistIndex = currentSpaces.FindIndex(e => e == lowestDist);

        while (lowestDistIndex != 0)
        {
            var lastElement = currentSpaces[currentSpaces.Count - 1];
            currentSpaces.RemoveAt(2);
            currentSpaces.Insert(0, lastElement);
            lowestDistIndex = currentSpaces.FindIndex(e => e == lowestDist);
        }

        whichToy = 0;
        foreach (var toy in toys)
        {
            var toyDistances = toy.distances;
            if (toyDistances[0] == currentSpaces[0] && toyDistances[1] == currentSpaces[1] && toyDistances[2] == currentSpaces[2])
            {
                return toys[whichToy].toolName;
                break;
            }
         
            whichToy++;
        }
        return "Default";
    }
}
