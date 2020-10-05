using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(menuName = "Abidin/Videos/AllVideos", order = 1)]
public class AllVideos : ScriptableObject
{
    public VideoContainer[] senarioVideos;

    public VideoClip GetVideo(int scenario, string videoName)
    {
        var videos = senarioVideos[scenario - 1].videos;
        var pickedVideo = videos.FirstOrDefault(element => element.name == videoName);

        return pickedVideo.video;
    }
  
}
