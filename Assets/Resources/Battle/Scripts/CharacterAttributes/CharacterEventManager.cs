using System;
using UnityEngine;

/// <summary>
/// 角色事件管理器，统一管理角色的各类事件
/// </summary>
public class CharacterEventManager
{
    // 持有事件所属的角色实例
    private readonly BasicCharacter _owner;

    // 监听血量变化事件
    public event Action<BasicCharacter> OnHealthPointsChanged;
    // 监听受到治疗事件
    public event Action<BasicCharacter> OnHealingReceived;
    // 监听防御变化事件
    public event Action<BasicCharacter> OnDefenseChanged;
    // 监听攻击变化事件
    public event Action<BasicCharacter> OnAttackChanged;
    // 监听被攻击事件
    public event Action<BasicCharacter> OnAttackedBy;

    // 攻击前事件（攻击者角度）
    public event Action<BasicCharacter> OnBeforeAttack;
    // 攻击时事件（攻击者角度）
    public event Func<float, DamageType> OnAttacking;
    // 攻击后事件（攻击者角度）
    public event Action<BasicCharacter, float> OnAfterAttack;
    // 受伤前事件（被攻击者角度，返回修改后伤害）
    public event Func<float, DamageType, float> OnBeforeTakeDamage;
    // 受伤后事件（被攻击者角度）
    public event Action<float, BasicCharacter> OnAfterTakeDamage;
    // 受到治疗前事件（修改治疗量）
    public event Func<int, int> OnBeforeHealing;
    // 受到治疗后事件
    public event Action<int> OnAfterHealing;

    // 主动技能使用前（可用于打断、前置校验）
    public event Func<Skill, bool> OnSkillActiveBefore;
    // 主动技能使用后（可用于播放特效、记录日志）
    public event Action<Skill, BasicCharacter> OnSkillActiveAfter;
    // 被动技能触发时（可用于显示提示、联动效果）
    public event Action<Skill, BasicCharacter> OnSkillPassiveTriggered;

    // 移动开始时（参数：目标位置）
    public event Action<Location> OnMoveStart;
    // 移动结束时（参数：原位置、新位置）
    public event Action<Location, Location> OnMoveEnd;

    public CharacterEventManager(BasicCharacter owner)
    {
        _owner = owner;
    }

    // 触发攻击前事件
    internal void TriggerBeforeAttack(BasicCharacter target)
    {
        OnBeforeAttack?.Invoke(target);
    }

    // 触发攻击时事件
    internal float TriggerAttacking(float originalDamage, DamageType damageType)
    {
        float modifiedDamage = originalDamage;
        if (OnAttacking != null)
        {
            foreach (var handler in OnAttacking.GetInvocationList())
            {
                modifiedDamage = (float)handler.DynamicInvoke(modifiedDamage, damageType);
            }
        }
        return modifiedDamage;
    }

    // 触发攻击后事件
    internal void TriggerAfterAttack(BasicCharacter target, float actualDamage)
    {
        OnAfterAttack?.Invoke(target, actualDamage);
    }

    // 触发受伤前事件（返回修改后的伤害）
    internal float TriggerBeforeTakeDamage(float originalDamage, DamageType damageType)
    {
        float modifiedDamage = originalDamage;
        if (OnBeforeTakeDamage != null)
        {
            foreach (var handler in OnBeforeTakeDamage.GetInvocationList())
            {
                modifiedDamage = (float)handler.DynamicInvoke(modifiedDamage, damageType);
            }
        }
        return modifiedDamage;
    }

    // 触发受伤后事件
    internal void TriggerAfterTakeDamage(float actualDamage, BasicCharacter attacker)
    {
        OnAfterTakeDamage?.Invoke(actualDamage, attacker);
    }

    // 触发治疗前事件
    internal int TriggerBeforeHealing(ref int healingAmount)
    {
        if (OnBeforeHealing != null)
        {
            foreach (var handler in OnBeforeHealing.GetInvocationList())
            {
                healingAmount = (int)handler.DynamicInvoke(healingAmount);
            }
        }
        return healingAmount;
    }

    // 触发治疗后事件
    internal void TriggerAfterHealing(int actualHealing)
    {
        OnAfterHealing?.Invoke(actualHealing);
    }

    // 触发血量变化事件
    internal void TriggerHealthPointsChanged()
    {
        OnHealthPointsChanged?.Invoke(_owner);
    }

    // 触发受到治疗事件
    internal void TriggerHealingReceived()
    {
        OnHealingReceived?.Invoke(_owner);
    }

    // 触发防御变化事件
    internal void TriggerDefenseChanged()
    {
        OnDefenseChanged?.Invoke(_owner);
    }

    // 触发攻击变化事件
    internal void TriggerAttackChanged()
    {
        OnAttackChanged?.Invoke(_owner);
    }

    // 触发被攻击事件
    internal void TriggerAttackedBy(BasicCharacter attacker)
    {
        OnAttackedBy?.Invoke(attacker);
    }

    // 触发主动技能使用前事件
    internal bool TriggerSkillActiveBefore(Skill skill)
    {
        bool canActivate = true;
        if (OnSkillActiveBefore != null)
        {
            foreach (var handler in OnSkillActiveBefore.GetInvocationList())
            {
                // 只要有一个处理器返回false，就视为不能激活技能
                canActivate = (bool)handler.DynamicInvoke(skill);
                if (!canActivate)
                    break;
            }
        }
        return canActivate;
    }

    // 触发主动技能使用后事件
    internal void TriggerSkillActiveAfter(Skill skill, BasicCharacter target)
    {
        OnSkillActiveAfter?.Invoke(skill, target);
    }

    // 触发被动技能触发事件
    internal void TriggerSkillPassiveTriggered(Skill skill, BasicCharacter target)
    {
        OnSkillPassiveTriggered?.Invoke(skill, target);
    }

    // 触发移动开始事件
    internal void TriggerMoveStart(Location targetLocation)
    {
        OnMoveStart?.Invoke(targetLocation);
    }

    // 触发移动结束事件
    internal void TriggerMoveEnd(Location originalLocation, Location newLocation)
    {
        OnMoveEnd?.Invoke(originalLocation, newLocation);
    }
}

