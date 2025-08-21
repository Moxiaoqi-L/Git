using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

// 单例音频管理器
public class AudioManager : MonoBehaviour
{
    // 单例实例
    public static AudioManager Instance;

    // BGM专用AudioSource A/B
    [SerializeField] public AudioSource bgmSourceA;  // BGM轨道A
    [SerializeField] public AudioSource bgmSourceB;  // BGM轨道B
    // 音效对象池大小（根据项目需求调整，小型项目5-10足够）
    [Header("音效设置")]
    [SerializeField] private int sfxPoolSize = 5;

    // 当前活跃的BGM轨道（A或B）
    private AudioSource activeBgmSource;
    private AudioSource inactiveBgmSource;

    // 音效对象池
    private List<AudioSource> sfxPool = new List<AudioSource>();
    // 音量设置（0-1）
    private float bgmVolume = 1f;
    private float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitBgmSources();
            InitSfxPool();
            LoadVolumeSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 初始化BGM轨道
    private void InitBgmSources()
    {
        // 确保两个轨道的基础设置一致
        bgmSourceA.playOnAwake = false;
        bgmSourceA.loop = true;
        bgmSourceA.volume = 0;

        bgmSourceB.playOnAwake = false;
        bgmSourceB.loop = true;
        bgmSourceB.volume = 0;

        // 默认激活轨道A
        activeBgmSource = bgmSourceA;
        inactiveBgmSource = bgmSourceB;
        
    }

    // 初始化音效对象池
    private void InitSfxPool()
    {
        // 创建指定数量的AudioSource并存入池
        for (int i = 0; i < sfxPoolSize; i++)
        {
            GameObject sfxObj = new GameObject($"SFX_Source_{i}");
            sfxObj.transform.parent = transform; // 作为AudioManager的子对象
            AudioSource source = sfxObj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0; // 2D音效（非3D空间音效）
            sfxPool.Add(source);
        }
    }

    // 从对象池获取可用的AudioSource
    private AudioSource GetFreeSfxSource()
    {
        foreach (var source in sfxPool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }
        // 如果池满了，临时创建一个（小型项目偶尔超池影响不大）
        GameObject tempObj = new GameObject("Temp_SFX_Source");
        tempObj.transform.parent = transform;
        AudioSource tempSource = tempObj.AddComponent<AudioSource>();
        tempSource.playOnAwake = false;
        tempSource.spatialBlend = 0;
        sfxPool.Add(tempSource); // 扩容池（可选）
        return tempSource;
    }

    // -------------------- BGM控制方法 --------------------
    /// <summary>
    /// 播放BGM（自动循环）
    /// </summary>
    public void PlayBGM(AudioClip newClip, float crossFadeTime = 1.5f)
    {
        if (newClip == null) return;
        // 如果当前没有播放BGM，直接淡入新BGM
        if (!activeBgmSource.isPlaying)
        {
            // 确保使用当前活跃轨道
            activeBgmSource.clip = newClip;
            activeBgmSource.volume = 0;  // 从0开始
            activeBgmSource.Play();
            StartCoroutine(FadeVolume(activeBgmSource, 0, bgmVolume, 3f));
            return;
        }

        // 1. 准备非活跃轨道播放新BGM（初始音量0）
        inactiveBgmSource.clip = newClip;
        inactiveBgmSource.volume = 0;
        inactiveBgmSource.Play();

        // 2. 同时执行：活跃轨道淡出，非活跃轨道淡入
        StartCoroutine(CrossFadeCoroutine(crossFadeTime));
    }

    /// <summary>
    /// 停止当前BGM（支持淡出）
    /// </summary>
    public void StopBGM(float fadeOutTime = 1.5f)
    {
        if (fadeOutTime > 0)
        {
            StartCoroutine(FadeVolume(activeBgmSource, bgmVolume, 0, fadeOutTime, () => activeBgmSource.Stop()));
        }
        else
        {
            activeBgmSource.Stop();
            activeBgmSource.volume = 0;
        }
    }

    /// <summary>
    /// 通用音量淡入淡出协程（可指定目标AudioSource）
    /// </summary>
    private IEnumerator FadeVolume(AudioSource source, float from, float to, float duration, Action onComplete = null)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        source.volume = to;  // 确保精确到达目标值
        onComplete?.Invoke();
    }

    // 交叉淡入淡出协程（核心逻辑）
    private IEnumerator CrossFadeCoroutine(float duration)
    {
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // 活跃轨道音量：1 → 0（淡出）
            activeBgmSource.volume = Mathf.Lerp(bgmVolume, 0, t);
            // 非活跃轨道音量：0 → 1（淡入）
            inactiveBgmSource.volume = Mathf.Lerp(0, bgmVolume, t);

            yield return null;
        }

        // 确保最终状态正确
        activeBgmSource.volume = 0;
        activeBgmSource.Stop();  // 停止旧BGM
        inactiveBgmSource.volume = bgmVolume;

        // 交换活跃/非活跃轨道（下次切换时使用）
        (activeBgmSource, inactiveBgmSource) = (inactiveBgmSource, activeBgmSource);
    }

    // -------------------- 音效控制方法 --------------------

    /// <summary>
    /// 播放音效（支持多音效同时播放）
    /// </summary>
    public void PlaySFX(AudioClip clip, float pitch = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("SFX clip is null!");
            return;
        }

        AudioSource source = GetFreeSfxSource();
        source.clip = clip;
        source.volume = sfxVolume;
        source.pitch = pitch; // 可通过pitch微调音效（如不同角色的脚步声）
        source.Play();
    }

    /// <summary>
    /// 停止所有音效（支持淡出）
    /// </summary>
    public void StopSFX(float fadeOutTime = 1.5f)
    {
        if (fadeOutTime > 0)
        {
            foreach (var source in sfxPool)
            {
                if (source.isPlaying)
                {
                    StartCoroutine(FadeVolume(source, sfxVolume, 0, fadeOutTime, () =>
                    {
                        source.Stop();
                        source.volume = sfxVolume;
                    }
                    ));
                }
            }
        }
        else
        {
            foreach (var source in sfxPool)
            {
                if (source.isPlaying)
                {
                    source.Stop();
                }
            }
        }
    }

    /// <summary>
    /// 调整音效音量
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        // 更新所有音效源的音量
        foreach (var source in sfxPool)
        {
            source.volume = sfxVolume;
        }
        SaveVolumeSettings(); // 保存设置
    }

    // -------------------- 音量保存与加载 --------------------

    private void SaveVolumeSettings()
    {
        // 用PlayerPrefs保存音量（下次启动保留）
        PlayerPrefs.SetFloat("BGM_Volume", bgmVolume);
        PlayerPrefs.SetFloat("SFX_Volume", sfxVolume);
        PlayerPrefs.Save();
    }

    private void LoadVolumeSettings()
    {
        bgmVolume = PlayerPrefs.GetFloat("BGM_Volume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFX_Volume", 1f);
        bgmSourceA.volume = bgmVolume;
        bgmSourceB.volume = bgmVolume;
    }
}