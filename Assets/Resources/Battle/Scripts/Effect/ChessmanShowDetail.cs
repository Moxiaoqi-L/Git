using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuffDetail : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public GameObject statPrefab;

    private GameObject buffDetail;

    public string title;
    public string content;


    // 鼠标放上来时实例化
    public void OnPointerEnter(PointerEventData eventData)
    {
        buffDetail = Instantiate(statPrefab, transform.parent);
        buffDetail.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = title;
        buffDetail.transform.Find("Content").GetComponent<TextMeshProUGUI>().text = content;
        buffDetail.transform.localPosition = buffDetail.transform.localPosition + new Vector3(0, -500, 0);

    }
    
    // 摧毁所有子元素
    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(buffDetail);
    }
}