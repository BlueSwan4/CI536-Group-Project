using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    // Based off Rehope Games Audio Manager Tutorial
    public static AudioManager Instance;

    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    // NOTE: May need to make this DontDestroyOnLoad and an instance at some point so can be accessed anywhere
    // Also need to connect to gameManager

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        Debug.Log(musicSource.isPlaying);
    }

    // Pass in the name of the music you want to play and it plays
    // Called from GameManager when states switch
    public void PlayMusic(string name)
    {
        // From what I understand this is a shortcut of a search algorithm
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if(s == null)
        {
            Debug.Log("No sound with this name");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
            Debug.Log("Hey");
        }
    }

    // Pass in the name of the sound effect you want to play and it plays
    public void PlaySFX(string name)
    {
        // From what I understand this is a shortcut of a search algorithm
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("No sfx with this name");
        }
        else
        {
            sfxSource.clip = s.clip;
            sfxSource.Play();
        }
    }

    // Functions can be called from settings to have sliders to change volume
    public void ChangeMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void ChangeSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
