// 技能基类，继承自 ScriptableObject
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : ScriptableObject
{
    // 技能名字
    private string skillName;
    // 技能描述
    private string skillDetail;
    // 技能消耗
    private List<Color> costs = new() {};
    // 技能类型
    public SkillType skillType;
    // 技能等级
    public int skillLevel; 

    // 技能名字
    public virtual string SkillName
    {
        get
        {
            return skillName;
        }
    }

    // 技能描述
    public virtual string SkillDetail
    {
        get
        {
            return skillDetail;
        }
    }

    // 技能消耗
    public virtual List<Color> Costs
    {
        get
        {
            return costs;
        }
    }

    // 初始化方法
     public abstract void Setup(Hero hero = null, Enemy enemy = null);

    // 使用方法
    public abstract void Use(Hero hero, Enemy target = null);
}

public enum SkillType
{
    // 主动技能
    Active,
    // 被动技能
    Passive
}