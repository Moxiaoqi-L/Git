// Battle/Scripts/Chessman/Buff/HealOverRoundBuff.cs
using UnityEngine;

public class HealBuff : Buff
{
    private int healPerLayer; // 每层恢复量

    public HealBuff(int healPerLayer, int initialLayers)
    {
        buffName = "愈合";
        this.healPerLayer = healPerLayer;
        stackLayers = initialLayers; // 初始层数
        duration = 0; // 无固定持续时间，依赖层数
    }

    public override void Apply(BasicCharacter character)
    {
        Debug.Log("治愈BUFF被赋予了！");
        // 无需立即效果，依赖回合结束触发
    }

    public override void Remove(BasicCharacter character)
    {
        // 无持续效果需要移除
    }

    public override void OnRoundEnd(BasicCharacter character)
    {
        Debug.Log("回合结束，治愈BUFF生效！");
        if (stackLayers > 0)
        {
            int healAmount = healPerLayer * stackLayers;
            character.IncreaseHealthPoints(healAmount); // 调用角色恢复方法
        }
        base.OnRoundEnd(character); // 减少层数
    }

    public override void Refresh(Buff newBuff)
    {
        stackLayers += newBuff.stackLayers; // 叠加层数
    }
}