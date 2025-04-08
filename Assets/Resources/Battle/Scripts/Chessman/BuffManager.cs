using System.Collections.Generic;
using UnityEngine;

// 管理英雄 BUFF 的类
public class BuffManager
{
    // 存储攻击类 BUFF 的字典，键为 BUFF 名称，值为包含 BUFF 数值和剩余回合数的结构体
    private Dictionary<string, BuffInfo> attackBuffs = new Dictionary<string, BuffInfo>();
    // 存储防御类 BUFF 的字典，键为 BUFF 名称，值为包含 BUFF 数值和剩余回合数的结构体
    private Dictionary<string, BuffInfo> defenseBuffs = new Dictionary<string, BuffInfo>();
    // 存储特殊 BUFF 的字典，键为 BUFF 名称，值为包含 BUFF 数值和剩余回合数的结构体
    private Dictionary<string, BuffInfo> specialBuffs = new Dictionary<string, BuffInfo>();
    // 存储从文件加载的所有 BUFF 数据
    private BuffList allBuffs;

    // 从 Resources 文件夹下的 Buffs.json 文件中加载 BUFF 数据
    public void LoadBuffsFromFile()
    {
        // 加载 Resources 文件夹下的 Buffs.json 文件
        TextAsset jsonAsset = Resources.Load<TextAsset>("Battle/Jsons/Buffs");
        if (jsonAsset != null)
        {
            // 将 JSON 数据解析为 BuffList 对象
            allBuffs = JsonUtility.FromJson<BuffList>(jsonAsset.text);
        }
    }

    // 根据 BUFF 名称添加对应的 BUFF，并指定持续回合数
    public void AddBuff(string buffName, int duration)
    {
        if (allBuffs != null)
        {
            // 遍历所有加载的 BUFF 数据
            foreach (BuffData buff in allBuffs.buffs)
            {
                if (buff.name == buffName)
                {
                    BuffInfo buffInfo = new BuffInfo { value = buff.value, remainingRounds = duration };
                    if (buff.type == "attack")
                    {
                        // 如果是攻击类 BUFF，添加到攻击类 BUFF 字典中
                        attackBuffs[buffName] = buffInfo;
                    }
                    else if (buff.type == "defense")
                    {
                        // 如果是防御类 BUFF，添加到防御类 BUFF 字典中
                        defenseBuffs[buffName] = buffInfo;
                    }
                    else if (buff.type == "special")
                    {
                        // 如果是特殊 BUFF，添加到特殊 BUFF 字典中
                        specialBuffs[buffName] = buffInfo;
                    }
                    Debug.Log($"Hero gained buff {buffName} for {duration} rounds.");
                    break;
                }
            }
        }
    }

    // 获取所有攻击类 BUFF 的总数值
    public float GetTotalAttackBuff()
    {
        float total = 0;
        // 遍历攻击类 BUFF 字典，累加所有 BUFF 数值
        foreach (var buff in attackBuffs.Values)
        {
            total += buff.value;
        }
        return total;
    }

    // 获取所有防御类 BUFF 的总数值
    public float GetTotalDefenseBuff()
    {
        float total = 0;
        // 遍历防御类 BUFF 字典，累加所有 BUFF 数值
        foreach (var buff in defenseBuffs.Values)
        {
            total += buff.value;
        }
        return total;
    }

    // 检查英雄是否拥有指定的特殊 BUFF
    public bool HasSpecialBuff(string buffName)
    {
        return specialBuffs.ContainsKey(buffName);
    }

    // 每回合结束时调用，减少所有 BUFF 的剩余回合数，并移除到期的 BUFF
    public void EndOfRound()
    {
        RemoveExpiredBuffs(attackBuffs);
        RemoveExpiredBuffs(defenseBuffs);
        RemoveExpiredBuffs(specialBuffs);
    }

    // 移除到期的 BUFF
    private void RemoveExpiredBuffs(Dictionary<string, BuffInfo> buffs)
    {
        var keysToRemove = new List<string>();
        foreach (var kvp in buffs)
        {
            var kvpv = kvp.Value;
            kvpv.remainingRounds--;
            if (kvp.Value.remainingRounds <= 0)
            {
                keysToRemove.Add(kvp.Key);
                Debug.Log($"Hero lost buff {kvp.Key}.");
            }
        }
        foreach (var key in keysToRemove)
        {
            buffs.Remove(key);
        }
    }
    
    // 移除指定的 BUFF
    public void RemoveBuff(string buffName)
    {
        if (attackBuffs.Remove(buffName))
        {
            Debug.Log($"Hero lost attack buff {buffName}.");
        }
        else if (defenseBuffs.Remove(buffName))
        {
            Debug.Log($"Hero lost defense buff {buffName}.");
        }
        else if (specialBuffs.Remove(buffName))
        {
            Debug.Log($"Hero lost special buff {buffName}.");
        }
    }

    // 移除所有的 BUFF
    public void RemoveAllBuffs()
    {
        attackBuffs.Clear();
        defenseBuffs.Clear();
        specialBuffs.Clear();
        Debug.Log("Hero lost all buffs.");
    }
}



// 表示单个 BUFF 信息的结构体，包含 BUFF 数值和剩余回合数
public struct BuffInfo
{
    public float value;
    public int remainingRounds;
}

// 表示单个 BUFF 数据的类
[System.Serializable]
public class BuffData
{
    // BUFF 名称
    public string name;
    // BUFF 类型，"attack" 表示攻击类，"defense" 表示防御类，"special" 表示特殊类
    public string type;
    // BUFF 数值
    public float value;
}

// 存储所有 BUFF 数据的列表类
[System.Serializable]
public class BuffList
{
    // BUFF 数据数组
    public BuffData[] buffs;
}    