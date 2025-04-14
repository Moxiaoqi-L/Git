using UnityEngine;

// 自我治疗技能，继承自 Skill 基类
// 该技能用于为英雄恢复 100 点生命值
public class SelfHealingSkill : Skill
{
    /// <summary>
    /// 构造函数，调用基类构造函数并传入技能名称 "自我治疗"
    /// </summary>
    public SelfHealingSkill() : base("自我治疗") { }

    /// <summary>
    /// 实现基类的 Use 方法，定义自我治疗技能的使用逻辑
    /// </summary>
    /// <param name="hero">使用技能的英雄</param>
    /// <param name="target">此技能不需要目标敌人，默认为 null</param>
    public override void Use(Hero hero, Enemy target = null)
    {
        // 增加英雄的生命值 100 点
        hero.IncreaseHealthPoints(100);
        // 输出英雄使用技能恢复生命值的日志
        Debug.Log(hero.heroAttributes.name + " 使用技能: " + skillName + ", 当前生命: " + hero.currentHealthPoints);
    }
}  