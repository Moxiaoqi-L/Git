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
    public List<Color> costs = new() { };
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
    public virtual string SkillName { get { return skillName; } }

    // 技能描述
    public virtual string SkillDetail { get { return skillDetail; } }

    // 技能消耗
    public virtual List<Color> Costs { get { return costs; } }

    // 初始化方法
    public virtual void Init(SkillManager skillManager, BasicCharacter character)
    {
        this.skillManager = skillManager;
        skillAudio = Resources.Load<AudioClip>("General/Audio/Skill/" + GetType().ToString());
        skillSprite = Resources.Load<Sprite>("General/Image/Skill/" + GetType().ToString());
    }

    // 使用方法
    public virtual bool Use(Hero hero, Enemy target = null) { return false; }

    public virtual bool BeforeUse()
    {
        // 技能禁用
        if (skillManager.character.cantUseSkills) return false;
        // 眩晕的异常状态
        if (skillManager.character.isStunned) return false;
        // 检查是否行动过
        if (skillManager.character.hasAttacked) return false;
        // 技能点不够
        if (!ColorPointCtrl.Get.RemoveColorPointsByColors(Costs)) return false;
        return true;
    }

    public virtual void AfterUse()
    {
        Hero h = (Hero)skillManager.character;
        h.CompleteAction();
    }

    // 统一执行入口，封装完整流程
    public bool Execute(Hero hero, Enemy target = null)
    {
        // 1. 前置检查：无法使用则直接返回
        if (!BeforeUse()) return false;

        // 2. 执行技能具体逻辑
        bool isSuccess = Use(hero, target);

        // 3. 技能使用成功后执行后置处理
        if (isSuccess) AfterUse();

        return isSuccess;
    }
}