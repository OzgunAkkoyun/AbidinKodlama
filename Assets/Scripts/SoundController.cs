using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    public Sound[] sounds;
    public bool toggle = false;
    public Sprite[] soundImages;
    public AudioListener listener;
    public static SoundController instance;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
    }
    
    public void Play(string name)
    {
       Sound s = Array.Find(sounds, sound => sound.name == name);
       s.source.Play();
    }

    public void Pause(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Pause();
    }

    public void ToggleSound()
    {
        toggle = !toggle;
        var soundButton = EventSystem.current.currentSelectedGameObject;

        if (toggle)
        {
            soundButton.GetComponent<Image>().sprite = soundImages[1];
            sounds.ToList().ForEach( sound => sound.source.volume = 0);
        }
        else
        {
            soundButton.GetComponent<Image>().sprite = soundImages[0];
            sounds.ToList().ForEach(sound => sound.source.volume = 1);
        }
    }
}