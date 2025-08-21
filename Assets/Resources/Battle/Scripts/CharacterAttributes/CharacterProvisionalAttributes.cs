using System;
using UnityEngine;

public class CharacterProvisionalAttributes : MonoBehaviour
{

    public CharacterAttributes characterAttributes;


    // 攻击力
    public int provisionalAttack;
    // 增加攻击力的方法
    public virtual void IncreaseAttack(int amount)
    {
        provisionalAttack += amount;
    }
    public int GetActualAttack()
    {
        return characterAttributes.attack + provisionalAttack;
    }

    // 伤害类型
    public DamageType provisionalDamageType;

    // 暴击几率
    public int provisionalCriticalRate;
    public virtual void IncreaseCriticalRate(int amount)
    {
        provisionalCriticalRate += amount;
    }
    public int GetActualCriticalRate()
    {
        return characterAttributes.criticalRate + provisionalCriticalRate;
    }

    // 暴击伤害
    public int provisionalCriticalDamageMultiplier;
    public virtual void IncreaseCriticalDamageMultiplier(int amount)
    {
        provisionalCriticalDamageMultiplier += amount;
    }
    public int GetActualCriticalDamageMultiplier()
    {
        return characterAttributes.criticalDamageMultiplier + provisionalCriticalDamageMultiplier;
    }


    // 技能增幅
    public int provisionalSkillPower;
    public virtual void IncreaseSkillPower(int amount)
    {
        provisionalSkillPower += amount;
    }
    public int GetActualSkillPower()
    {
        return characterAttributes.skillPower + provisionalSkillPower;
    }


    // 伤害增幅
    public int provisionalDamagePower;
    public virtual void IncreaseDamagePower(int amount)
    {
        provisionalDamagePower += amount;
    }
    public int GetActualDamagePower()
    {
        return characterAttributes.damagePower + provisionalDamagePower;
    }

    // 护甲
    public int provisionalArmor;
    public virtual void IncreaseArmor(int amount)
    {
        provisionalArmor += amount;
    }
    
    // 防御力
    public int provisionalDefense;
    public virtual void IncreaseDefense(int amount)
    {
        provisionalDefense += amount;
    }
    public int GetActualDefense()
    {  
        return characterAttributes.defense + provisionalDefense;
    }

    // 精神抗性
    public int provisionalMagicDefense;
    public virtual void IncreaseMagicDefense(int amount)
    {
        provisionalMagicDefense += amount;
    }
    public int GetActualMagicDefense()
    {
        return characterAttributes.magicDefense;
    }

    // 伤害修改
    public int provisionalDamageTakenMultiplier;
    public virtual void IncreaseDamageTakenMultiplier(int amount)
    {
        provisionalDamageTakenMultiplier += amount;
    }
    public int GetActualDamageTakenMultiplier()
    {
        return characterAttributes.damageTakenMultiplier + provisionalDamageTakenMultiplier;
    }


    // 当前生命值
    public int currentHealthPoints;
    // 获取当前 HP
    public int GetActualHP()
    {
        return currentHealthPoints;
    }

    // 最大生命值
    public int provisionalMaxHealthPoints;
    public virtual void IncreaseMaxHealthPoints(int amount)
    {
        provisionalMaxHealthPoints += amount;
    }
    public int GetActualMaxHP()
    {
        return characterAttributes.maxHealthPoints;
    }
}
