// 技能基类，继承自 ScriptableObject
using System.Collections.Generic;
using UnityEngine;

public class YourSkillName : Skill
{
    // 技能名字
    public override string SkillName
    {
        get
        {
            return "";
        }
    }

    // 技能描述
    public override string SkillDetail
    {
        get
        {
            return "";
        }  
    }

    // 初始化方法
    public override void Init(SkillManager skillManager, BasicCharacter character)
    {
        base.Init(skillManager, character);

    }

    // 使用方法
    public override bool Use(Hero hero, Enemy target = null)
    {
        return true;
    }
}