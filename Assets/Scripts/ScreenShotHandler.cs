using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ScreenShotHandler : MonoBehaviour
{
    private static ScreenShotHandler instance;

    private Camera myCamera;
    private bool takeScreenShootOnNextFrame;
    void Awake()
    {
        instance = this;
    }

    private void OnPostRender()
    {
        if (!takeScreenShootOnNextFrame) return;
        takeScreenShootOnNextFrame = false;
        RenderTexture renderTexture = myCamera.targetTexture;

        Texture2D renderResult = new Texture2D(renderTexture.width,renderTexture.height,TextureFormat.ARGB32,false);
            
        Rect rect = new Rect(0,0,renderTexture.width,renderTexture.height);

        renderResult.ReadPixels(rect,0,0);

        byte[] byteArray = renderResult.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath+"/CameraSS.png",byteArray);

        RenderTexture.ReleaseTemporary(renderTexture);
        myCamera.targetTexture = null;

        //Show taken ss
        var fileBytes = File.ReadAllBytes(Application.dataPath + "/CameraSS.png");
        var texture = new Texture2D(2, 2, TextureFormat.RGB24, false);
        texture.LoadImage(fileBytes);

        Sprite mySprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100.0f);
        mySprite.name = "deneme";
       // GameObject.Find("SSImage").GetComponent<Image>().sprite = mySprite;
    }

    private void TakeScreenShot(int width, int height)
    {
        myCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        myCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        takeScreenShootOnNextFrame = true;
    }

    public static void TakeScreenShot_Static(int width, int height)
    {
        instance.TakeScreenShot(width,height);
    }
}
