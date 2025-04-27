using UnityEngine;

// BUFF 基类
public abstract class Buff
{
    public string buffName;         // BUFF 唯一名称
    public Sprite buffSprite;       // BUFF 图片
    public string buffDetail;
    public int duration;            // 持续时间（秒，0 表示永久）
    public int stackLayers;         // 叠加层数

    // 应用 BUFF 效果（子类实现具体逻辑）
    public virtual void Apply(BasicCharacter character)
    {
        buffSprite = Resources.Load<Sprite>("Battle/Image/Buff/" + GetType().ToString());
    }

    // 移除 BUFF 效果（子类实现具体逻辑）
    public abstract void Remove(BasicCharacter character);

    // 刷新 BUFF（叠加层数或持续时间）
    public virtual void Refresh(Buff newBuff)
    {
        stackLayers += newBuff.stackLayers;
        duration = Mathf.Max(duration, newBuff.duration); // 取较长持续时间
    }

    // 回合结束时触发（用于持续回合生效的BUFF）
    public virtual void OnRoundEnd(BasicCharacter character)
    {
        stackLayers = Mathf.Max(0, stackLayers - 1); // 减少层数\
    }
}