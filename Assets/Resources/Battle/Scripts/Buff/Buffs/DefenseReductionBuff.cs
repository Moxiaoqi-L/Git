using UnityEngine;

public class DefenseReductionBuff : Buff
{
    private int defenseReduction; // 降低的防御力数值

    public DefenseReductionBuff(int defenseReduction, int initialLayers)
    {
        buffName = "<color=#2E8B57>防御力下降</color>"; // 青绿色标记
        this.defenseReduction = defenseReduction;
        stackLayers = initialLayers; // 持续回合数
        duration = 0; // 依赖回合结束移除
        UpdateBuffDetail();
    }

    private void UpdateBuffDetail()
    {
        buffDetail = "防御力降低 <color=#2E8B57>" + defenseReduction + "</color> \n"
                    + "持续<color=#2E8B57>" + stackLayers + "</color>回合";
    }

    public override void Apply(BasicCharacter character)
    {
        base.Apply(character);
        // 应用防御力降低效果
        character.IncreaseDefense(-defenseReduction);
    }

    public override void Remove(BasicCharacter character)
    {
        // 移除防御力降低效果
        character.IncreaseDefense(defenseReduction);
    }

    public override void OnRoundEnd(BasicCharacter character)
    {
        // 回合结束时减少层数（基类已实现 stackLayers--）
        base.OnRoundEnd(character);
        UpdateBuffDetail();
    }

    public override void Refresh(Buff newBuff)
    {
        if (newBuff is DefenseReductionBuff newDefenseReductionBuff)
        {
            // 选取降低防御力最多的保留
            defenseReduction = Mathf.Max(defenseReduction, newDefenseReductionBuff.defenseReduction);
            // 选取持续回合最长的保留
            stackLayers = Mathf.Max(stackLayers, newDefenseReductionBuff.stackLayers);
            UpdateBuffDetail();
        }
    }
}