using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Get = null;

    public AudioSource VocalAudio;
    public AudioSource BGM;
    public AudioSource Sound;

    private void Awake() {
        if (Get == null)
        {
            Get = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioClip audioClip)
    {
        Sound.clip = audioClip;
        Sound.Play();
    }

    public void PlayVocal(AudioClip audioClip)
    {
        VocalAudio.clip = audioClip;
        VocalAudio.Play();
    }

    public void PlayBGM(AudioClip audioClip)
    {
        BGM.clip = audioClip;
        BGM.Play();
    }
}
