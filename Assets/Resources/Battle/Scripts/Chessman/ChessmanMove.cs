
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

    private GameObject currentObject;

    public Chessman chessman;
    // 新增：移动完成事件（供Hero监听）
    public event Action<Hero> OnMoveCompleted;


    protected override void Start()
    {
        base.Start();
        // 设置到单例中较好，不然每一个物品都会初始化查找
        topOfUiT = GameObject.Find("Canvas").transform;
        chessman = GetComponent<Chessman>();
    }


    public void OnBeginDrag(PointerEventData _)
    {
        currentObject = _.pointerCurrentRaycast.gameObject;
        if (transform.parent == topOfUiT) return;
        beginParentTransform = transform.parent;
        transform.SetParent(topOfUiT);
        // 取消显示攻击范围（当前Hero的攻击范围）
        // 同时取消显示技能按钮
        if (chessman.camp == Camp.Player && chessman.hero != null)
        {
            Chessman targetChessman = chessman;
            chessman.DestroySkillButton();
            // 调用Chessman的取消方法
            if (SelectCore.Selection == chessman) targetChessman.HighlightAttackRange(targetChessman.hero, false);
            else if (SelectCore.Selection && SelectCore.Selection != chessman) SelectCore.Selection.HighlightAttackRange(SelectCore.Selection.hero, false);
        }
    }


    public void OnDrag(PointerEventData _)
    {
        // 获取相机（假设UI的Canvas绑定了对应的相机，通常为MainCamera）
        Camera canvasCamera = GetComponentInParent<Canvas>().worldCamera;
        if (canvasCamera == null) canvasCamera = Camera.main; // 备用相机
        
        // 转换屏幕坐标到世界坐标（z轴设为UI的深度，避免被相机裁剪）
        Vector3 worldPosition = canvasCamera.ScreenToWorldPoint(new Vector3(_.position.x, _.position.y, 10f)); // z设为相机前10单位
        transform.position = worldPosition;
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
        if (go.tag == "Square" && currentObject.GetComponent<Hero>().GetMoveRange().Contains(go.GetComponent<Square>().location) && APMPManager.Get.ConsumeMP()) //如果当前拖动物体下是：格子 时     
        {
            SetPosAndParent(transform, go.transform);
            transform.GetComponent<Image>().raycastTarget = true;
            chessman.location = go.GetComponent<Square>().location;
            // 触发英雄位移事件
            OnMoveCompleted?.Invoke(chessman.hero);
            // 重新显示新位置的攻击范围（仅当是玩家阵营时）
            if (chessman != null && SelectCore.Selection != null && chessman.camp == Camp.Player && chessman.hero != null && SelectCore.Selection.hero == chessman.hero)
            {
                Chessman targetChessman = chessman;
                targetChessman.HighlightAttackRange(targetChessman.hero, true); // 调用Chessman的显示方法
            }
        }
        else //其他任何情况，物体回归原始位置
        {
            SetPosAndParent(transform, beginParentTransform);
            transform.GetComponent<Image>().raycastTarget = true;
            // 重新显示新位置的攻击范围（仅当是玩家阵营时）
            if (chessman != null && SelectCore.Selection != null && chessman.camp == Camp.Player && chessman.hero != null && SelectCore.Selection.hero == chessman.hero)
            {
                Chessman targetChessman = chessman;
                targetChessman.HighlightAttackRange(targetChessman.hero, true); // 调用Chessman的显示方法
            }
        }
    }

    // 设置父物体，UI位置归正
    private void SetPosAndParent(Transform t, Transform parent)
    {
        t.SetParent(parent);
        t.position = parent.position;
    }
}
