using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class ImageScrollAligner : MonoBehaviour
{
    [Header("组件引用")]
    // public ScrollRect scrollRect;
    public RectTransform content;
    public RectTransform pointer;
    public float smoothSpeed = 3f;

    // 图片插入的位置
    public GameObject contentField;
    // 章节预制体
    public GameObject chapterImagePrefab;
    // 进入章节按钮
    public LoadScene loadScene;

    // 音效缓存
    public AudioClip chapterChange;

    // 只需要在场景加载时执行
    private void Awake() {
        TextAsset jsonFile = Resources.Load<TextAsset>($"General/ChapterJson/ChapterInfo");
        if (jsonFile == null) Debug.LogWarning("卧槽！章节文件出错了！你在干什么");
        ChapterData chapterData = JsonUtility.FromJson<ChapterData>(jsonFile.text);
        foreach (Chapter chapter in chapterData.info)
        {
            GameObject imageButton = Instantiate(chapterImagePrefab, contentField.transform);
            imageButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("General/Image/" + chapter.chapterImage);
            Button button = imageButton.GetComponent<Button>();
            button.onClick.AddListener(() => {
                loadScene.sceneName = chapter.chapterName;
                OnImageClick(imageButton.GetComponent<RectTransform>(), chapter.chapterName, chapter.chapterDetail);
            });
        }
    }

    private void Start()
    {
        // 延迟一帧确保布局组件初始化完成
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        // 初始指向第一张照片
        StartCoroutine(SmoothScrollTo(-275));
    }

    private void OnImageClick(RectTransform clickedImage, string chapterName, string chapterDetail)
    {
        if (!ChapterDetailManager.Get.isMoveComplete) return;
        // 计算指针在屏幕空间的Y坐标（假设指针垂直对齐，X固定）
        float pointerY = pointer.position.y;
        // 计算图片中心在屏幕空间的Y坐标
        float imgScreenCenterY = clickedImage.position.y + clickedImage.rect.height * 0.5f;
        // 计算content需要移动的Y距离（使图片中心Y对齐指针Y）
        float deltaY = pointerY - imgScreenCenterY;
        float targetY = content.anchoredPosition.y + deltaY;
        // 移动照片
        StartCoroutine(SmoothScrollTo(targetY + 115));
        // 展示详细
        ChapterDetailManager.Get.ShowStoryDetail(chapterName, chapterDetail);
        // 音效处理
        AudioManager.Get.PlaySound(chapterChange);
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

[Serializable]
public class ChapterData
{
    public List<Chapter> info; // 章节细节
}

[Serializable]
public class Chapter
{
    public string chapterName; // 章节名称
    public string chapterType; // 章节类型
    public string chapterDetail; // 章节详情
    public string chapterImage; // 章节图片
    public string chapterDetailImage; // 章节详情图片
}