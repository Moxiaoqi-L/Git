using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheesemanShowDetail : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject detailUI;
    public GameObject buffDetail;

    private Hero hero;
    private Enemy enemy;

    private void Start()
    {
        detailUI = transform.GetChild(2).gameObject;
        detailUI.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f);
        detailUI.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.5f);
        detailUI.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 400);
        detailUI.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
        detailUI.GetComponent<RectTransform>().localPosition = new Vector2(100, 0);
        detailUI.GetComponent<Canvas>().overrideSorting = true;
        hero = GetComponent<Hero>();
        enemy = GetComponent<Enemy>();
    }
    
    // 鼠标放上来时实例化
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hero != null)
        {
            int i = 0;
            foreach (var buff in hero.buffManager.activeBuffs)
            {
                GameObject stat = Instantiate(buffDetail, detailUI.transform);
                stat.transform.localPosition += new Vector3(0, -i * 100, 0);
                stat.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = buff.Value.buffName;
                stat.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = buff.Value.stackLayers.ToString();
                stat.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = buff.Value.buffDetail;
                i++;
            }
        }
        if (enemy != null)
        {
            int i = 0;
            foreach (var buff in enemy.buffManager.activeBuffs)
            {
                GameObject stat = Instantiate(buffDetail, detailUI.transform);
                stat.transform.localPosition += new Vector3(0, -i * 100, 0);
                stat.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = buff.Value.buffName;
                stat.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = buff.Value.stackLayers.ToString();
                stat.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = buff.Value.buffDetail;
                i++;
            }     
        }
    }
    
    // 摧毁所有子元素
    public void OnPointerExit(PointerEventData eventData)
    {
        for (int i = 0; i < detailUI.transform.childCount; i++) {  
            Destroy(detailUI.transform.GetChild(i).gameObject);  
        }  
    }
}