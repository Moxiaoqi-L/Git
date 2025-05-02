// filePath: Git/Assets/Resources/Battle/Scripts/Buff/Buffs/AttackIncreaseBuff.cs
using UnityEngine;

public class AttackIncreaseBuff : Buff
{
    private int attackIncreasePercentage; // 攻击力提升百分比
    private int actualIncreaseValue;      // 实际提升的攻击力数值（基于基础攻击计算）

    public AttackIncreaseBuff(int attackIncreasePercentage, int initialLayers)
    {
        buffName = "<color=#FFA500>攻击力提升</color>";
        this.attackIncreasePercentage = attackIncreasePercentage;
        stackLayers = initialLayers; // 持续回合数
        duration = 0; // 依赖回合结束移除
        UpdateBuffDetail();
    }

    // 更新BUFF描述（显示提升百分比和剩余回合）
    private void UpdateBuffDetail()
    {
        buffDetail = $"攻击力提升 <color=#FFA500>{attackIncreasePercentage}%</color> \n持续<color=#FFA500>{stackLayers}</color>回合";
    }

    // 应用BUFF时提升攻击力
    public override void Apply(BasicCharacter character)
    {
        base.Apply(character);
        // 计算实际提升值：基础攻击力 * 百分比
        actualIncreaseValue = Mathf.RoundToInt(character.characterAttributes.attack * (attackIncreasePercentage / 100f));
        // 调用角色方法增加临时攻击力
        character.IncreaseAttack(actualIncreaseValue);
    }

    // 移除BUFF时恢复攻击力
    public override void Remove(BasicCharacter character)
    {
        character.IncreaseAttack(-actualIncreaseValue); // 减去提升的数值
    }

    // 回合结束时更新层数和描述
    public override void OnRoundEnd(BasicCharacter character)
    {
        base.OnRoundEnd(character); // 基类会减少stackLayers
        UpdateBuffDetail();
    }

    // 刷新BUFF（叠加层数或延长持续时间）
    public override void Refresh(Buff newBuff)
    {
        if (newBuff is AttackIncreaseBuff newAttackBuff)
        {
            // 叠加持续回合, 取最大值
            stackLayers = Mathf.Max(stackLayers, newAttackBuff.stackLayers);
            // 保留更高的百分比
            attackIncreasePercentage = Mathf.Max(attackIncreasePercentage, newAttackBuff.attackIncreasePercentage);
            UpdateBuffDetail();
        }
    }
}