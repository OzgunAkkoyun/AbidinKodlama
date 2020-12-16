using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraMovementForSS : MonoBehaviour
{
    public UIHandler ui;
    public GameObject ssLayout;
    public float sensitivity;
    public Quaternion cameraRotation;
    public GameObject cameraTarget;

    void Start()
    {
        cameraTarget = GameObject.Find("CameraTarget");
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButton(0) && ssLayout.activeSelf)
        {
            float rotateHorizontal = Input.GetAxis("Mouse X");
            float rotateVertical = Input.GetAxis("Mouse Y");
            Camera.main.transform.RotateAround(Camera.main.transform.position, -Vector3.up,
                rotateHorizontal *
                sensitivity); //use transform.Rotate(-transform.up * rotateHorizontal * sensitivity) instead if you dont want the camera to rotate around the player
            Camera.main.transform.RotateAround(Camera.main.transform.position, Camera.main.transform.right,
                rotateVertical *
                sensitivity); // again, use transform.Rotate(transform.right * rotateVertical * sensitivity) if you don't want the camera to rotate around the player
        }
    }

    public void OpenSSLayout()
    {
        ssLayout.SetActive(true);
    }

    public void TakeSS()
    {
        ScreenShotHandler.TakeScreenShot_Static(1080, 720);
        ssLayout.SetActive(false);
        Invoke("PutCameraToTargetPosition",0.2f);
    }

    public void PutCameraToTargetPosition()
    {
        Camera.main.transform.position = cameraTarget.transform.position;
        Camera.main.transform.rotation = cameraTarget.transform.rotation;
    }
}
