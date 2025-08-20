using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public float speed = 1500f;
    public float hitOffset = 0f;
    public GameObject hit;
    public GameObject flash;

    public AudioClip hitClip;

    // 目标位置
    private Vector2 targetPosition;

    private Action action;

    // 新增：设置目标位置的方法
    public void SetTargetPosition(Vector2 position, Action onAnimationComplete = null)
    {
        targetPosition = position;
        action = onAnimationComplete;
    }

    void Update()
    {
        if (speed <= 0) return; // 防御性检查

        // 向目标位置移动（使用Vector2.MoveTowards确保精准到达）
        Vector2 currentPos = transform.position;
        Vector2 newPos = Vector2.MoveTowards(
            currentPos, 
            targetPosition, 
            speed * Time.deltaTime
        );
        transform.position = newPos;

        // 到达目标时触发效果
        if (Vector2.Distance(newPos, targetPosition) < 0.01f)
        {
            TriggerHitEffect(targetPosition); // 调用碰撞时的逻辑
        }
    }

    // 替代原OnCollisionEnter2D的逻辑
    private void TriggerHitEffect(Vector2 impactPosition)
    {
        // 计算碰撞法线（目标位置方向，假设目标为点，法线可设为反方向）
        Vector2 normal = (impactPosition - (Vector2)transform.position).normalized;
        if (normal == Vector2.zero) normal = Vector2.up; // 防零向量

        float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg - 90f;
        Quaternion rot = Quaternion.Euler(0, 0, angle);
        Vector2 pos = impactPosition + normal * hitOffset;

        // 生成Hit效果
        if (hit != null) 
        {
            Instantiate(hit, pos, rot);
            if (hitClip != null) AudioManager.Instance.PlaySFX(hitClip);
        }
        if (flash != null) Instantiate(flash, pos, rot);
        Destroy(gameObject);
    }

    private void OnDestroy() {
        action?.Invoke();
    }
}