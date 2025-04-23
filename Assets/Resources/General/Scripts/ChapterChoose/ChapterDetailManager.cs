
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ChapterDetailManager : MonoBehaviour
{
    public static ChapterDetailManager Get = null;

    public GameObject storyDetailField;

    // 获取组件
    public TextMeshProUGUI chapterName;
    public TextMeshProUGUI chapterContent;

    // 获取组件控制透明度
    private CanvasGroup canvasGroup;

    public bool isMoveComplete = true;


    private void Awake() {
        Get = this;

        // 获取组件
        canvasGroup = storyDetailField.GetComponent<CanvasGroup>();
    }

    // 动画
    public void ShowStoryDetail(string chapterName, string chapterDetail)
    {
        if (!isMoveComplete) return;
        isMoveComplete = false;
        Sequence sequence = DOTween.Sequence();
        float x = canvasGroup.transform.position.x;
        sequence.Append(canvasGroup.DOFade(0, 0.2f));
        sequence.Join(canvasGroup.transform.DOMoveX(x + 50, 0.2f));
        sequence.Append(canvasGroup.transform.DOMoveX(x - 100, 0.1f));
        // 进行文字和图片变更
        sequence.JoinCallback(() =>{
            this.chapterName.text = chapterName;
            chapterContent.text = chapterDetail;
        });
        sequence.Append(canvasGroup.DOFade(1, 0.2f));
        sequence.Join(canvasGroup.transform.DOMoveX(x, 0.2f));
        sequence.OnComplete(() =>{
            isMoveComplete = true;
        });
    }
}
