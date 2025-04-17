using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 棋子选择器：单例模式
public class ButtonSelectCore : MonoBehaviour
{
    public static ButtonSelectCore Get = null;
    [SerializeField]
    private Button selection;
    public static Button Selection => Get.selection;
 
    private void Awake()
    {
        Get = this;
    }

    // 选中
    public static void TrySelect(Button button)
    {
        Get.selection = button;
    }

    // 取消选中
    public static void DropSelect()
    {
        Get.selection = null;
    }
}