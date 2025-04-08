using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fllow : MonoBehaviour
{
    public Transform ObjectFllow;

    // Update is called once per frame
    void Update()
    {
        Vector2 realposition= Camera.main.WorldToScreenPoint(ObjectFllow.position);//世界坐标转化
        GetComponent<RectTransform>().position = realposition;
    }
}
