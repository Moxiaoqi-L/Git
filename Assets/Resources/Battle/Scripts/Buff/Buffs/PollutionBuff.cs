using UnityEngine;

public class PollutionBuff : Buff
{
    // 构造函数初始化Buff基本信息（名称、描述、图标等）
    public PollutionBuff(int initialLayers)
    {
        buffName = "污染";
        buffDetail = "获得的治疗效果降低50%";
        buffSprite = Resources.Load<Sprite>("BuffIcons/Pollution");
        stackLayers = initialLayers;
        duration = 0;
    }

    // 应用Buff时注册治疗事件监听
    public override void Apply(BasicCharacter basicCharacter)
    {
        base.Apply(basicCharacter);
        basicCharacter.OnHealingReceived += ReduceHealing;
    }

    // 移除Buff时取消事件监听
    public override void Remove(BasicCharacter basicCharacter)
    {
        basicCharacter.OnHealingReceived -= ReduceHealing;
    }

    // 治疗量降低逻辑
    private void ReduceHealing(ref int healingAmount)
    {
        healingAmount = (int)(healingAmount * 0.5f); // 治疗量减半
    }
}