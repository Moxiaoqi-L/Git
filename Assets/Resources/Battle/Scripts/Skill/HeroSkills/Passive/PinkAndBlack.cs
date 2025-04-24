// Scripts/Skill/HeroSkills_C/Passive/PinkAndBlack.cs
using UnityEngine;


public class PinkAndBlack : Skill
{
    // 目标地块Y值
    public int targetY = 0;
    // 每层中毒BUFF数值
    public int damagePerLayer = 1;
    // BUFF层数
    public int layers = 2;

    public override string SkillName
    {
        get 
        { 
            return "粉切黑"; 
        }
    }

    public override string SkillDetail
    {
        get 
        { 
            return "在后排时, 攻击对敌人施加 <color=#aa96da>"+ layers +" </color>层 <color=#aa96da>中毒</color>\n"
            + "每层中毒造成 <color=#aa96da>" + damagePerLayer + " </color>伤害"; 
        }
    }

    // 被动技能初始化（在Hero加载时调用）
    public override void Init(SkillManager skillManager, BasicCharacter character)
    {
        base.Init(skillManager, character);
        var hero =  character as Hero;
        skillType = SkillType.Passive;
        // 监听英雄的攻击事件（需要在Hero中添加事件回调）
        hero.OnAttackCompleted += ApplyPoison;
    }

    public override bool Use(Hero hero, Enemy target = null)
    {
        // 无需实现
        return false;
    }

    // 攻击完成时的回调方法
    private void ApplyPoison(Hero hero, Enemy target)
    {
        // 检查当前所在地块的Y坐标是否为0
        if (hero.chessman.location.y == targetY && target)
        {
            // 添加中毒Buff
            target.AddBuff("中毒", damagePerLayer, layers); 
        }
    }
}