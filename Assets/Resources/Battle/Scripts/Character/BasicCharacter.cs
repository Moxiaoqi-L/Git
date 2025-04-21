using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 英雄类，代表游戏中的英雄角色，包含英雄的各种属性和行为
public abstract class BasicCharacter : CharacterProvisionalAttributes
{
    // 获取棋子
    public Chessman chessman;
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
        if (characterAttributes.skills.Length > 0)
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
        // 图片缓存
        damageTypeImage = Resources.Load<Sprite>("General/Image/DamagetypeImage/" + characterAttributes.damageType);
    }

    // 获取攻击范围
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

    // 从坐标获取攻击范围
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
    public virtual void Defend(float incomingDamage, bool ignoreDefense = false, Color color = new())
    {
        float actualDamage;
        if (ignoreDefense) actualDamage = incomingDamage;
        else actualDamage = Mathf.Max(incomingDamage * 0.1f, 
        incomingDamage * (GetActualDamageTakenMultiplier() / 100f) - (characterAttributes.defense + provisionalDefense));
        // 展示伤害动画
        ShowDamageNumber((int)actualDamage, color);
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
        // 受伤台词
        TriggerLine(LineEventType.Defense);
    }

    // 增加攻击力的方法
    public override void IncreaseAttack(int amount)
    {
        base.IncreaseAttack(amount);
        OnAttackChanged?.Invoke(this);
    }

    // 增加防御力的方法
    public override void IncreaseDefense(int amount)
    {
        base.IncreaseDefense(amount);
        OnDefenseChanged?.Invoke(this);
    }

    // 增加生命值的方法
    public void IncreaseHealthPoints(int amount, Color color = new())
    {
        ShowDamageNumber(amount, Constants.HEAL_COLOR);
        // 增加英雄的生命值
        currentHealthPoints += amount;
        currentHealthPoints = currentHealthPoints > characterAttributes.maxHealthPoints ? characterAttributes.maxHealthPoints : currentHealthPoints;
        // 血量变化事件
        OnHealthPointsChanged?.Invoke(this);
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

    // 清除BUFF
    public void ClearAllBuffs()
    {
        buffManager.RemoveAllBuffs(); // 调用BuffManager的清除方法
    }

    // 重置自身状态
    public virtual void RefreshSelf()
    {
        ClearAllBuffs();
        hasAttacked = false; // 重置攻击状态
    }


    // 触发事件台词
    public void TriggerLine(LineEventType eventType)
    {
        // 根据事件类型设置触发概率
        float triggerChance = 0;

        if (eventType == LineEventType.Attack)
        {
            triggerChance = 0.1f; // Attack台词25%概率
        }
        else if (eventType == LineEventType.SkillActive)
        {
            triggerChance = 1f; // Skill台词100%概率
        }
        else if (eventType == LineEventType.Defense)
        {
            triggerChance = 0.05f; // Defense台词100%概率
        }
        // 概率判定
        if (UnityEngine.Random.value > triggerChance) return;

        CharacterLineData line = LineManager.Get.GetRandomLine(characterAttributes.characterName, eventType);
        if (line != null)
        {
            // 解析颜色（处理格式错误）
            Color color = Color.white;
            if (!ColorUtility.TryParseHtmlString(line.textColor, out color))
            {
                Debug.LogWarning($"颜色格式错误：{line.textColor}，使用默认白色");
            }

            // 显示到UI列表（假设已有TextListManager）
            TextListManager.Get.AddLine(
                characterName: characterAttributes.characterName,
                lineText: line.lineText,
                textColor: color
            );
        }
    }

    // 受伤震动动画
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

    // 自己的回合结束
    public virtual void EndOfRound()
    {
        // 处理 BUFF 的剩余回合数
        buffManager.OnCharacterRoundEnd();
    }

    // 死亡回调（子类实现具体逻辑）
    protected virtual void OnDeath()
    {
        TriggerLine(LineEventType.Death);
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
}    