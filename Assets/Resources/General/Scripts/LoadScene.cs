using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    // 场景名称
    public string sceneName;

    // 场景加载的实例
    private SceneLoaderWithAnimation sceneLoader;
    // 按钮组件
    private Button button;

    void Start()
    {
        // 获取场景加载
        sceneLoader = FindObjectOfType<SceneLoaderWithAnimation>();   
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        sceneLoader.LoadScene(sceneName);
    }
}
