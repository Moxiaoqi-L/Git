using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EnhancedRandomBackground : MonoBehaviour {
    public Image backgroundImage; // 全屏背景图片
    public Image characterImage; // 人物
    public float maxOffset = 80f; // 最大偏移量（像素，建议 ≤1/50 屏幕宽度）
    public float minAlpha = 0.2f; // 最小透明度
    public float maxDuration = 1.5f; // 最长动画时长
    public float minDuration = 1.0f; // 最短动画时长

    private RectTransform backgroundRect;
    private RectTransform secondRect; // 第二张图片的RectTransform
    private Vector2 centerPosition; // 屏幕中心位置
    private Vector2 characterCenterPosition; // 人物中心位置
    private float originalAlpha; // 初始透明度（默认为1）

    private Tween _bounceTween;  

    void Start() {
        if (backgroundImage == null) {
            Debug.LogError("请为背景图片赋值！");
            return;
        }
        backgroundRect = backgroundImage.rectTransform;
        secondRect = characterImage.rectTransform;
        centerPosition = backgroundRect.anchoredPosition;
        characterCenterPosition = secondRect.anchoredPosition + new Vector2(0, 800);
        originalAlpha = backgroundImage.color.a;
        AdaptToScreen(); // 确保背景图全屏
        StartEnhancedAnimation();
    }

    void StartEnhancedAnimation() {
        // 生成相同的随机参数（确保两张图动画同步变化）
        float randomX = GaussianRandom(0, maxOffset / 3);
        float randomY = GaussianRandom(0, maxOffset / 3);
        Vector2 targetPos = centerPosition + new Vector2(randomX, randomY);
        Vector2 targetPos2 = characterCenterPosition + new Vector2(randomX, randomY);
        float duration = Random.Range(minDuration, maxDuration);
        Ease randomEase = GetRandomEase();
        float targetAlpha = Random.Range(minAlpha, 1f);

        // 复合动画序列：同时控制两张图片
        _bounceTween = DOTween.Sequence()
            // 第一张图动画
            .Append(backgroundRect.DOAnchorPos(targetPos, duration).SetEase(randomEase))
            .Join(backgroundImage.DOFade(targetAlpha, duration * 0.6f).SetEase(Ease.OutSine))
            // 第二张图执行完全相同的动画
            .Join(secondRect.DOAnchorPos(-targetPos2, duration).SetEase(randomEase))
            .OnComplete(StartEnhancedAnimation); // 循环触发
    }

    // 高斯分布随机值（均值mean，标准差stdDev）
    float GaussianRandom(float mean, float stdDev) {
        float u1 = 1 - Random.value; // 避免0值
        float u2 = 1 - Random.value;
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
        return mean + randStdNormal * stdDev;
    }

    // 获取随机缓动函数（增加更多曲线）
    Ease GetRandomEase() {
        int index = Random.Range(0, 5);
        switch (index) {
            case 0: return Ease.InOutSine;
            case 1: return Ease.OutQuad;
            case 2: return Ease.InCubic;
            case 3: return Ease.OutQuint;
            case 4: return Ease.InOutExpo;
            default: return Ease.Linear;
        }
    }

    // 适配屏幕尺寸（确保全屏）
    void AdaptToScreen() {
        backgroundRect.sizeDelta += new Vector2(144, 81);
    }

    private void OnDestroy() {
        _bounceTween.Kill();
        _bounceTween = null;
    }
}