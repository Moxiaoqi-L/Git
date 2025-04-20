using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        newPoint.transform.DOMove(pointArea.position, 0.4f).OnComplete(() =>
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
        float pointWidth = redPointPrefab.GetComponent<RectTransform>().sizeDelta.x + 40;
        float totalWidth = pointWidth * points.Count;
        float startX = -totalWidth / 2f + pointWidth / 2f;

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 targetPosition = new(startX + i * pointWidth, 0, 0);
            points[i].transform.DOLocalMove(targetPosition, 0.3f);
        }
    }

    // 移除颜色点数的方法
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

    // 根据颜色移除颜色
    public bool RemoveColorPointsByColors(List<Color> targetColors)
    {
        // 统计每种目标颜色需要移除的数量（原逻辑）
        Dictionary<Color, int> colorCountToRemove = new Dictionary<Color, int>();
        foreach (Color color in targetColors)
        {
            colorCountToRemove[color] = colorCountToRemove.GetValueOrDefault(color, 0) + 1;
        }

        List<GameObject> pointsToRemove = new List<GameObject>();
        Dictionary<Color, int> matchedColorCount = new Dictionary<Color, int>(); // 记录目标颜色已匹配的数量
        int totalPointsToRemove = colorCountToRemove.Values.Sum();

        foreach (GameObject point in points)
        {
            Image pointImage = point.GetComponent<Image>();
            if (pointImage == null) continue;
            Color pointColor = pointImage.color;

            // 遍历所有目标颜色，检查是否在容差范围内相等
            foreach (Color targetColor in colorCountToRemove.Keys)
            {
                if (AreColorsEqual(pointColor, targetColor))
                {
                    int requiredCount = colorCountToRemove[targetColor];
                    int currentMatched = matchedColorCount.GetValueOrDefault(targetColor, 0);

                    if (currentMatched < requiredCount)
                    {
                        pointsToRemove.Add(point);
                        matchedColorCount[targetColor] = currentMatched + 1;
                        // 达到总移除数量时提前终止
                        if (pointsToRemove.Count == totalPointsToRemove) break;
                    }
                }
            }

            if (pointsToRemove.Count == totalPointsToRemove) break; // 提前退出循环
        }

        // 检查所有目标颜色是否满足数量要求
        foreach (var kvp in colorCountToRemove)
        {
            if (matchedColorCount.GetValueOrDefault(kvp.Key, 0) < kvp.Value)
            {
                return false;
            }
        }

        // 执行移除操作（与原逻辑一致）
        foreach (GameObject point in pointsToRemove)
        {
            RemoveColorPoint(point);
        }
        return true;
    }

    // 比较两个颜色是否相等（考虑浮点数误差）
    private bool AreColorsEqual(Color color1, Color color2)
    {
        const float tolerance = 0.02f;
        return Mathf.Abs(color1.r - color2.r) < tolerance &&
               Mathf.Abs(color1.g - color2.g) < tolerance &&
               Mathf.Abs(color1.b - color2.b) < tolerance &&
               Mathf.Abs(color1.a - color2.a) < tolerance;
    }

}
