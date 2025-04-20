using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 英雄类，代表游戏中的英雄角色，包含英雄的各种属性和行为
public abstract class BasicCharacter : MonoBehaviour
{
    // 获取棋子
    public Chessman chessman;
    // 统一属性基类（替代原Hero/Enemy特有的属性类）
    public CharacterAttributes characterAttributes;
    // 管理英雄 BUFF 的对象
    public BuffManager buffManager;
    // 攻击动画
    public IAttackAnimation attackAnimation;
    // 拥有的主动技能
    public Skill activeSkill;
    // 拥有的被动技能
    public Skill passiveSkill;

    // 获取头像
    public Image image;

    // 当前生命值
    public int currentHealthPoints;
    // 临时攻击力
    public int provisionalAttack;
    // 临时防御
    public int provisionalDefense;

    // 通过characterAttributes获取攻击范围
    public Location[] AttackRange => characterAttributes.attackRange;

    // 每回合只能攻击一次
    public bool hasAttacked;
    // 眩晕状态
    public bool isStunned = false;

    // 缓存立绘, 头像, 技能图片等
    public Sprite damageTypeImage;
    public Sprite characterImage;
    public Sprite avatarImage;
    public Sprite activeSkillImage;
    public Sprite passiveSkillImage;

    // 监听血量变化事件
    public event Action<BasicCharacter> OnHealthPointsChanged;
    // 监听防御变化事件
    public event Action<BasicCharacter> OnDefenseChanged;
    // 监听攻击变化事件
    public event Action<BasicCharacter> OnAttackChanged;

    
    // 初始化英雄的技能列表
    protected virtual void InitializeSkills()
    {
        if (characterAttributes.skills != null)
        {
            activeSkill = characterAttributes.skills[0];
        }
        
        passiveSkill = characterAttributes.passiveSkill;
    }

    protected void Start() {
        // 获取棋子自身
        chessman = GetComponent<Chessman>();
        // 初始化 BUFF 管理器
        buffManager = new BuffManager(this);
        // 默认使用简单攻击动画
        attackAnimation = new DefaulAttackAnimation();

        // 缓存图片
        characterImage = Resources.Load<Sprite>("General/Image/CharacterImage/" + characterAttributes.characterImage);
        damageTypeImage = Resources.Load<Sprite>("General/Image/DamagetypeImage/" + characterAttributes.damageType);
        passiveSkillImage = Resources.Load<Sprite>("General/Image/PassiveSkillImage/" + characterAttributes.passiveSkillImage);
    }

    public List<Location> GetAttackRange()
    {
        List<Location> attackLocations = new List<Location>();
        Location currentLocation = chessman.location;
        
        foreach (var location in characterAttributes.attackRange)
        {
            Location targetLocation = new Location(
                currentLocation.x + location.x,
                currentLocation.y + location.y
            );
            if (targetLocation.IsValid())
            {
                attackLocations.Add(targetLocation);
            }
        }
        return attackLocations;
    }

    public List<Location> GetAttackRangeFromLocation(Location origin)
    {
        List<Location> attackLocations = new List<Location>();
        foreach (var location in characterAttributes.attackRange)
        {
            Location targetLocation = new Location(
                origin.x + location.x,
                origin.y + location.y
            );
            if (targetLocation.IsValid())
            {
                attackLocations.Add(targetLocation);
            }
        }
        return attackLocations;
    }

    // 通用防御方法，用于处理受到的伤害
    public virtual void Defend(float incomingDamage, bool ignoreDefense = false)
    {
        float actualDamage;
        if (ignoreDefense) actualDamage = incomingDamage;
        else actualDamage = Mathf.Max(0, incomingDamage - (characterAttributes.defense + provisionalDefense));
        // 展示伤害动画
        ShowDamageNumber((int)actualDamage);
        // 受伤震动
        GetDamageShake();
        currentHealthPoints -= (int)actualDamage;
        if (currentHealthPoints <= 0) currentHealthPoints = 0;
        // 受伤事件
        OnHealthPointsChanged?.Invoke(this);
        if (currentHealthPoints <= 0)
        {
            // 死亡回调  TODO 死亡事件
            OnDeath();
            chessman.ExitFromBoard();
        }

    }
    // 增加攻击力的方法
    public void IncreaseAttack(int amount)
    {
        provisionalAttack += amount;
        OnAttackChanged?.Invoke(this);
    }

    // 增加防御力的方法
    public void IncreaseDefense(int amount)
    {
        provisionalDefense += amount;
        OnDefenseChanged?.Invoke(this);
    }

    // 增加生命值的方法
    public void IncreaseHealthPoints(int amount)
    {
        ShowDamageNumber(amount, Constants.HEAL_COLOR);
        // 增加英雄的生命值
        currentHealthPoints += amount;
        currentHealthPoints = currentHealthPoints > characterAttributes.maxHealthPoints ? characterAttributes.maxHealthPoints : currentHealthPoints;
        // 血量变化事件
        OnHealthPointsChanged?.Invoke(this);
    }

    // 获取实际攻击力
    public int GetActualAttack()
    {
        return characterAttributes.attack + provisionalAttack;
    }

    // 获取实际防御力
    public int GetActualDefense()
    {  
        return characterAttributes.defense + provisionalDefense;
    }

    // 获取实际精神抗性
    public int GetActualMagicDefense()
    {
        return characterAttributes.magicDefense;
    }

    // 获取当前 HP
    public int GetActualHP()
    {
        return currentHealthPoints;
    }

    // 获取实际最大 HP
    public int GetActualMaxHP()
    {
        return characterAttributes.maxHealthPoints;
    }


    // BUFF 添加方法
    public void AddBuff(string buffName, params object[] args)
    {
        buffManager.AddBuff(buffName, args);
    }

    // BUFF 移除方法
    public void RemoveBuff(string buffName)
    {
        buffManager.RemoveBuff(buffName);
    }

    // 重置自身状态
    public void ClearAllBuffs()
    {
        buffManager.RemoveAllBuffs(); // 调用BuffManager的清除方法
    }

    public virtual void RefreshSelf()
    {
        ClearAllBuffs();
        hasAttacked = false; // 重置攻击状态
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
    protected void ShowDamageNumber(int damage, Color color = new())
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
                if (damageText != null)
                {
                    damageText.text = damage.ToString();
                }
                if (color != new Color()) damageText.color = color;
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
            canvasGroup.alpha = 1.2f - (elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 销毁数字对象
        Destroy(damageNumber);
    }

    public virtual void EndOfRound()
    {
        // 处理 BUFF 的剩余回合数
        buffManager.OnCharacterRoundEnd();
    }

    // 死亡回调（子类实现具体逻辑）
    protected abstract void OnDeath();
}    