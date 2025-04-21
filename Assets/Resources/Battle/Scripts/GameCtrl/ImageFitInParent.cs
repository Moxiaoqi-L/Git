using UnityEngine;
using UnityEngine.UI;

public class ImageFitInParent : MonoBehaviour
{
    private Image image;
    private AspectRatioFitter fitter;
    
    public CharacterDetail characterDetail;

    void Start()
    {
        // 获取 Image 组件
        image = GetComponent<Image>();

        // 获取或添加 Aspect Ratio Fitter 组件
        fitter = image.gameObject.GetComponent<AspectRatioFitter>();

        // 设置 Aspect Ratio Fitter 的属性
        UpdateAspectRatio();

        // 可以在这里添加代码来监听图片的实时变化，例如图片替换的事件
        characterDetail.spriteChanged += UpdateAspectRatio;
    }
    // 当图片发生变化时调用此方法更新宽高比
    public void UpdateAspectRatio()
    {
        
        if (image.sprite != null)
        {
            fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            fitter.aspectRatio = (float)image.sprite.texture.width / image.sprite.texture.height;
        }
    }
}    