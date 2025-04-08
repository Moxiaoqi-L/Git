using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 棋子选择器：单例模式
public class SelectCore : MonoBehaviour
{
    public static SelectCore Get = null;
    [SerializeField]
    private Chessman selection;
    public static Chessman Selection => Get.selection;
 
    private void Awake()
    {
        Get = this;
    }

    // 选中
    public static void TrySelect(Chessman chessman)
    {
        Get.selection = chessman;
    }

    // 取消选中
    public static void DropSelect()
    {
        Get.selection = null;
    }
}