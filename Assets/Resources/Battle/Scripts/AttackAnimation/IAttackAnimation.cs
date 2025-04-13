using UnityEngine;

// 定义攻击动画的接口
public interface IAttackAnimation
{
    // 执行攻击动画的方法，接收攻击者和目标作为参数
    void PlayAttackAnimation(Transform attacker, Transform target, bool reverse = false);
}