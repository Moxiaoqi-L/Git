using UnityEngine;
using DG.Tweening;
using System;

// 英雄类，代表游戏中的英雄角色，包含英雄的各种属性和行为
public class Hero : BasicCharacter
{
    // 英雄的属性，使用 ScriptableObject 存储，可在编辑器中配置
    public HeroAttributes heroAttributes;
    // 英雄的图片
    private ChessmanMove chessmanMove;
    // 每回合只能攻击一次
    public bool hasAttacked;

    protected override void InitializeSkills()
    {
        if (heroAttributes.skills != null)
        {
            foreach (Skill skill in heroAttributes.skills)
            {
                if (skill != null)
                {
                    skills.Add(skill);
                }
            }  
        }
    }

    // 当对象启用时调用的方法，用于初始化和执行一些操作
    private new void Start() {
        base.Start();
        // 初始化技能列表
        InitializeSkills();
        Debug.Log("初始化技能列表:" + skills.Count);
        // 初始化生命值
        currentHealthPoints = heroAttributes.maxHealthPoints;
        // 获取移动方式
        chessmanMove = GetComponent<ChessmanMove>();
        // 初始允许攻击
        hasAttacked = false;          
    }

    // 英雄的攻击方法，用于对敌人造成伤害，新增 selfAttack 参数用于控制是否自我攻击
    public void Attack(Enemy target)
    {
        if (hasAttacked)
        {
            return;
        }
        // 播放攻击动画
        attackAnimation.PlayAttackAnimation(this.transform, target.transform);
        // 计算命中率
        float hitRate = heroAttributes.accuracy / (heroAttributes.accuracy + target.enemyAttributes.evasion);
        float randomValue = UnityEngine.Random.value;
        if (randomValue <= hitRate)
        {
            float actualattack = GetActualAttack();
            bool isCritical = UnityEngine.Random.value <= heroAttributes.criticalRate;
            float damage = actualattack;
            if (isCritical)
            {
                damage *= heroAttributes.criticalDamageMultiplier;
                Debug.Log(heroAttributes.name + " 暴击了！ ");
            }
            Debug.Log(heroAttributes.name + " 攻击了！ ");
            target.Defend(damage);
        }
        else
        {
            Debug.Log(heroAttributes.name + " 攻击落空！ ");
        }
        // 获取攻击点数
        ColorPointCtrl.Get.GetColorPoint(this.transform, this.chessman.location.y);
        // 完成攻击
        FinishAttack();
    }

    // 防御方法，用于处理受到的伤害
    public void Defend(float incomingDamage)
    {
        float actualDamage = Mathf.Max(0, incomingDamage - heroAttributes.defense);
        // 展示伤害动画
        ShowDamageNumber((int)actualDamage);
        // 受伤震动
        GetDamageShake();
        currentHealthPoints -= (int)actualDamage;
        if (currentHealthPoints < 0)
        {
            currentHealthPoints = 0;
            Debug.Log(heroAttributes.name + " 鼠掉了 ");
            chessman.ExitFromBoard();
        }

    }

    public float GetActualAttack(){
        return heroAttributes.attack + heroAttributes.attack * buffManager.GetTotalAttackBuff();
    }

    // 增加英雄生命值的方法
    public void IncreaseHealthPoints(int amount)
    {
        ShowDamageNumber(amount, true);
        // 增加英雄的生命值
        currentHealthPoints += amount;
        currentHealthPoints = currentHealthPoints > heroAttributes.maxHealthPoints ? heroAttributes.maxHealthPoints : currentHealthPoints;
    }

    // 每回合结束时调用，
    public void EndOfRound()
    {
        // 处理 BUFF 的剩余回合数
        buffManager.EndOfRound();
        // 恢复头像
        RestoreImageColor();
        // 设置允许攻击
        hasAttacked = false;
        // 恢复位移
        chessmanMove.enabled = true;
    }

    public void FinishAttack(){
        // 设置已经攻击
        hasAttacked = true;
        // 头像变灰
        MakeImageGray();
        // 禁止位移
        chessmanMove.enabled = false;
    }

    // 图片变灰的方法
    private void MakeImageGray()
    {
        Color grayColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        image.DOColor(grayColor, 0.5f);
    }

    // 图片恢复正常的方法
    public void RestoreImageColor()
    {
        hasAttacked = false; // 重置攻击状态
        Color normalColor = Color.white;
        image.DOColor(normalColor, 0.5f);
    }
}    