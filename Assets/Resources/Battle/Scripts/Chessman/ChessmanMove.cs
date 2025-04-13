
// 通过继承 + 接口实现 图片拖动替换位置。
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



/// 管理UI元素排序：使UI可通过拖动进行位置互换
public class ChessmanMove : Button, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Transform beginParentTransform; //记录开始拖动时的父级对象
    private Transform topOfUiT;
    public Chessman chessman;


    protected override void Start()
    {
        base.Start();
        // 设置到单例中较好，不然每一个物品都会初始化查找
        topOfUiT = GameObject.Find("Canvas").transform;
        chessman = GetComponent<Chessman>();
    }


    public void OnBeginDrag(PointerEventData _)
    {
        if (transform.parent == topOfUiT) return;
        beginParentTransform = transform.parent;
        transform.SetParent(topOfUiT);
    }


    public void OnDrag(PointerEventData _)
    {
        transform.position = Input.mousePosition;
        if (transform.GetComponent<Image>().raycastTarget) transform.GetComponent<Image>().raycastTarget = false;
    }


    public void OnEndDrag(PointerEventData _)
    {
        GameObject go = _.pointerCurrentRaycast.gameObject;
        if(go == null || go.GetComponent<Square>()?.camp == Camp.Enemy){
            SetPosAndParent(transform, beginParentTransform);
            transform.GetComponent<Image>().raycastTarget = true;
            return;            
        }
        if (go.tag == "Square") //如果当前拖动物体下是：格子 时
        {
            SetPosAndParent(transform, go.transform);
            transform.GetComponent<Image>().raycastTarget = true;
            chessman.location = go.GetComponent<Square>().location;
        }
        else //其他任何情况，物体回归原始位置
        {
            SetPosAndParent(transform, beginParentTransform);
            transform.GetComponent<Image>().raycastTarget = true;
        }
    }

    // 设置父物体，UI位置归正
    private void SetPosAndParent(Transform t, Transform parent)
    {
        t.SetParent(parent);
        t.position = parent.position;
    }
}
