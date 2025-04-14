using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "强力攻击", menuName = "技能/强力攻击")]
public class AmplifiedAttackSkill : Skill
{   
    // 技能花费
    public Color[] costs = {Constants.REDPOINT,Constants.REDPOINT};
    public override void Use(Hero hero, Enemy target = null)
    {
        if (!ColorPointCtrl.Get.RemoveColorPointsByColors(costs)) return;
        // 技能使用逻辑
        if (target == null)
        {
            // 尝试获取英雄所在列的第一个敌人
            List<Enemy> enemiesInSameColumn = MethodsForSkills.GetEnemiesInSameColumn(hero);
            if (enemiesInSameColumn.Count > 0)
            {
                target = enemiesInSameColumn[0];
            }
            else
            {
                // 如果该列没有敌人，选择最近的敌人
                target = MethodsForSkills.GetNearestEnemy(hero);
            }
        }

        if (target == null)
        {
            Debug.Log("没有可用的敌人目标！");
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
    // 技能花费
    public Color[] costs = {Constants.BLUEPOINT,Constants.BLUEPOINT};
    public override void Use(Hero hero, Enemy target = null)
    {
        if (!ColorPointCtrl.Get.RemoveColorPointsByColors(costs)) return;
        hero.IncreaseHealthPoints(30);
        Debug.Log(hero.heroAttributes.name + " 使用技能: " + skillName + ", 当前生命: " + hero.currentHealthPoints);
    }
}
