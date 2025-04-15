using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 英雄类，代表游戏中的英雄角色，包含英雄的各种属性和行为
public abstract class BasicCharacter : MonoBehaviour
{
    // 获取棋子
    public Chessman chessman;
    // 管理英雄 BUFF 的对象
    protected BuffManager buffManager;
    // 攻击动画
    public IAttackAnimation attackAnimation;
    // 拥有的技能列表
    public List<Skill> skills = new();

    // 获取头像
    public Image image;

    // 当前生命值
    public int currentHealthPoints;
    // 临时攻击力
    public int provisionalAttack;
    // 临时防御
    public int provisionalDefense;
    
    // 初始化英雄的技能列表
    protected abstract void InitializeSkills();

    protected void Start() {
        // 获取棋子自身
        chessman = GetComponent<Chessman>();
        // 获取头像
        image = GetComponent<Image>();
        // 初始化 BUFF 管理器
        buffManager = new BuffManager();
        // 默认使用简单攻击动画
        attackAnimation = new DefaulAttackAnimation();
    }

    // 增加攻击力的方法
    public void IncreaseAttack(int amount)
    {
        provisionalAttack += amount;
    }

    // 增加防御力的方法
    public void IncreaseDefense(int amount)
    {
        provisionalDefense += amount;
    }

    // 添加 BUFF 的方法，通过 BUFF 名称调用 BuffManager 的添加方法
    public void AddBuff(string buffName, int duration)
    {
        // 调用 BuffManager 的 AddBuff 方法添加 BUFF
        // buffManager.AddBuff(buffName, duration);
    }

    // 移除 BUFF 的方法，通过 BUFF 名称调用 BuffManager 的移除方法
    public void RemoveBuff(string buffName)
    {
        // 调用 BuffManager 的 RemoveAttackBuff 方法移除攻击类 BUFF
        // buffManager.RemoveBuff(buffName);
    }

    // 受伤震动
    protected void GetDamageShake()
    {
        // 让图片震动
        if (image != null)
        {
            // 自定义震动的频率和强度
            float duration = 0.3f; // 震动持续时间
            float strength = 8f;   // 震动强度
            int vibrato = 15;      // 震动频率，每秒震动 15 次
            float randomness = 90f; // 震动方向的随机程度

            image.rectTransform.DOShakeAnchorPos(duration, strength, vibrato, randomness, false, true);
        }
    }

    // 展示受伤伤害动画
    protected void ShowDamageNumber(int damage, bool isHeal = false)
    {
        // 加载伤害数字预制体
        GameObject damageNumberPrefab = Resources.Load<GameObject>("Battle/Prefab/DamageNumber");
        if (damageNumberPrefab != null)
        {
            // 获取 Canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                // 实例化伤害数字对象
                GameObject damageNumber = Instantiate(damageNumberPrefab);
                damageNumber.transform.SetParent(this.transform);
                // 获取 Text 组件并设置伤害值
                TextMeshProUGUI damageText = damageNumber.GetComponent<TextMeshProUGUI>();
                if (isHeal) damageText.color = Constants.HEALCOLOR;
                if (damageText != null)
                {
                    damageText.text = damage.ToString();
                }

                // 让数字向上移动并逐渐消失,使用一个协程来实现
                StartCoroutine(MoveAndFade(damageNumber));
            }
        }
    }

    // 数字向上移动并逐渐消失的协程
    protected IEnumerator MoveAndFade(GameObject damageNumber)
    {
        float duration = 0.6f; // 动画持续时间
        float elapsedTime = 0f;
        Vector3 startPosition = this.transform.position + new Vector3(0 ,50 ,0);
        Vector3 endPosition = startPosition + Vector3.up * 60f; // 向上移动 1 个单位

        CanvasGroup canvasGroup = damageNumber.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = damageNumber.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 1f;

        while (elapsedTime < duration)
        {
            // 移动数字
            damageNumber.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            // 逐渐消失
            canvasGroup.alpha = 1f - (elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 销毁数字对象
        Destroy(damageNumber);
    }
}    