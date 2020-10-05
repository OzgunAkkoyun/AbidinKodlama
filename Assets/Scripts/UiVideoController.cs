using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UiVideoController : MonoBehaviour
{
    public GameManager gm;
    private GameObject video;
    public AllVideos allVideos;
    public GameObject videoPanel;
    public AllVideosShowed allVideosShowed;
    public AllVideosShowed currentAllVideosShowed;

    public AllVideosShowed.Videos.LevelVideos currentVideoObject;
    public void PrepareAllVideos()
    {
        var allVideosString = PlayerPrefs.GetString("allVideos");

        if (allVideosString != "")
        {
            currentAllVideosShowed = JsonConvert.DeserializeObject<AllVideosShowed>(allVideosString, 
                new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }
        else
        {
            string allVideosString1 = JsonConvert.SerializeObject(allVideosShowed, Formatting.Indented,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
            PlayerPrefs.SetString("allVideos", allVideosString1);
            currentAllVideosShowed = allVideosShowed;
        }
    }

    public void SaveVideos()
    {
        string allVideosJsonString = JsonConvert.SerializeObject(currentAllVideosShowed, Formatting.Indented,
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

        PlayerPrefs.SetString("allVideos", allVideosJsonString);
    }


    public void GetCurrentVideoObject(string videoName)
    {
        currentVideoObject = currentAllVideosShowed.GetVideoObject(gm.playerDatas.whichScenario, videoName);
    }
    public void ShowVideo(string videoName)
    {
        var pickedVideo = allVideos.GetVideo(gm.playerDatas.whichScenario, videoName);

        if (pickedVideo == null)
            return;
        SoundController.instance.Pause("Theme");
        videoPanel.SetActive(true);
        video = videoPanel.transform.Find("VideoPlayer").gameObject;

        var videoPlayer = video.GetComponent<VideoPlayer>();

        videoPlayer.clip = pickedVideo;
        videoPlayer.Play();
        videoPlayer.loopPointReached += CloseVideo;
    }

    public void CloseVideo(VideoPlayer vp)
    {
        StartCoroutine("VideoFadeOut");
        SoundController.instance.Play("Theme");
        currentVideoObject.isShowed = true;
        SaveVideos();
        //gm.playerDatas.showedOpeningVideo = true;
        //gm.PlayerDataSave();
    }

    IEnumerator VideoFadeOut()
    {
        var videoRawImageObject = videoPanel.transform.Find("RawImage");
        var videoRawImage = videoRawImageObject.GetComponent<RawImage>();

        Color curColor = videoRawImage.color;
        var targetAlpha = 0;

        while (curColor.a > 0)
        {
            curColor = videoRawImage.color;
            float alphaDiff = Mathf.Abs(curColor.a - targetAlpha);
            
            curColor.a -= 1.1f * Time.deltaTime;

            videoRawImage.color = curColor;

            yield return new WaitForSeconds(0);
        }

        var image = videoPanel.GetComponent<Image>();

        var tempColor = image.color;
        tempColor.a = 1f;
        image.color = tempColor;
        videoRawImage.color = tempColor;
        videoPanel.SetActive(false);
    }

    public void YonergeButton()
    {
        var videoName = gm.playerDatas.whichScenario + "-" + gm.playerDatas.lastMapSize;

        ShowVideo(videoName);
    }
}
