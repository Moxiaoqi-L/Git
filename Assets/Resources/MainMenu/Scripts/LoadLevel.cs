using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadLevel : MonoBehaviour
{
    private Button button;
    public string sceneName;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("获取了" + button);
        button = GetComponent<Button>();
        button.onClick.AddListener(ButtonClick);
    }

    private void ButtonClick()
    {
        Debug.Log("点了");
        TransitionManager.Instance.StartTransition(sceneName);
    }
}
