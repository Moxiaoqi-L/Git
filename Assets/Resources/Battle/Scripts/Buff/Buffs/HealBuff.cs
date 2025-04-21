// Battle/Scripts/Chessman/Buff/HealOverRoundBuff.cs
using UnityEngine;

public class HealBuff : Buff
{
    private int healPerLayer; // 每层恢复量

    public HealBuff(int healPerLayer, int initialLayers)
    {
        buffName = "<color=#30e3ca>愈合</color>";
        buffSprite = Resources.Load<Sprite>($"Battle/Image/Buff/愈合");
        this.healPerLayer = healPerLayer;
        stackLayers = initialLayers; // 初始层数
        duration = 0; // 无固定持续时间，依赖层数
        // BUFF 细节
        buffDetail = "回合结束时回复自身<color=#00E180>" + healPerLayer * stackLayers + "</color>生命";
    }

    public override void Apply(BasicCharacter character)
    {
        // 无需立即效果，依赖回合结束触发
    }

    public override void Remove(BasicCharacter character)
    {
        // 无持续效果需要移除
    }

    public override void OnRoundEnd(BasicCharacter character)
    {
        if (stackLayers > 0)
        {
            int healAmount = healPerLayer * stackLayers;
            character.IncreaseHealthPoints(healAmount); // 调用角色恢复方法
        }
        base.OnRoundEnd(character); // 减少层数
        buffDetail = "回合结束时回复自身<color=#30e3ca>" + healPerLayer * stackLayers + "</color>生命";
    }

    public override void Refresh(Buff newBuff)
    {
        stackLayers += newBuff.stackLayers; // 叠加层数
        buffDetail = "回合结束时回复自身<color=#30e3ca>" + healPerLayer * stackLayers + "</color>生命";
    }
}