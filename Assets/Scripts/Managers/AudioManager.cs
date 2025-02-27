using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    // NOTE: May need to make this DontDestroyOnLoad and an instance at some point so can be accessed anywhere
    // Also need to connect to gameManager

    private void Start()
    {
        // NOTE: Empty for now. Add music that wants to play from the start here (e.g. overworld music)
    }

    public void PlaySound(string name)
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
        }
    }

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
}
