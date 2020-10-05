using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Abidin/Videos/AllVideosShowed", order = 1)]
public class AllVideosShowed : ScriptableObject
{
    [Serializable]
    public class Videos
    {
        [Serializable]
        public class LevelVideos
        {
            public string name;
            public bool isShowed;
        }

        public LevelVideos[] levelVideos;
    }
    public Videos[] videos;

    public Videos.LevelVideos GetVideoObject(int scenario, string videoName)
    {
        var videosObject = videos[scenario - 1].levelVideos;
        var pickedVideo = videosObject.FirstOrDefault(element => element.name == videoName);

        return pickedVideo;
    }
}
