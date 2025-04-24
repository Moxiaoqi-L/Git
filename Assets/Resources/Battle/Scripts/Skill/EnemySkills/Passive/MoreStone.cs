// Scripts/Skill/EnemySkills_C/Passive/MoreStone.cs
using System.Collections.Generic;
using UnityEngine;


public class MoreStone : Skill
{
    // 伤害增幅百分
    public int damageAmplifyPercentage = 20;
    // 额外攻击范围（示例：扩展 1 格）
    public List<Location> extraAttackRange = new() { new(0, -3), new(1, -3), new(-1, -3)};

    private Enemy targetEnemy;
    private int consecutiveMissedAttacks; // 连续落空次数

    public override string SkillName => "石头！更多的石头！";

    public override string SkillDetail => "当连续两回合攻击落空时\n" +
                                          "获得 <color=#ff2e63>额外攻击范围</color>\n" +
                                          "获得 <color=#ff2e63>20% 伤害增幅</color>";

    public override void Init(SkillManager skillManager, BasicCharacter character)
    {
        base.Init(skillManager, character);
        skillType = SkillType.Passive;
        var enemy =  character as Enemy;
        if (enemy == null) return;
        targetEnemy = enemy;
        consecutiveMissedAttacks = 0;
        
        // 监听攻击完成事件（判断是否落空）
        targetEnemy.OnAttackMissed += OnAttackMissed;
        targetEnemy.OnAttackCompleted += OnAttackCompleted;

        Debug.Log("Setup:" + consecutiveMissedAttacks);

    }

    private void OnAttackMissed(Hero targetHero, Enemy attacker)
    {        
        if (consecutiveMissedAttacks == -1) return;
        // 落空次数 + 1
        consecutiveMissedAttacks++;

        if (consecutiveMissedAttacks >= 2)
        {
            ApplyEffects();
            consecutiveMissedAttacks = -1; // 重置计数器
        }
    }

    private void OnAttackCompleted(Hero targetHero, Enemy attacker)
    {
        if (consecutiveMissedAttacks == -1) return;
        consecutiveMissedAttacks = 0;
    }

    private void ApplyEffects()
    {
        
        // 增加伤害增幅（通过临时属性）
        targetEnemy.IncreaseDamagePower(20);
        
        // 扩展攻击范围
        foreach (var location in extraAttackRange)
        {
            targetEnemy.attackRange.Add(location);
        }
        Debug.Log(targetEnemy.attackRange.Count);
    }

    public override bool Use(Hero hero, Enemy target = null)
    {
        // 被动技能无需主动使用
        return false;
    }
}