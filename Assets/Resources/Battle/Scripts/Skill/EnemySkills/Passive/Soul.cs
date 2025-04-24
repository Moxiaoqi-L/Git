// 技能基类，继承自 ScriptableObject
using System;
using System.Collections.Generic;
using UnityEngine;

public class Soul : Skill
{
    // 技能名字
    public override string SkillName
    {
        get
        {
            return "灵化";
        }
    }

    // 技能描述
    public override string SkillDetail
    {
        get
        {
            return "受到的物理伤害降低 50%";
        }  
    }

    // 初始化方法
    public override void Init(SkillManager skillManager, BasicCharacter character)
    {
        base.Init(skillManager, character);
        skillManager.character.BeforeLoseHp += DecreaseDamage;
    }

    public float DecreaseDamage(float damage, DamageType damageType)
    {
        if (damageType == DamageType.Physical)
        {
            skillManager.character.ShowText("灵化！", Color.white);
            return damage/2f;
        } 
        return damage;
    }

    // 使用方法
    public override bool Use(Hero hero, Enemy target = null)
    {
        return true;
    }
}