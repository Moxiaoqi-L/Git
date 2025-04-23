using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public static class LineParser
{
    public static List<CharacterLineData> LoadLines(string characterName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>($"Battle/Lines/{characterName}");
        if (jsonFile == null) return null;
        LinesData data = JsonUtility.FromJson<LinesData>(jsonFile.text);
        return data.lines;
    }
}

[Serializable]
public class CharacterLineData
{
    public string eventType; // 事件类型
    public string lineText; // 台词文本
    public string textColor; // 颜色（如"#FF0000"）
}

[Serializable]
public class LinesData
{
    public string characterName; // 角色名
    public List<CharacterLineData> lines = new List<CharacterLineData>();
}

public enum LineEventType
{
    Attack,       // 攻击事件
    Defense,      // 受击/防御事件
    Death,      // 死亡事件
    SkillActive,  // 主动技能释放
    SkillPassive,  // 被动技能触发
    SkillUltimate  // 大招事件
}