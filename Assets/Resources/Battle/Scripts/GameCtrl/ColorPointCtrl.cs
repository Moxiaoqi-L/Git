using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ColorPointCtrl : MonoBehaviour
{
    public static ColorPointCtrl Get = null;
    public Transform pointArea; // 点数存放区域
    public GameObject redPointPrefab; // 红色圆球预制体
    public GameObject yellowPointPrefab; // 黄圆球预制体
    public GameObject bluePointPrefab; // 蓝色圆球预制体
    public int maxPoints = 4; // 区域最多存放的点数

    private static List<GameObject> points = new List<GameObject>();

    private void Awake()
    {
            Get = this;
    }

    public GameObject GetColorGameObject(Transform attacker, int row)
    {
        if (row == 0) 
        {
            // 创建一个新的蓝色圆球
            return Instantiate(bluePointPrefab, attacker.position, Quaternion.identity, pointArea.parent);
        }else if (row == 1)
        {
            // 创建一个新的黄色圆球
            return Instantiate(yellowPointPrefab, attacker.position, Quaternion.identity, pointArea.parent);
        }else if (row == 2)
        {
            // 创建一个新的红色圆球
            return Instantiate(redPointPrefab, attacker.position, Quaternion.identity, pointArea.parent);
        }else
        {
            // 创建一个新的红色圆球
            return Instantiate(redPointPrefab, attacker.position, Quaternion.identity, pointArea.parent);            
        }
    }

    public void GetColorPoint(Transform attacker, int row)
    {

        GameObject newPoint = GetColorGameObject(attacker, row);
        // 让圆球飞向点数区域
        newPoint.transform.DOMove(pointArea.position, 0.5f).OnComplete(() =>
        {
            // 将红色圆球移动到点数区域内
            newPoint.transform.SetParent(pointArea);
            newPoint.transform.localPosition = Vector3.zero;

            // 将新的点数添加到列表中
            points.Add(newPoint);

            // 检查点数是否超过最大限制
            if (points.Count > maxPoints)
            {
                // 移除最左侧的点数
                GameObject firstPoint = points[0];
                points.RemoveAt(0);
                Destroy(firstPoint);
            }

            // 重新排列点数
            RearrangePoints();
        });
    }

    private void RearrangePoints()
    {
        float pointWidth = redPointPrefab.GetComponent<RectTransform>().sizeDelta.x + 20;
        float totalWidth = pointWidth * points.Count;
        float startX = -totalWidth / 2f + pointWidth / 2f;

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 targetPosition = new(startX + i * pointWidth, 0, 0);
            points[i].transform.DOLocalMove(targetPosition, 0.5f);
        }
    }

}
