using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRegistry : MonoBehaviour
{
    public static SkillRegistry Get;

    private void Awake() {
        Get = this;
        AutoRegisterSkills();
    }

    // Skill 注册表（名称到类型的映射）
    private static Dictionary<string, Type> SkillTypeRegistry = new(){};

    public Dictionary<string, Type> GetSkillTypeRegistry
    {
        get
        {
            return SkillTypeRegistry;
        }
    }

    /// <summary>
    /// 自动扫描程序集并注册所有技能类型
    /// </summary>
    private static void AutoRegisterSkills()
    {
        SkillTypeRegistry.Clear();
        
        // 遍历所有加载的程序集（根据需求可限制特定程序集）
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                // 获取程序集中所有类型
                foreach (var type in assembly.GetTypes())
                {
                    // 筛选条件：继承自Skill类，非抽象类，非接口
                    if (typeof(Skill).IsAssignableFrom(type) && 
                        !type.IsAbstract && 
                        !type.IsInterface)
                    {
                        // 提取技能名称（这里使用类名，可通过特性自定义名称）
                        string skillName = type.Name; 
                        
                        // 注册到字典（可添加重复检查）
                        if (!SkillTypeRegistry.ContainsKey(skillName))
                        {
                            SkillTypeRegistry.Add(skillName, type);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"程序集扫描错误：{assembly.FullName} - {e.Message}");
            }
        }
    }
}
