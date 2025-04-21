using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextListManager : MonoBehaviour
{

    public static TextListManager Get = null;

    [Header("配置")]
    public TMP_FontAsset defaultFont; // 默认字体
    public GameObject listEntryPrefab; // 列表条目预制体
    public Transform listContainer; // 列表容器（需挂载Vertical Layout Group和Content Size Fitter）
    public int maxEntries = 5; // 最大显示条目数

    private Queue<TextListEntry> activeEntries = new Queue<TextListEntry>();

    private void Awake() {
        Get = this;
    }

    /// <summary>
    /// 添加台词到列表
    /// </summary>
    public void AddLine(string characterName, string lineText, Color textColor)
    {
        HideOldestEntry();
        // 创建条目
        TextListEntry entry = GetOrCreateEntry();
        
        // 组合显示文本（角色名+台词）
        string displayText = $"{characterName}: {lineText}";
        entry.SetLine(displayText, textColor);
        entry.Show();
    }

    private TextListEntry GetOrCreateEntry()
    {   
        // 重用已隐藏的条目
        if (activeEntries.Count > 0)
        {
            TextListEntry entry = activeEntries.Dequeue();
            entry.gameObject.SetActive(true);
            return entry;
        }

        // 创建新条目
        GameObject newEntry = Instantiate(listEntryPrefab, listContainer);
        TextListEntry component = newEntry.GetComponent<TextListEntry>();
        return component;
    }

    /// <summary>
    /// 隐藏旧条目（当超过最大数量时）
    /// </summary>
    private void HideOldestEntry()
    {
        if (listContainer.childCount == maxEntries)
        {
            Transform oldestChild = listContainer.GetChild(0);
            TextListEntry entry = oldestChild.GetComponent<TextListEntry>();
            entry.Hide();
            oldestChild.SetAsLastSibling(); // 移到末尾以便重用
            activeEntries.Enqueue(entry);
        }
    }
}
