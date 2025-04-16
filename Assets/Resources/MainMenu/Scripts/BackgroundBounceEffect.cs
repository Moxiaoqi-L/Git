using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EnhancedRandomBackground : MonoBehaviour {
    public Image backgroundImage; // 全屏背景图片
    public float maxOffset = 20f; // 最大偏移量（像素，建议 ≤1/50 屏幕宽度）
    public float minAlpha = 0.9f; // 最小透明度
    public float maxDuration = 1.5f; // 最长动画时长
    public float minDuration = 1.0f; // 最短动画时长

    private RectTransform backgroundRect;
    private Vector2 centerPosition; // 屏幕中心位置
    private float originalAlpha; // 初始透明度（默认为1）

    void Start() {
        if (backgroundImage == null) {
            Debug.LogError("请为背景图片赋值！");
            return;
        }
        backgroundRect = backgroundImage.rectTransform;
        centerPosition = backgroundRect.anchoredPosition;
        originalAlpha = backgroundImage.color.a;
        AdaptToScreen(); // 确保背景图全屏
        StartEnhancedAnimation();
    }

    void StartEnhancedAnimation() {
        // 生成高斯分布的随机偏移
        float randomX = GaussianRandom(0, maxOffset / 3); // 标准差为maxOffset的1/3
        float randomY = GaussianRandom(0, maxOffset / 3);
        Vector2 targetPos = centerPosition + new Vector2(randomX, randomY);

        // 随机动画时长（带0.2秒波动）
        float duration = Random.Range(minDuration, maxDuration);

        // 复合动画序列：移动+旋转+透明度
        DOTween.Sequence()
            .Append(backgroundRect.DOAnchorPos(targetPos, duration)
                .SetEase(GetRandomEase())) // 随机缓动
            .Join(backgroundImage.DOFade(
                Random.Range(minAlpha, 1f), duration * 0.6f)
                .SetEase(Ease.OutSine)) // 透明度波动
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
}