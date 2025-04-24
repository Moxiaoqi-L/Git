using System.Collections.Generic;
using UnityEngine;


public class PowerAttack : Skill
{

    public float attackDamageMultiplier = 200;

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

    public override void Init(SkillManager skillManager, BasicCharacter character)
    {
        base.Init(skillManager, character);
        skillType = SkillType.Active;

        // 无需Setup
    }

    public override bool Use(Hero hero, Enemy target = null)
    {
        if (!BeforeUse()) return false;
        if (hero.isStunned) return false;
        if (hero.hasAttacked || !ColorPointCtrl.Get.RemoveColorPointsByColors(Costs)) return false;
        // 技能使用逻辑
        if (target == null)
        {
            // 尝试获取英雄所在列的第一个敌人
            List<Enemy> enemiesInSameColumn = MethodsForSkills.GetEnemiesInSameColumn(hero);
            if (enemiesInSameColumn.Count > 0)
            {
                int minY = 5;
                foreach (Enemy enemy in enemiesInSameColumn)
                {
                    int currentY = enemy.chessman.location.y;
                    if (currentY <= minY)
                    {
                        minY = currentY;
                        target = enemy;
                    }
                }
            }
        }

        if (target == null)
        {
            Debug.Log("没有可用的敌人目标！");
            return false;
        }

        float actualAttack = hero.GetActualAttack();
        float damage = actualAttack * (attackDamageMultiplier / 100) * (1 + hero.characterAttributes.skillPower) * (1 + hero.characterAttributes.damagePower);
        target.Defend(damage, DamageType.Physical);
        target.AddBuff("眩晕", 1);
        AudioManager.Get.PlaySound(skillAudio);
        hero.FinishAttack(target);
        return true;
    }
}
