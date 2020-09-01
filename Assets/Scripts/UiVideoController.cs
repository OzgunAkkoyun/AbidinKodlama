using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UiVideoController : MonoBehaviour
{
    public GameManager gm;
    private GameObject video;
    public AllVideos allVideos;
    public GameObject videoPanel;

    public void ShowVideo(string videoName)
    {
        var pickedVideo = allVideos.GetVideo(gm.playerDatas.whichScenario, videoName);

        if (pickedVideo == null)
            return;
        gm.sc.Pause("Theme");
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
        gm.sc.Play("Theme");
        gm.playerDatas.showedOpeningVideo = true;
        gm.PlayerDataSave();
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
