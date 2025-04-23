using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMover2D : MonoBehaviour
{
    public float speed = 15f;
    public float hitOffset = 0f;
    public bool useFirePointRotation;
    public float rotationOffsetZ = 0f; // 2D仅需要Z轴旋转偏移
    public GameObject hit;
    public GameObject flash;
    private Rigidbody2D rb;
    public GameObject[] detached;

    private Transform target; // 新增：目标引用（可通过构造函数或公共方法设置）

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (flash != null)
        {
            var flashInstance = Instantiate(flash, transform.position, Quaternion.identity);
            flashInstance.transform.right = transform.right; // 设置2D方向
            var flashPs = flashInstance.GetComponent<ParticleSystem>();
            if (flashPs != null)
            {
                Destroy(flashInstance, flashPs.main.duration);
            }
            else if (flashInstance.transform.childCount > 0)
            {
                var flashPsParts = flashInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(flashInstance, flashPsParts.main.duration);
            }
        }
        Destroy(gameObject, 5);
    }

    void FixedUpdate()
    {
        if (speed != 0)
        {
            rb.velocity = transform.right * speed; // 2D使用right方向
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 锁定2D刚体运动
        rb.constraints = RigidbodyConstraints2D.FreezeAll; 
        speed = 0;

        ContactPoint2D contact = collision.GetContact(0);
        Vector2 normal = contact.normal;
        
        // 计算2D碰撞旋转（从向上方向到法线的角度）
        float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg - 90f; // 转换为Z轴旋转角度
        Quaternion rot = Quaternion.Euler(0, 0, angle);
        
        Vector2 pos = (Vector2)contact.point + normal * hitOffset; // 2D坐标计算

        if (hit != null)
        {
            var hitInstance = Instantiate(hit, pos, rot);
            
            // 处理2D旋转逻辑
            if (useFirePointRotation)
            {
                hitInstance.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, 180f); // 2D旋转仅Z轴
            }
            else if (rotationOffsetZ != 0f)
            {
                hitInstance.transform.rotation = Quaternion.Euler(0, 0, rotationOffsetZ); // 应用Z轴偏移
            }
            else
            {
                hitInstance.transform.right = normal; // 面向碰撞法线方向
            }

            // 处理粒子系统销毁
            var hitPs = hitInstance.GetComponent<ParticleSystem>();
            if (hitPs != null)
            {
                Destroy(hitInstance, hitPs.main.duration);
            }
            else if (hitInstance.transform.childCount > 0)
            {
                var hitPsParts = hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitInstance, hitPsParts.main.duration);
            }
        }

        // 分离子物体
        foreach (var detachedPrefab in detached)
        {
            if (detachedPrefab != null)
            {
                detachedPrefab.transform.SetParent(null);
            }
        }

        Destroy(gameObject);
    }
}