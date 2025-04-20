using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "强力攻击", menuName = "技能/强力攻击")]
public class PowerAttack : Skill
{
    public override string SkillName
    {
        get
        {
            return "强力攻击";
        }
    }
    public override string SkillDetail
    {
        get
        {
            return "对正前方敌人造成 <color=#ff2e63>" + attackDamageMultiplier + "%攻击力 </color>的伤害\n"
            + "并 <color=#ff9a00>击晕</color> 敌人";
        }
    }
    public override List<Color> Costs
    {
        get
        {
            return new List<Color> { Constants.REDPOINT, Constants.REDPOINT};
        }
    }
    
    public float attackDamageMultiplier;


    public override void Setup(Hero hero = null, Enemy enemy = null)
    {
        // 无需Setup
    }

    public override void Use(Hero hero, Enemy target = null)
    {
        if (hero.isStunned) return;
        if (hero.hasAttacked || !ColorPointCtrl.Get.RemoveColorPointsByColors(Costs)) return;
        // 技能使用逻辑
        if (target == null)
        {
            // 尝试获取英雄所在列的第一个敌人
            List<Enemy> enemiesInSameColumn = MethodsForSkills.GetEnemiesInSameColumn(hero);
            if (enemiesInSameColumn.Count > 0)
            {
                target = enemiesInSameColumn[0];
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
        target.AddBuff("眩晕", 1);
        hero.FinishAttack(target);
    }
}
