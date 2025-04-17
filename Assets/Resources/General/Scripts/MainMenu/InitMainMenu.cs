using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitMainMenu : MonoBehaviour
{
    public AudioClip audioClip;
    private AudioSource BGM;

    void Start()
    {
        BGM = GameObject.Find("BGM").GetComponent<AudioSource>();
        BGM.clip = audioClip;
        BGM.Play();
    }
}
