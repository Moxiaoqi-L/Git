using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InitMainMenu : MonoBehaviour
{
    public AudioClip audioClip;

    void Start()
    {
        if (AudioManager.Get.BGM.clip != audioClip) AudioManager.Get.PlayBGM(audioClip);
    }
}
