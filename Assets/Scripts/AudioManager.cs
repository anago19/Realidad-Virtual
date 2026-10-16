using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioClip normalMusic;
    public AudioClip tenseMusic;
    public AudioClip chaseMusic;

    public void SetTenseMusic()
    {
        musicSource.clip = tenseMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void SetChaseMusic()
    {
        musicSource.clip = chaseMusic;
        musicSource.loop = true;
        musicSource.Play();
    }
}

