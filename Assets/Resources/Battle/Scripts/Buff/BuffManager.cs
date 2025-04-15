using System;
using System.Collections.Generic;
using UnityEngine;

// 管理角色 BUFF 的类
public class BuffManager
{
    private BasicCharacter character; // 关联的角色实例
    private Dictionary<string, Buff> activeBuffs = new Dictionary<string, Buff>(); // 当前生效的 BUFF

    // BUFF 类型注册表（名称到类型的映射）
    private static Dictionary<string, Type> buffTypeRegistry = new Dictionary<string, Type>
    {
        { "自愈", typeof(HealBuff) },
        { "中毒", typeof(PoisonBuff) }
    };

    public BuffManager(BasicCharacter character)
    {
        this.character = character;
        // 初始化时注册所有支持的 BUFF 类型（可通过反射自动扫描优化）
    }

    /// <summary>
    /// 添加 BUFF（通用接口）
    /// </summary>
    /// <param name="buffName">BUFF 名称</param>
    /// <param name="args">构造参数（按顺序传递，如持续时间、数值加成等）</param>
    public void AddBuff(string buffName, params object[] args)
    {
        if (!buffTypeRegistry.ContainsKey(buffName))
        {
            Debug.LogWarning($"未注册的 BUFF：{buffName}");
            return;
        }

        Type buffType = buffTypeRegistry[buffName];
        Buff newBuff = CreateBuffInstance(buffType, args);

        if (newBuff == null) return;

        // 处理已存在的同类型 BUFF（叠加或刷新）
        if (activeBuffs.TryGetValue(buffName, out Buff existingBuff))
        {
            existingBuff.Refresh(newBuff); // 刷新持续时间或叠加层数
        }
        else
        {
            activeBuffs.Add(buffName, newBuff);
            newBuff.Apply(character); // 应用 BUFF 效果
            StartBuffTimer(newBuff); // 启动持续时间计时器
        }
    }

    // 移除指定 BUFF
    public void RemoveBuff(string buffName)
    {
        if (activeBuffs.TryGetValue(buffName, out Buff buff))
        {
            buff.Remove(character); // 移除效果
            activeBuffs.Remove(buffName);
        }
    }

    // 创建 BUFF 实例
    private Buff CreateBuffInstance(Type type, object[] args)
    {
        try
        {
            // 无参数构造
            if (args.Length == 0)
            {
                return Activator.CreateInstance(type) as Buff;
            }
            // 单参数构造（持续时间）
            else if (args.Length == 1 && args[0] is int durationA)
            {
                return Activator.CreateInstance(type, durationA) as Buff;
            }
            // 双参数构造（数值加成 + 持续时间）
            else if (args.Length == 2 && args[0] is int value && args[1] is int durationB)
            {
                return Activator.CreateInstance(type, value, durationB) as Buff;
            }
            else
            {
                Debug.LogError($"不支持的参数格式：{string.Join(",", args)}");
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"创建 BUFF 失败：{e.Message}");
            return null;
        }
    }

    // 启动 BUFF 持续时间计时器
    private void StartBuffTimer(Buff buff)
    {
        if (buff.duration > 0)
        {
            // 使用协程处理持续时间（需在 Character 中调用 StartCoroutine）
            character.StartCoroutine(WaitForBuffDuration(buff));
        }
    }

    // 角色回合结束时调用（处理回合生效的BUFF）
    public void OnCharacterRoundEnd()
    {
        List<string> buffsToCheck = new List<string>(activeBuffs.Keys);
        foreach (string buffName in buffsToCheck)
        {
            Buff buff = activeBuffs[buffName];
            buff.OnRoundEnd(character); // 触发回合结束逻辑
            if (buff.stackLayers <= 0)
            {
                RemoveBuff(buffName); // 层数为0时移除
            }
        }
    }

    private System.Collections.IEnumerator WaitForBuffDuration(Buff buff)
    {
        yield return new WaitForSeconds(buff.duration);
        RemoveBuff(buff.buffName); // 持续时间结束后自动移除
    }
}