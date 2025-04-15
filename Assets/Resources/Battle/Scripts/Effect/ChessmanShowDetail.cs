using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheesemanShowDetail : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject detailUI;
    public GameObject buffDetail;

    private Hero hero;
    private Enemy enemy;

    private void Start()
    {
        hero = GetComponent<Hero>();
        enemy = GetComponent<Enemy>();
    }
    
    // 鼠标放上来时实例化
    public void OnPointerEnter(PointerEventData eventData)
    {

    }
    
    // 摧毁所有子元素
    public void OnPointerExit(PointerEventData eventData)
    {
        for (int i = 0; i < detailUI.transform.childCount; i++) {  
            Destroy(detailUI.transform.GetChild(i).gameObject);  
        }  
    }
}