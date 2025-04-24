// Scripts/Buff/Buffs/SoulLossBuff.cs
using UnityEngine;

public class SoulLossBuff : Buff
{
    // 易伤
    public int damageMultiplier = 25; 

    public SoulLossBuff(int initialLayers)
    {
        stackLayers = initialLayers; // 持续回合数
        duration = 0; // 依赖回合结束移除
        buffName = "<color=#608BBB>失魂</color>";
        UpdateBuffDetail();
    }

    private void UpdateBuffDetail()
    {
        buffDetail = "受到的伤害增加 <color=#608BBB>" + damageMultiplier + "%</color> \n"
                    + "持续<color=#608BBB>" + stackLayers + "</color>回合";        
    }

    public override void Apply(BasicCharacter character)
    {
        base.Apply(character);
        // 应用伤害加成
        character.IncreaseDamageTakenMultiplier(damageMultiplier);
    }

    public override void Remove(BasicCharacter character)
    {
        // 移除伤害加成
        character.IncreaseDamageTakenMultiplier(-damageMultiplier);
    }

    public override void OnRoundEnd(BasicCharacter character)
    {
        // 回合结束时减少层数（基类已实现 stackLayers--）
        base.OnRoundEnd(character);
        UpdateBuffDetail();
    }

    public override void Refresh(Buff newBuff)
    {
        // 可叠加持续回合（如层数+1则多持续一回合）
        stackLayers += newBuff.stackLayers;
        buffDetail = "受到的伤害增加 <color=#608BBB>" + damageMultiplier + "%</color> \n"
                    + "持续<color=#608BBB>" + stackLayers + "</color>回合";
    }
}