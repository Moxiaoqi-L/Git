using UnityEngine;

// 增幅攻击技能，继承自 Skill 基类
// 该技能用于对目标敌人造成双倍攻击力的伤害
public class AmplifiedAttackSkill : Skill
{
    /// <summary>
    /// 构造函数，调用基类构造函数并传入技能名称 "增幅攻击"
    /// </summary>
    public AmplifiedAttackSkill() : base("增幅攻击") { }

    /// <summary>
    /// 实现基类的 Use 方法，定义增幅攻击技能的使用逻辑
    /// </summary>
    /// <param name="hero">使用技能的英雄</param>
    /// <param name="target">技能的目标敌人</param>
    public override void Use(Hero hero, Enemy target = null)
    {
        // 检查是否指定了目标敌人
        if (target == null)
        {
            // 若未指定目标敌人，输出提示信息并返回
            Debug.Log("使用增幅攻击技能需要指定目标敌人！");
            return;
        }

        // 计算实际的攻击力，考虑所有攻击类 BUFF 的加成
        float actualAttack = hero.GetActualAttack();
        // 计算技能造成的伤害，为实际攻击力的两倍
        float damage = actualAttack * 2 * (1 + hero.heroAttributes.skillPower) * (1 + hero.heroAttributes.damagePower);

        target.Defend((int)damage);
    }
}