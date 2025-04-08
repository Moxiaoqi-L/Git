using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

// 英雄类，代表游戏中的英雄角色，包含英雄的各种属性和行为
public class Hero : BasicCharacter
{
    // 英雄的属性，使用 ScriptableObject 存储，可在编辑器中配置
    public HeroAttributes heroAttributes;

    protected override void InitializeSkills()
    {
        // skills.Add(new AmplifiedAttackSkill());
        // skills.Add(new SelfHealingSkill());
    }

    // 当对象启用时调用的方法，用于初始化和执行一些操作
    private void Start() {
        // 初始化 BUFF 管理器
        buffManager = new BuffManager();
        // 初始化技能列表
        InitializeSkills();
        // 获取棋子自身
        chessman = GetComponent<Chessman>();
        // 初始化生命值
        currentHealthPoints = heroAttributes.maxHealthPoints;          
    }

    // 英雄的攻击方法，用于对敌人造成伤害，新增 selfAttack 参数用于控制是否自我攻击
    public void Attack(Enemy target, Hero self)
    {
        // 计算命中率
        float hitRate = heroAttributes.accuracy / (heroAttributes.accuracy + target.enemyAttributes.evasion);
        float randomValue = Random.value;
        if (randomValue <= hitRate)
        {
            float actualattack = heroAttributes.attack + heroAttributes.attack * buffManager.GetTotalAttackBuff();
            bool isCritical = Random.value <= heroAttributes.criticalRate;
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
    }

    // 防御方法，用于处理受到的伤害
    public void Defend(float incomingDamage)
    {
        float actualDamage = Mathf.Max(0, incomingDamage - heroAttributes.defense);
        currentHealthPoints -= (int)actualDamage;
        if (currentHealthPoints < 0)
        {
            currentHealthPoints = 0;
            Debug.Log(heroAttributes.name + " 鼠掉了 ");
            chessman.ExitFromBoard();
        }
    }

    // 增加英雄生命值的方法
    public void IncreaseHealthPoints(int amount)
    {
        // 增加英雄的生命值
        currentHealthPoints += amount;
        currentHealthPoints = currentHealthPoints > heroAttributes.maxHealthPoints ? heroAttributes.maxHealthPoints : currentHealthPoints;
    }

    // 每回合结束时调用，处理 BUFF 的剩余回合数
    public void EndOfRound()
    {
        buffManager.EndOfRound();
    }

    // 当英雄对象鼠掉调用的方法
    void OnDestroy()
    {

    }
}    