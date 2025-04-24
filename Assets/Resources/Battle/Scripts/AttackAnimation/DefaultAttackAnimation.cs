using System;
using DG.Tweening;
using UnityEngine;

// 简单攻击动画类，实现 IAttackAnimation 接口
public class DefaulAttackAnimation
{
    // 实现 PlayAttackAnimation 方法，定义简单攻击动画逻辑
    public void PlayAttackAnimation(Transform attacker, Transform target, bool reverse = false, 
                                    string attackType = null, Action onAnimationComplete = null)
    {
        if (attackType != null) 
        {
            MissileManager.Get.Shoot(attacker, target, attackType, onAnimationComplete);
            return;
        }
    }
}