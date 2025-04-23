// 技能基类，继承自 ScriptableObject
using System.Collections.Generic;
using UnityEngine;


public class Petrified : Skill
{
    // 技能名字
    public override string SkillName
    {
        get
        {
            return "石化";
        }
    }

    // 技能描述
    public override string SkillDetail
    {
        get
        {
            return "第5回合时石化, 无法攻击";
        }  
    }

    // 初始化方法
    public override void Init(BasicCharacter character)
    {
        skillType = SkillType.Passive;

    }

    // 使用方法
    public override bool Use(Hero hero, Enemy target = null)
    {
        return true;
    }
}