using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ABLoader : MonoBehaviour
{
    // AB包清单（存储依赖关系）
    private static AssetBundleManifest _manifest;
    // 已加载的AB包缓存（避免重复加载）
    private static Dictionary<string, AssetBundle> _loadedABs = new();

    /// <summary>
    /// 初始化：加载AB包清单（首次加载时调用）
    /// </summary>
    public static IEnumerator InitManifest()
    {
        // 清单文件所在的AB包名称（构建时自动生成，通常为"AssetBundles"，需与构建路径对应）
        // 注意：名称需与构建输出的清单包名一致（小写）
        string manifestABName = "PC"; 

        // 1. 先从热更新目录加载清单
        string hotUpdateManifestPath = Path.Combine(Application.persistentDataPath, "AssetBundles", "HotUpdate", manifestABName);
        AssetBundle manifestAB = null;

        if (File.Exists(hotUpdateManifestPath))
        {
            // PC端用LoadFromFile（同步高效）
            manifestAB = AssetBundle.LoadFromFile(hotUpdateManifestPath); 
        }
        // 2. 热更新目录不存在，从基础目录加载
        else
        {
            string baseManifestPath = Path.Combine(Application.streamingAssetsPath, manifestABName);
            if (File.Exists(baseManifestPath))
            {
                manifestAB = AssetBundle.LoadFromFile(baseManifestPath);
            }
            else
            {
                Debug.LogError("AB包清单不存在！路径：" + baseManifestPath);
                yield break;
            }
        }

        // 获取清单数据
        _manifest = manifestAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        // 释放清单包体，保留清单对象
        manifestAB.Unload(false); 
        Debug.Log("AB清单初始化完成");
    }

    /// <summary>
    /// 异步加载AB包及其中的资源
    /// </summary>
    /// <typeparam name="T">资源类型（如Sprite、GameObject等）</typeparam>
    /// <param name="abName">AB包名称（如"ui/skills"）</param>
    /// <param name="assetName">资源名称（如"RetaliationAttackReduction"）</param>
    /// <param name="onLoaded">加载完成回调</param>
    public static IEnumerator LoadAssetAsync<T>(string abName, string assetName, Action<T> onLoaded) where T : UnityEngine.Object
    {
        if (_manifest == null)
        {
            Debug.LogError("请先初始化AB清单！");
            yield break;
        }

        // 1. 加载依赖的AB包
        string[] dependencies = _manifest.GetAllDependencies(abName);
        foreach (string depABName in dependencies)
        {
            if (!_loadedABs.ContainsKey(depABName))
            {
                yield return LoadAB(depABName);
            }
        }

        // 2. 加载目标AB包
        yield return LoadAB(abName);
        if (!_loadedABs.TryGetValue(abName, out AssetBundle targetAB))
        {
            Debug.LogError("加载AB包失败：" + abName);
            onLoaded?.Invoke(null);
            yield break;
        }

        // 3. 从AB包中加载资源
        var request = targetAB.LoadAssetAsync<T>(assetName);
        if (request == null)
        {
            Debug.LogError("加载AB包《" + abName + "》成功！获取包内资源：" + assetName + "失败！");
        }
        yield return request;

        T asset = request.asset as T;
        onLoaded?.Invoke(asset);
    }

    /// <summary>
    /// 加载单个AB包（内部使用，处理路径优先级）
    /// </summary>
    private static IEnumerator LoadAB(string abName)
    {
        // 检查是否已加载
        if (_loadedABs.ContainsKey(abName)) yield break;

        AssetBundle ab = null;
        string abPath = string.Empty;

        // 优先从热更新目录加载
        abPath = Path.Combine(Application.persistentDataPath, "AssetBundles", "HotUpdate", abName);
        if (File.Exists(abPath))
        {
            ab = AssetBundle.LoadFromFile(abPath); // PC端同步加载效率高
        }
        // 热更新目录不存在，从基础目录加载
        else
        {
            abPath = Path.Combine(Application.streamingAssetsPath, abName);
            if (File.Exists(abPath))
            {
                ab = AssetBundle.LoadFromFile(abPath);
            }
            // 路径不存在，尝试用UnityWebRequest加载（兼容特殊路径）
            else
            {
                using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle("file://" + abPath))
                {
                    yield return www.SendWebRequest();
                    if (www.result == UnityWebRequest.Result.Success)
                    {
                        ab = DownloadHandlerAssetBundle.GetContent(www);
                    }
                    else
                    {
                        Debug.LogError("AB包加载失败：" + abPath + "，错误：" + www.error);
                        yield break;
                    }
                }
            }
        }

        if (ab != null)
        {
            _loadedABs[abName] = ab;
            Debug.Log("AB包加载成功：" + abName);
        }
        else
        {
            Debug.LogError("AB包为空：" + abPath);
        }
    }

    /// <summary>
    /// 卸载AB包（释放资源时调用）
    /// </summary>
    /// <param name="abName">AB包名称</param>
    /// <param name="unloadAllLoadedObjects">是否同时卸载从该包加载的资源</param>
    public static void UnloadAB(string abName, bool unloadAllLoadedObjects = false)
    {
        if (_loadedABs.TryGetValue(abName, out AssetBundle ab))
        {
            ab.Unload(unloadAllLoadedObjects);
            _loadedABs.Remove(abName);
            Debug.Log("AB包已卸载：" + abName);
        }
    }

    /// <summary>
    /// 卸载所有AB包（如场景切换时）
    /// </summary>
    public static void UnloadAllABs(bool unloadAllLoadedObjects = false)
    {
        foreach (var ab in _loadedABs.Values)
        {
            ab.Unload(unloadAllLoadedObjects);
        }
        _loadedABs.Clear();
        Debug.Log("所有AB包已卸载");
    }
}