using UnityEngine;

[CreateAssetMenu(fileName = "强力攻击", menuName = "技能/强力攻击")]
public class AmplifiedAttackSkill : Skill
{
    public override void Use(Hero hero, Enemy target = null)
    {
        // 技能使用逻辑
        if (target == null)
        {
            Debug.Log("使用增幅攻击技能需要指定目标敌人！");
            return;
        }
        float actualAttack = hero.GetActualAttack();
        float damage = actualAttack * 2 * (1 + hero.heroAttributes.skillPower) * (1 + hero.heroAttributes.damagePower);
        target.Defend((int)damage);
    }
}

// 自我治疗技能
[CreateAssetMenu(fileName = "自我治疗", menuName = "技能/自我治疗")]
public class SelfHealingSkill : Skill
{
    public override void Use(Hero hero, Enemy target = null)
    {
        hero.IncreaseHealthPoints(100);
        Debug.Log(hero.heroAttributes.name + " 使用技能: " + skillName + ", 当前生命: " + hero.currentHealthPoints);
    }
}
