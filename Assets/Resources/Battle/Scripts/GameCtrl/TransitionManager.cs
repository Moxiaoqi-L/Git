using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    public Image transitionImage; // 转场用的UI元素，如黑色遮罩
    public float transitionDuration = 1f; // 转场动画持续时间

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 开始转场动画
    public void StartTransition(System.Action onTransitionComplete)
    {
        // 显示转场UI
        transitionImage.gameObject.SetActive(true);
        transitionImage.color = new Color(0, 0, 0, 0);

        // 淡入效果
        transitionImage.DOFade(1, transitionDuration / 2).OnComplete(() =>
        {
            // 淡入完成后，执行回调函数，通常是加载新的轮次内容
            onTransitionComplete?.Invoke();

            // 淡出效果
            transitionImage.DOFade(0, transitionDuration / 2).OnComplete(() =>
            {
                // 转场结束，隐藏转场UI
                transitionImage.gameObject.SetActive(false);
            });
        });
    }
}