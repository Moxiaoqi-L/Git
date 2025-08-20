using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    // 管理英雄 Skill 的对象
    public SkillManager skillManager;
    // 攻击动画
    public DefaulAttackAnimation attackAnimation;
    // 拥有的主动技能
    public List<Skill> activeSkill => skillManager.activeSkills;
    // 拥有的被动技能
    public List<Skill> passiveSkill => skillManager.passiveSkills;

    // 获取头像
    public Image image;
    // 攻击文字预制体
    public GameObject damageNumberPrefab;

    // 通过characterAttributes获取攻击范围
    public List<Location> attackRange;
    // 通过characterAttributes获取移动范围
    public List<Location> moveRange;

    // 该回合是否已经攻击
    public bool hasAttacked = false;
    // 禁止攻击
    public bool cantAttack = false;
    // 眩晕状态
    public bool isStunned = false;
    // 禁止使用技能
    public bool cantUseSkills = false;

    // 缓存立绘, 头像, 技能图片等
    public Sprite damageTypeImage;
    public Sprite characterImage;
    public Sprite avatarImage;
    public Sprite activeSkillImage;
    public Sprite passiveSkillImage;

    // 事件管理器实例
    public CharacterEventManager EventManager { get; private set; }

    protected void Start()
    {
        // 获取棋子自身
        chessman = GetComponent<Chessman>();
        // 初始化 BUFF 管理器
        buffManager = new BuffManager(this);
        // 初始化 Skill 管理器
        skillManager = new SkillManager(this);
        // 初始化事件管理器
        EventManager = new CharacterEventManager(this);
        // 初始化属性
        characterAttributes.InitAttributes();
        // 默认使用简单攻击动画
        attackAnimation = new DefaulAttackAnimation();
        // 图片缓存
        damageTypeImage = Resources.Load<Sprite>("General/Image/DamagetypeImage/" + characterAttributes.damageType);
        // 初始化攻击范围
        attackRange = new List<Location>(characterAttributes.attackRange);
        // 初始化移动范围
        moveRange = new List<Location>(CharacterAttributes.MoveRange[characterAttributes.moveRangeType]);
        // cantUseSkills
        cantUseSkills = false;
    }

    // 初始化英雄的技能列表
    protected virtual void InitializeSkills()
    {
        foreach (string activeSkill in characterAttributes.activeSkills)
        {
            skillManager.AddSkill(activeSkill);
        }
        foreach (string passiveSkill in characterAttributes.passiveSkills)
        {
            skillManager.AddSkill(passiveSkill);
        }
    }

    // 获取攻击范围
    public List<Location> GetAttackRange()
    {
        List<Location> attackLocations = new List<Location>();
        Location currentLocation = chessman.location;
        
        foreach (var location in attackRange)
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

    // 初始化人物属性
    public void CharacterAttributesInit()
    {

    }

    // 从坐标获取攻击范围
    public List<Location> GetAttackRangeFromLocation(Location origin)
    {
        List<Location> attackLocations = new List<Location>();
        foreach (var location in attackRange)
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

    /// <summary>
    /// 通用防御方法，用于处理受到的伤害
    /// </summary>
    /// <param name="incomingDamage">即将承受的伤害</param>
    /// <param name="damageType">伤害类型</param>
    /// <param name="ignoreDefense">是否忽略防御</param>
    /// <param name="color">展示伤害动画的颜色</param>
    /// <param name="from">伤害来源</param>
    public virtual void Defend(float incomingDamage, DamageType damageType, bool ignoreDefense = false, Color color = new(), BasicCharacter from = null)
    {
        float actualDamage;
        if (ignoreDefense)
        {
            actualDamage = incomingDamage;
        }
        else
        {
            // 伤害计算（物理）：最少造成 10% 即将承受的伤害。最终伤害 = 到来伤害 * ( 承伤修改 / 100 ) - 防御
            if (damageType == DamageType.Physical) actualDamage = Mathf.Max(incomingDamage * 0.1f, incomingDamage * (GetActualDamageTakenMultiplier() / 100f) - GetActualDefense());
            // 伤害计算（精神）：最少造成 10% 即将承受的伤害。最终伤害 = 到来伤害 * ( 承伤修改 / 100 ) * ((100 - 精神防御) / 100);
            else if (damageType == DamageType.Mental) actualDamage = Mathf.Max(incomingDamage * 0.1f, incomingDamage * (GetActualDamageTakenMultiplier() / 100f) * ((100 - GetActualMagicDefense()) / 100f));
            // 其他情况
            else actualDamage = incomingDamage;
        }
        // 受伤震动
        GetDamageShake();

        // 受伤前事件触发
        actualDamage = EventManager.TriggerBeforeTakeDamage(actualDamage, damageType);

        // 展示伤害动画
        ShowDamageNumber((int)actualDamage, color);
        // 被击中事件
        if (from != null) EventManager.TriggerAttackedBy(from);
        // 血量减少
        currentHealthPoints -= (int)actualDamage;
        // 防止血量削减为 0 
        if (currentHealthPoints <= 0) currentHealthPoints = 0;
        // 受伤事件
        EventManager.TriggerHealthPointsChanged();
        if (currentHealthPoints <= 0)
        {
            // 死亡回调
            OnDeath();
            chessman.ExitFromBoard();
        }
        // 受伤台词
        if (!ignoreDefense) TriggerLine(LineEventType.Defense);
    }

    // 增加攻击力的方法
    public override void IncreaseAttack(int amount)
    {
        base.IncreaseAttack(amount);
        EventManager.TriggerAttackChanged();
    }

    // 增加防御力的方法
    public override void IncreaseDefense(int amount)
    {
        base.IncreaseDefense(amount);
        EventManager.TriggerDefenseChanged();
    }

    // 增加生命值的方法
    public void IncreaseHealthPoints(int amount, Color color = new())
    {
        if (amount > 0) EventManager.TriggerBeforeHealing(ref amount);
        ShowDamageNumber(amount, color);
        // 增加英雄的生命值
        currentHealthPoints += amount;
        currentHealthPoints = currentHealthPoints > characterAttributes.maxHealthPoints ? characterAttributes.maxHealthPoints : currentHealthPoints;
        // 血量变化事件
        EventManager.TriggerHealthPointsChanged();
        // 触发治疗后事件
        EventManager.TriggerAfterHealing(amount);
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
        else if (eventType == LineEventType.Death)
        {
            triggerChance = 1f; // Death台词100%概率
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
                lineText: "「" + line.lineText + "」",
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
                StartCoroutine(MoveAndFade(damageNumber, new Vector3(0, 60, 0), 60));
            }
        }
    }

    public void ShowText(string text, Color color = new())
    {
        if (damageNumberPrefab != null)
        {
            // 获取 Canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                // 实例化伤害数字对象
                GameObject textObj = Instantiate(damageNumberPrefab, this.transform);
                // 获取 Text 组件并设置伤害值
                TextMeshProUGUI damageText = textObj.GetComponent<TextMeshProUGUI>();
                if (damageText != null)
                {
                    damageText.text = text;
                }
                if (color != new Color()) damageText.color = color;
                // 让数字向上移动并逐渐消失,使用一个协程来实现
                StartCoroutine(MoveAndFade(textObj, new Vector3(0, 20, 0), 15));
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
    protected IEnumerator MoveAndFade(GameObject text, Vector3 vector3, int moveUp)
    {
        float duration = 0.6f; // 动画持续时间
        float elapsedTime = 0f;
        Vector3 startPosition = this.transform.position + vector3;
        Vector3 endPosition = startPosition + Vector3.up * moveUp; // 向上移动 1 个单位

        CanvasGroup canvasGroup = text.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = text.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 1f;
        while (elapsedTime < duration)
        {
            // 移动文字
            text.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            // 逐渐消失
            canvasGroup.alpha = 1.2f - (elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // 销毁数字对象
        Destroy(text);
    }

    // 对象被摧毁时处理的函数
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    
}    