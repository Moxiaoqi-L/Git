using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// 棋子选择器：单例模式
public class SelectCore : MonoBehaviour
{
    public static SelectCore Get = null;

    public GameObject characterDetailField;

    private static CanvasGroup canvasGroup;

    [SerializeField]
    private Chessman selection;
    public static Chessman Selection => Get.selection;
 
    private void Awake()
    {
        Get = this;
        canvasGroup = characterDetailField.GetComponent<CanvasGroup>();
    }

    // 选中
    public static void TrySelect(Chessman chessman)
    {
        // 获取选中的棋子
        Get.selection = chessman;
        // 展示该棋子信息
        canvasGroup.DOFade(1, 0.2f);
    }

    // 取消选中
    public static void DropSelect()
    {
        // 取消选中棋子
        Get.selection = null;
        // 隐藏信息
        canvasGroup.DOFade(0, 0.2f);
    }
}