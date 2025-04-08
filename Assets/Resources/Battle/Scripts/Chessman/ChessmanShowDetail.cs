using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheesemanShowDetail : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float yOffset = 10f;//y轴偏移量
    public float xOffset = 10f;//x轴偏移量
    RectTransform rectTransform;
    Canvas canvas;
    GameObject detailUI;

    string cheeseDetail="null";//棋子详细信息的文本
    int cheeseHitPoint;//血量
    public Chessman chessman;//当前物体挂载的chess脚本
    private void Start()
    {
        chessman = GetComponent<Chessman>();
        canvas= FindObjectOfType<Canvas>();
        rectTransform=GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)//鼠标放上来时实例化棋子介绍UI
    {
        if (chessman != null && chessman.camp == Camp.Player)//若当前选中棋子不为空，就去获取信息
        {
            cheeseHitPoint = chessman.hero.currentHealthPoints;//获取所选棋子的血量
            cheeseDetail = "血量： " + cheeseHitPoint.ToString();
        }
        detailUI = (GameObject)GameObject.Instantiate(Resources.Load("Battle/Prefab/ChessmanDetailUI"));
        detailUI.transform.SetParent(canvas.transform, false);
        RectTransform rectTransform_new = detailUI.GetComponent<RectTransform>();
        rectTransform_new.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x+xOffset, rectTransform.anchoredPosition.y+yOffset);
        TextMeshProUGUI text = detailUI.GetComponent<TextMeshProUGUI>();
        text.text = cheeseDetail;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(detailUI);
    }
}