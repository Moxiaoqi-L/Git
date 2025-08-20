using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InitMainMenu : MonoBehaviour
{
    public AudioClip audioClip;

    void Start()
    {
        if (AudioManager.Instance.bgmSourceA.clip != audioClip && AudioManager.Instance.bgmSourceB.clip != audioClip) AudioManager.Instance.PlayBGM(audioClip);
    }
}
