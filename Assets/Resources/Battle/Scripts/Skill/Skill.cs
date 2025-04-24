// 技能基类
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Skill
{
    // 技能名字
    public string skillName;
    // 技能描述
    public string skillDetail;
    // 技能消耗
    public List<Color> costs = new() {};
    // 技能类型
    public SkillType skillType;
    // 技能等级
    public int skillLevel;

    // 技能关联的管理器
    public SkillManager skillManager;

    // 使用技能的音效
    protected AudioClip skillAudio;

    // 技能图标
    public Sprite skillSprite;

    // 技能名字
    public virtual string SkillName { get{return skillName;} }

    // 技能描述
    public virtual string SkillDetail { get{return skillDetail;} }

    // 技能消耗
    public virtual List<Color> Costs{ get{return costs;} }

    // 初始化方法
    public virtual void Init(SkillManager skillManager, BasicCharacter character)
    {
        this.skillManager = skillManager;
        skillAudio = Resources.Load<AudioClip>("Battle/Audio/Skill/" + GetType().ToString());
        skillSprite = Resources.Load<Sprite>("Battle/Image/Skill/" + GetType().ToString());
    }

    // 使用方法
    public virtual bool Use(Hero hero, Enemy target = null){return false;}

    public virtual bool BeforeUse()
    {
        if (skillManager.character.cantUseSkills) return false;
        return true;
    }
}