// Battle/Scripts/Buff/Buffs/StunBuff.cs
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StunBuff : Buff
{
    private Sequence stunTweener;
    private Color originalColor; // 保存原始颜色

    public StunBuff(int initialLayers)
    {
        buffName = "<color=#FF0000>眩晕</color>";
        buffDetail = "禁止攻击一回合";
        stackLayers = initialLayers; // 持续1回合（层数1）
        duration = 0; // 无固定持续时间，依赖回合结束移除
    }

    public override void Apply(BasicCharacter character)
    {
        base.Apply(character);
        originalColor = character.image.color; // 初始化时保存原始颜色
        PlayStunAnimation(character.image);
        character.isStunned = true;
        // 禁止移动
        if (character is Hero hero) // 确保是英雄对象
        {
            hero.chessmanMove.enabled = false; // 禁用移动组件
        }
    }

    public override void Remove(BasicCharacter character)
    {
        StopStunAnimation(character.image);
        // 移除时：清除眩晕状态
        character.isStunned = false;
        // 允许移动
        if (character is Hero hero) // 确保是英雄对象
        {
            hero.chessmanMove.enabled = true; // 启用移动组件
        }
    }

    public override void OnRoundEnd(BasicCharacter character)
    {
        // 回合结束时自动减少层数（基类已实现 stackLayers--）
        base.OnRoundEnd(character);
    }

    public override void Refresh(Buff newBuff)
    {
        // 可叠加眩晕回合（如层数+1则多禁止一回合）
        stackLayers += newBuff.stackLayers;
        buffDetail = $"禁止攻击 {stackLayers} 回合";
    }
    // 播放眩晕动画（在被施加眩晕Buff时调用）
    public void PlayStunAnimation(Image image)
    {
        if (stunTweener != null && stunTweener.IsPlaying()) return; // 避免重复播放

        // 定义动画序列：旋转 + 颜色闪烁
        stunTweener = DOTween.Sequence()
            .Append(image.rectTransform.DORotate(new Vector3(0, 0, 15), 0.2f, RotateMode.Fast)) // 顺时针旋转30度
            .Append(image.rectTransform.DORotate(new Vector3(0, 0, -15), 0.4f, RotateMode.Fast)) // 逆时针旋转60度（总回正）
            .Append(image.rectTransform.DORotate(new Vector3(0, 0, 0), 0.2f, RotateMode.Fast)) // 回正
            .Join(image.DOColor(Color.red, 0.2f)) // 闪烁红色
            .Join(image.DOColor(originalColor, 0.2f)) // 恢复原色
            .SetLoops(-1, LoopType.Yoyo) // 无限循环，往返播放
            .SetEase(Ease.InOutSine); // 缓动效果
    }

    // 停止眩晕动画（在眩晕Buff移除时调用）
    public void StopStunAnimation(Image image)
    {
        if (stunTweener != null)
        {
            stunTweener.Kill(); // 停止动画
            image.color = originalColor; // 恢复原始颜色
            image.rectTransform.localEulerAngles = Vector3.zero; // 重置旋转
        }
    }

}