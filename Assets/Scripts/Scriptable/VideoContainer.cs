using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(menuName = "Abidin/Video", order = 1)]
public class VideoContainer : ScriptableObject
{
    [Serializable]
    public struct Videos
    {
        public string name;
        public VideoClip video;
        public bool isVideoShowed;
    }
    public Videos[] videos;
}