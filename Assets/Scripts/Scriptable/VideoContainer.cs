﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(menuName = "Abidin/Videos/Video", order = 1)]
public class VideoContainer : ScriptableObject
{
    [Serializable]
    public struct Videos
    {
        public string name;
        public VideoClip video;
    }
    public Videos[] videos;
}