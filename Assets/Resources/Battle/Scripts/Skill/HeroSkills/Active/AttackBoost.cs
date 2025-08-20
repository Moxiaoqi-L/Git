// filePath: Git/Assets/Resources/Battle/Scripts/Skill/HeroSkills/Active/AttackBoostSkill.cs
using System.Collections.Generic;
using UnityEngine;

public class AttackBoost : Skill
{
    // 攻击力提升百分比（5%）
    public int attackIncreasePercentage = 20;
    // 持续回合数
    public int durationRounds = 2;

    // 技能名称
    public override string SkillName => "攻击力提升";

    // 技能描述
    public override string SkillDetail => 
        $"为自身附加<color=#FFA500>{attackIncreasePercentage}%攻击力</color>提升，持续<color=#FFA500>{durationRounds}</color>回合";

    // 技能消耗（示例：2个红点）
    public override List<Color> Costs => new() { Constants.REDPOINT, Constants.REDPOINT };

    // 初始化技能类型
    public override void Init(SkillManager skillManager, BasicCharacter character)
    {
        base.Init(skillManager, character);
        skillType = SkillType.Active; // 设置为主动技能
    }

    // 使用技能时添加BUFF
    public override bool Use(Hero hero, Enemy target = null)
    {
        // 为自身添加攻击力提升BUFF（参数：百分比，持续回合数）
        hero.AddBuff("攻击力提升", attackIncreasePercentage, durationRounds);
        return true; // 技能使用成功
    }
}