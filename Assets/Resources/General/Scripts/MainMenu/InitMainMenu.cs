using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InitMainMenu : MonoBehaviour
{
    public AudioClip audioClip;

    private void Awake() {
        //游戏初始化时先初始化清单
        StartCoroutine(ABLoader.InitManifest());
    }

    void Start()
    {
        if (AudioManager.Instance.bgmSourceA.clip != audioClip && AudioManager.Instance.bgmSourceB.clip != audioClip) AudioManager.Instance.PlayBGM(audioClip);
    }
}
