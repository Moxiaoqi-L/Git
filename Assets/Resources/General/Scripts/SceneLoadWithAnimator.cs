using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderWithAnimation : MonoBehaviour
{
    // 要切换的场景名称
    public string sceneName;
    // 获取动画组件
    private Animator animator;

    private void Awake() {
        // 获取动画组件
        animator = GetComponent<Animator>();
        // 阻止切换场景是销毁
        DontDestroyOnLoad(gameObject);
    }
    
    // 加载场景的方法
    public void LoadScene(string sceneName)
    {
        StartCoroutine(IELoadScene(sceneName));
    }
    public void LoadScene(string sceneName, Action action)
    {
        StartCoroutine(IELoadScene(sceneName, action));
    }

    // 使用协程加载场景
    private IEnumerator IELoadScene(string sceneName, Action action = null)
    {
        animator.SetBool("FadeIn", true);
        animator.SetBool("FadeOut", false);
        if (action == null)
        {
            yield return new WaitForSeconds(2f);
            AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
            async.completed += CompleteLoadScene;
        }
        else
        {
            yield return new WaitForSeconds(1f);
            action.Invoke();
            CompleteLoadScene();
        }
    }

    // 完成加载
    private void CompleteLoadScene(AsyncOperation operation = null)
    {
        animator.SetBool("FadeIn", false);
        animator.SetBool("FadeOut", true);
    }
}