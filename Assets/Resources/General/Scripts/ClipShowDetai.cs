using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ClipShowDetai : MonoBehaviour
{
    private Button button;
    private GameObject detailPanel;
    private void Start()
    {
        button = GetComponent<Button>();
        detailPanel = GameObject.Find("DetailPanel");
        button.onClick.AddListener(OnButtonClick);
        Debug.Log(detailPanel);
    }

    private void OnButtonClick()
    {
        // 展示detail
        if (detailPanel.GetComponent<CanvasGroup>().alpha == 0)
        {
            detailPanel.GetComponent<CanvasGroup>().DOFade(1, 0.4f);
        }

        // 按钮选择器
        if (ButtonSelectCore.Selection == button)
        {
            ButtonSelectCore.DropSelect();
            detailPanel.GetComponent<CanvasGroup>().DOFade(0, 0.4f);
        }
        else if (ButtonSelectCore.Selection != button)
        {
            ButtonSelectCore.TrySelect(button);
        }

    }
}
