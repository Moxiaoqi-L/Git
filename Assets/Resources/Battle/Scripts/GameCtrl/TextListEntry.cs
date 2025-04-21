using TMPro;
using UnityEngine;

public class TextListEntry : MonoBehaviour
{
    [SerializeField] public TMP_Text textComponent;
    [SerializeField] public CanvasGroup canvasGroup; // 用于控制显示隐藏

    public void SetLine(string text, Color color)
    {
        textComponent.text = text;
        textComponent.color = color;
    }

    public void Show()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
}
