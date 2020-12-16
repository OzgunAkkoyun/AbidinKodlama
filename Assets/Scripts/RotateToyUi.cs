using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RotateToyUi : MonoBehaviour
{
    public PathGenarator pathGenarator;
    public GameManager gm;
    public GetInputs getInputs;

    public GameObject ifObjectContainer;
    public GameObject ifObjectChild;
    public Transform ifObjectBg;

    public GameObject wheelContainer;
    public GameObject whellChilds;

    void Start()
    {
        //CreateWheel();
    }
    public void OpenIfObjectContainer()
    {
        ifObjectContainer.SetActive(true);
    }

    public void OpenIfObjectWheel()
    {
        wheelContainer.SetActive(true);
    }

    public int r = 200;
    public int howManyObject;
    public void CreateWheel()
    {
        //var centerPos = wheelContainer.transform.position;

        //for (int i = 0; i < howManyObject; i++)
        //{
        //    float radius = howManyObject;
        //    float angle = i * Mathf.PI * 2f / radius;
        //    Vector3 newPos = wheelContainer.transform.position + (new Vector3(Mathf.Cos(angle) * radius, -2, Mathf.Sin(angle) * radius));
        //    var a = Instantiate(whellChilds, newPos, Quaternion.Euler(0, 0, 0));
        //    a.transform.SetParent(wheelContainer.transform);
        //}
        var numPoints = 2;
        var centrePos = wheelContainer.transform.position;
        var radiusX = 100;
        var radiusY = 100;
        for (var pointNum = 0; pointNum < numPoints; pointNum++)
        {
            // "i" now represents the progress around the circle from 0-1
            // we multiply by 1.0 to ensure we get a fraction as a result.
            var i = (pointNum * 1.0) / numPoints;
            // get the angle for this step (in radians, not degrees)
            var angle = i * Mathf.PI * 2;
            // the X &amp; Y position for this angle are calculated using Sin &amp; Cos
            var x = Mathf.Sin((float)angle) * radiusX;
            var y = Mathf.Cos((float)angle) * radiusY;
            var pos = new Vector3(x, y, 0) + centrePos;
            // no need to assign the instance to a variable unless you're using it afterwards:
            var a = Instantiate(whellChilds, pos, Quaternion.identity);
            a.transform.SetParent(wheelContainer.transform);
        }


    }

    public void SetAllIfObjectsInContainer(int objectCount)
    {
        var animals = pathGenarator.currentAnimals.animals;
        //int[] animalsIndexs = new []{0,1,2};
        for (int i = 0; i < objectCount; i++)
        {
            var ifObjectChildInstantiated = Instantiate(ifObjectChild);
            ifObjectChildInstantiated.transform.SetParent(ifObjectBg);

            SetIfObjectsImage(ifObjectChildInstantiated, ref animals,i);
            
        }
    }

    public void SetAllIfObjectsInWheel(int objectCount)
    {
        var animals = pathGenarator.currentAnimals.animals;
        //int[] animalsIndexs = new []{0,1,2};
        for (int i = 0; i < 8; i++)
        {
            var child = wheelContainer.transform.GetChild(i);
            var childImage = child.Find("Image");
            if (i < objectCount)
            {
                var getRandomIfObject = animals[i];
                childImage.GetComponent<Image>().sprite = animals[i].animalsGameObjectsImage;

                child.GetComponent<Button>().onClick.AddListener(() => CloseIfObjectWheel(getRandomIfObject));

                child.GetComponent<Button>().onClick.AddListener(() => getInputs.GetIfInput(getRandomIfObject.ifName));
            }
            else
            {
                child.GetComponent<Button>().interactable = false;
                childImage.gameObject.SetActive(false);
            }
        }
    }

    private void SetIfObjectsImage(GameObject ifObjectChildInstantiated,
        ref PathGenarator.IfObjects.AnimalForLevel[] animals, int i)
    {
        //var index = Random.Range(0, animalsIndexs.Length);

        var getRandomIfObject = animals[i];

        ifObjectChildInstantiated.GetComponent<Image>().sprite =
            getRandomIfObject.animalsGameObjectsImage;

        ifObjectChildInstantiated.GetComponent<Button>().onClick.AddListener(() => CloseIfObjectContainer(getRandomIfObject));

        ifObjectChildInstantiated.GetComponent<Button>().onClick.AddListener(()=>getInputs.GetIfInput(getRandomIfObject.ifName));

        //RemoveAt(ref animalsIndexs, index);
    }
    private void CloseIfObjectWheel(PathGenarator.IfObjects.AnimalForLevel getRandomIfObject)
    {
        wheelContainer.SetActive(false);
        pathGenarator.selectedAnimals.Add(getRandomIfObject);
    }

    private void CloseIfObjectContainer(PathGenarator.IfObjects.AnimalForLevel getRandomIfObject)
    {
        ifObjectContainer.SetActive(false);
        pathGenarator.selectedAnimals.Add(getRandomIfObject);
    }

    public static void RemoveAt<T>(ref T[] arr, int index)
    {
        // replace the element at index with the last element
        arr[index] = arr[arr.Length - 1];
        // finally, let's decrement Array's size by one
        Array.Resize(ref arr, arr.Length - 1);
    }
}
