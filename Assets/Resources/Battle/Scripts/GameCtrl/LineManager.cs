using System;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    public static LineManager Get { get; private set; }
    
    private Dictionary<string, List<CharacterLineData>> characterLines = new();

    private void Awake()
    {
        // 单例初始化
        if (Get == null)
        {
            Get = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 加载所有角色的台词数据（英雄和敌人）
    /// </summary>
    public void LoadAllCharacterLines()
    {
        characterLines.Clear(); // 清空旧数据
        
        // 加载所有英雄台词
        foreach (var hero in Chessman.AllHeros())
        {
            string characterName = hero.characterAttributes.characterName;
            LoadCharacterLines(characterName);
        }

        // 加载所有敌人台词
        foreach (Enemy enemy in Chessman.AllEnemies())
        {
            string characterName = enemy.characterAttributes.characterName;
            LoadCharacterLines(characterName);
        }
    }

    private void LoadCharacterLines(string characterName)
    {
        if (characterLines.ContainsKey(characterName)) return;

        List<CharacterLineData> lines = LineParser.LoadLines(characterName);
        if (lines != null)
        {
            characterLines[characterName] = lines;
        }
        else
        {
            Debug.LogWarning($"未找到角色台词文件：{characterName}");
        }
    }


    // 获取随机台词条目
    public CharacterLineData GetRandomLine(string characterName, LineEventType eventType)
    {
        if (!characterLines.TryGetValue(characterName, out var lines))
        {
            Debug.LogWarning($"角色 {characterName} 没有注册台词数据");
            return null;
        }

        var filteredLines = lines.FindAll(l => l.eventType == eventType.ToString());
        if (filteredLines.Count == 0)
        {
            Debug.LogWarning($"角色 {characterName} 没有 {eventType} 类型的台词");
            return null;
        }

        // 返回随机条目
        return filteredLines[UnityEngine.Random.Range(0, filteredLines.Count)];
    }

}