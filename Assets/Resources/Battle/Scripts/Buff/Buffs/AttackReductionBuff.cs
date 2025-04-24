using UnityEngine;

// 攻击力下降BUFF类
public class AttackReductionBuff : Buff
{
    private int attackReduction; // 降低的攻击力数值

    public AttackReductionBuff(int attackReduction, int initialLayers)
    {
        buffName = "<color=#FF4500>攻击力下降</color>";
        this.attackReduction = attackReduction;
        stackLayers = initialLayers; // 持续回合数
        duration = 0; // 依赖回合结束移除
        UpdateBuffDetail();
    }

    private void UpdateBuffDetail()
    {
        buffDetail = "攻击力降低 <color=#FF4500>" + attackReduction + "</color> \n"
                    + "持续<color=#FF4500>" + stackLayers + "</color>回合";
    }

    public override void Apply(BasicCharacter character)
    {
        base.Apply(character);
        // 应用攻击力降低效果
        character.IncreaseAttack(-attackReduction);
    }

    public override void Remove(BasicCharacter character)
    {
        // 移除攻击力降低效果
        character.IncreaseAttack(attackReduction);
    }

    public override void OnRoundEnd(BasicCharacter character)
    {
        // 回合结束时减少层数（基类已实现 stackLayers--）
        base.OnRoundEnd(character);
        UpdateBuffDetail();
    }

    public override void Refresh(Buff newBuff)
    {
        if (newBuff is AttackReductionBuff newAttackReductionBuff)
        {
            // 选取降低攻击力最多的保留
            attackReduction = Mathf.Max(attackReduction, newAttackReductionBuff.attackReduction);
            // 选取持续回合最长的保留
            stackLayers = Mathf.Max(stackLayers, newAttackReductionBuff.stackLayers);
            UpdateBuffDetail();
        }
    }
}