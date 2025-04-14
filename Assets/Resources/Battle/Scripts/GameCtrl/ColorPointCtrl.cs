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

    // 实例化颜色点数
    public GameObject GetColorGameObject(Transform attacker, int row)
    {
        if (row == 0) 
        {
            // 创建一个新的蓝色圆球
            return Instantiate(bluePointPrefab, attacker.position, Quaternion.identity, pointArea.parent);
        }
        else if (row == 1)
        {
            // 创建一个新的黄色圆球
            return Instantiate(yellowPointPrefab, attacker.position, Quaternion.identity, pointArea.parent);
        }
        else if (row == 2)
        {
            // 创建一个新的红色圆球
            return Instantiate(redPointPrefab, attacker.position, Quaternion.identity, pointArea.parent);
        }
        else
        {
            // 创建一个新的红色圆球
            return Instantiate(redPointPrefab, attacker.position, Quaternion.identity, pointArea.parent);            
        }
    }

    // 获得颜色点数
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
            Debug.Log(points[0]);
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

    // 新增移除颜色点数的方法
    public void RemoveColorPoint(GameObject pointToRemove)
    {
        if (points.Contains(pointToRemove))
        {
            points.Remove(pointToRemove);

            // 移除动画：让点数消失并缩小
            pointToRemove.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() =>
            {
                Destroy(pointToRemove);
                // 重新排列剩余的点数
                RearrangePoints();
            });
        }
    }

    // 根据多个颜色移除点数的方法
    public bool RemoveColorPointsByColors(Color[] colors)
    {
        // 统计每种颜色需要移除的数量
        Dictionary<Color, int> colorCountToRemove = new Dictionary<Color, int>();
        foreach (Color color in colors)
        {
            if (colorCountToRemove.TryGetValue(color, out int count))
            {
                colorCountToRemove[color] = count + 1;
            }
            else
            {
                colorCountToRemove[color] = 1;
            }
        }
        // 用于存储需要移除的点数
        List<GameObject> pointsToRemove = new List<GameObject>();
        // 用于记录每种颜色已匹配到的点数
        Dictionary<Color, int> matchedColorCount = new Dictionary<Color, int>();
        // 提前计算需要移除的总点数
        int totalPointsToRemove = 0;
        foreach (int count in colorCountToRemove.Values)
        {
            totalPointsToRemove += count;
        }
        // 遍历点数列表
        foreach (GameObject point in points)
        {
            Image pointImage = point.GetComponent<Image>();
            if (pointImage != null)
            {
                Color pointColor = pointImage.color;
                if (colorCountToRemove.TryGetValue(pointColor, out int requiredCount))
                {
                    if (!matchedColorCount.TryGetValue(pointColor, out int currentCount))
                    {
                        currentCount = 0;
                    }
                    if (currentCount < requiredCount)
                    {
                        pointsToRemove.Add(point);
                        matchedColorCount[pointColor] = currentCount + 1;
                        // 检查是否已找到所有需要移除的点数
                        if (pointsToRemove.Count == totalPointsToRemove)
                        {
                            break;
                        }
                    }
                }
            }
        }
        // 检查是否所有颜色的点数都足够
        foreach (KeyValuePair<Color, int> kvp in colorCountToRemove)
        {
            if (!matchedColorCount.TryGetValue(kvp.Key, out int actualCount) || actualCount < kvp.Value)
            {
                return false; // 有指定颜色的点数不足，不进行移除操作
            }
        }
        // 移除所有匹配的点数
        foreach (GameObject point in pointsToRemove)
        {
            RemoveColorPoint(point);
        }
        return true;
    }

    // 比较两个颜色是否相等（考虑浮点数误差）
    private bool AreColorsEqual(Color color1, Color color2)
    {
        const float tolerance = 0.001f;
        return Mathf.Abs(color1.r - color2.r) < tolerance &&
               Mathf.Abs(color1.g - color2.g) < tolerance &&
               Mathf.Abs(color1.b - color2.b) < tolerance &&
               Mathf.Abs(color1.a - color2.a) < tolerance;
    }

}
