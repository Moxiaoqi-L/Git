using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ImageScrollAligner : MonoBehaviour
{
    [Header("组件引用")]
    public ScrollRect scrollRect;
    public RectTransform content;
    public RectTransform pointer;
    public float smoothSpeed = 3f;

    private List<RectTransform> imageList = new();

    private void Start()
    {
        // 延迟一帧确保布局组件初始化完成
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        CollectImages();
        AddClickListeners();
        // 初始指向第一张照片
        StartCoroutine(SmoothScrollTo(-275));
    }

    private void CollectImages()
    {
        imageList.Clear();
        foreach (Transform child in content)
        {
            RectTransform imgRt = child.GetComponent<RectTransform>();
            if (imgRt != null) imageList.Add(imgRt);
        }
    }

    private void AddClickListeners()
    {
        foreach (RectTransform img in imageList)
        {
            EventTrigger trigger = img.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry clickEntry = new EventTrigger.Entry();
            clickEntry.eventID = EventTriggerType.PointerClick;
            clickEntry.callback.AddListener((data) => OnImageClick(img));
            trigger.triggers.Add(clickEntry);
        }
    }

    private void OnImageClick(RectTransform clickedImage)
    {
        // 计算指针在屏幕空间的Y坐标（假设指针垂直对齐，X固定）
        float pointerY = pointer.position.y;
        // 计算图片中心在屏幕空间的Y坐标
        float imgScreenCenterY = clickedImage.position.y + clickedImage.rect.height * 0.5f;
        // 计算content需要移动的Y距离（使图片中心Y对齐指针Y）
        float deltaY = pointerY - imgScreenCenterY;
        float targetY = content.anchoredPosition.y + deltaY;
        // 移动照片
        StartCoroutine(SmoothScrollTo(targetY + 115));
    }

    private System.Collections.IEnumerator SmoothScrollTo(float targetY)
    {
        float currentY = content.anchoredPosition.y;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * smoothSpeed;
            content.anchoredPosition = new Vector2(
                content.anchoredPosition.x, 
                Mathf.Lerp(currentY, targetY, t)
            );
            yield return null;
        }
    }
}