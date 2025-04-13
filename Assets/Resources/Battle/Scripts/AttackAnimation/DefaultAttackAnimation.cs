using DG.Tweening;
using UnityEngine;

// 简单攻击动画类，实现 IAttackAnimation 接口
public class DefaulAttackAnimation : IAttackAnimation
{
    // 实现 PlayAttackAnimation 方法，定义简单攻击动画逻辑
    public void PlayAttackAnimation(Transform attacker, Transform target, bool reverse = false)
    {
        if (reverse)
        {
            attacker.DOPunchPosition(new Vector3(0,-50,0), 0.15f, 1, 0.1f);
        }
        else
        {
            attacker.DOPunchPosition(new Vector3(0,50,0), 0.15f, 1, 0.1f);
        }
    }
}