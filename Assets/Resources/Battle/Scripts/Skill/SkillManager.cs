using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 管理角色 技能 的类
public class SkillManager
{
    public BasicCharacter character; // 关联的角色实例
    public List<Skill> activeSkills = new(); // 主动技能字典
    public List<Skill> passiveSkills = new(); // 被动技能字典

    private Dictionary<string, Type> skillTypeRegistry;

    public SkillManager(BasicCharacter character)
    {
        this.character = character;
        // 初始化时注册所有支持的 BUFF 类型（可通过反射自动扫描）
        skillTypeRegistry = SkillRegistry.Get.GetSkillTypeRegistry;
    }

    /// 添加 技能
    public void AddSkill(string skillName)
    {
        Type skillType = skillTypeRegistry[skillName];
        Skill newSkill = CreateSkillInstance(skillType);

        newSkill.Init(this, character);

        

        if (newSkill.skillType == SkillType.Active) activeSkills.Add(newSkill);
        if (newSkill.skillType == SkillType.Passive) passiveSkills.Add(newSkill);
    }

    // 创建 BUFF 实例
    private Skill CreateSkillInstance(Type skill)
    {
        try
        {
            return Activator.CreateInstance(skill) as Skill;
        }
        catch (Exception e)
        {
            Debug.LogError($"创建 技能 失败：{e.Message}");
            return null;
        }
    }
}