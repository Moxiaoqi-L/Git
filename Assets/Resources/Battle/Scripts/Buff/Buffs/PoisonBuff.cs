// Battle/Scripts/Chessman/Buff/PoisonBuff.cs
using UnityEngine;

public class PoisonBuff : Buff
{
    private int damagePerLayer; // 每层伤害量

    public PoisonBuff(int damagePerLayer, int initialLayers)
    {
        buffName = "<color=#8470FF>中毒</color>";
        buffSprite = Resources.Load<Sprite>($"Battle/Image/Buff/中毒");
        this.damagePerLayer = damagePerLayer;
        stackLayers = initialLayers;
        duration = 0; // 无固定持续时间，依赖层数
        buffDetail = "回合结束时受到<color=#8470FF>" + damagePerLayer * stackLayers + "</color>点伤害";
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
            int damageAmount = damagePerLayer * stackLayers;
            character.Defend(damageAmount, true, Constants.POISON_COLOR); // 调用角色受伤方法
        }
        base.OnRoundEnd(character); // 减少层数
        buffDetail = "回合结束时受到<color=#8470FF>" + damagePerLayer * stackLayers + "</color>点伤害";

    }

    public override void Refresh(Buff newBuff)
    {
        stackLayers += newBuff.stackLayers; // 叠加层数
        buffDetail = "回合结束时受到<color=#8470FF>" + damagePerLayer * stackLayers + "</color>点伤害";
    }
}