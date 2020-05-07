using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetInputs : MonoBehaviour
{
    [HideInInspector]
    public List<KeyCode> inputs = new List<KeyCode>();
    public List<GameObject> codeInputsObjects = new List<GameObject>();

    public GameObject codeInputObject;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            inputs.Add(KeyCode.UpArrow);
            ShowKeys(90);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            inputs.Add(KeyCode.LeftArrow);
            ShowKeys(180);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            inputs.Add(KeyCode.RightArrow);
            ShowKeys(0);

        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            inputs.Add(KeyCode.DownArrow);
            ShowKeys(-90);

        }
    }

    void ShowKeys(int keyRotate)
    {
        Debug.Log("key");
        //var panel = GameObject.Find("CodePanel/Scroll");
        //var codeInput = Instantiate(codeInputObject, codeInputObject.transform.position, Quaternion.identity);
        //codeInputsObjects.Add(codeInput);
        //codeInput.transform.localScale = new Vector3(1,1,1);
        //codeInput.transform.parent = panel.transform;
        //var arrow = codeInput.transform.Find("Image");
        //arrow.gameObject.transform.Rotate(new Vector3(0,0, keyRotate));
        //GameObject.Find("CodePanel").GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 0);
    }
}
