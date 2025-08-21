using UnityEngine;
using System.Collections;

// 全局协程管理器，负责代理非MonoBehaviour类的协程
public class CoroutineManager : MonoBehaviour
{
    // 单例实例
    public static CoroutineManager Instance { get; private set; }

    private void Awake()
    {
        // 确保全局唯一
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 跨场景持久化
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 提供启动协程的方法（供外部调用）
    public Coroutine StartCoroutineEx(IEnumerator coroutine)
    {
        return StartCoroutine(coroutine);
    }

    // 提供停止协程的方法
    public void StopCoroutineEx(Coroutine coroutine)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
    }
}