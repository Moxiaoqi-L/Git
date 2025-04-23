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
        Object.Instantiate(MissilePrefabs.Get.Prefabs[0], target.transform);
    // 从 MissilePrefabs 中获取预制体（假设索引 0 为普通攻击 projectile）
        // GameObject projectilePrefab = MissilePrefabs.Get.Prefabs[0];
        // GameObject projectile = GameObject.Instantiate(projectilePrefab, attacker.position, Quaternion.identity);
        
        // // 配置 projectile 方向（指向目标）
        // Vector2 direction = (target.position - attacker.position).normalized;
        // projectile.transform.right = direction; // 2D 方向设置
        
        // // 获取 ProjectileMover2D 组件并设置目标（可选，如需传递额外数据）
        // ProjectileMover2D mover = projectile.GetComponent<ProjectileMover2D>();
        // mover.SetTarget(target); // 自定义方法，用于碰撞时识别目标
    }
}