using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "强力攻击", menuName = "技能/强力攻击")]
public class PowerAttack : Skill
{   
    // 技能花费
    public Color[] costs = {Constants.REDPOINT,Constants.REDPOINT};
    public float attackDamageMultiplier;
    public override void Use(Hero hero, Enemy target = null)
    {
        if (hero.isStunned) return;
        if (hero.hasAttacked || !ColorPointCtrl.Get.RemoveColorPointsByColors(costs)) return;
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
        float damage = actualAttack * (attackDamageMultiplier / 100) * (1 + hero.characterAttributes.skillPower) * (1 + hero.characterAttributes.damagePower);
        target.Defend((int)damage);
        hero.FinishAttack();
    }
}
