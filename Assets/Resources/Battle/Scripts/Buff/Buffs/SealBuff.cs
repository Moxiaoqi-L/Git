using UnityEngine;

public class SealBuff : Buff
{
    public SealBuff(int initialLayers)
    {
        buffName = "<color=#8B0000>封印</color>";
        buffDetail = "无法使用技能，持续<color=#8B0000>" + initialLayers + "</color>回合";
        stackLayers = initialLayers; // 持续回合数
        duration = 0; // 依赖回合结束移除
    }

    public override void Apply(BasicCharacter character)
    {
        base.Apply(character);
        Debug.Log("技能封印");
        character.cantUseSkills = true; // 新增状态：禁止使用技能
    }

    public override void Remove(BasicCharacter character)
    {
        character.cantUseSkills = false; // 移除禁止状态
    }

    public override void OnRoundEnd(BasicCharacter character)
    {
        base.OnRoundEnd(character);
        buffDetail = "持续<color=#8B0000>" + stackLayers + "</color>回合"; // 更新剩余回合显示
    }
}