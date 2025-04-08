using UnityEngine;
using System.Collections.Generic;

// 英雄类，代表游戏中的英雄角色，包含英雄的各种属性和行为
public class Enemy : BasicCharacter
{
    // 英雄的属性，使用 ScriptableObject 存储，可在编辑器中配置
    public EnemyAttributes enemyAttributes;

    protected override void InitializeSkills()
    {

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
        currentHealthPoints = enemyAttributes.maxHealthPoints;                    
    }

    // 英雄的攻击方法，用于对敌人造成伤害，新增 selfAttack 参数用于控制是否自我攻击
    public void Attack(Hero target, Enemy self)
    {
        // 计算命中率
        float hitRate = enemyAttributes.accuracy / (enemyAttributes.accuracy + target.heroAttributes.evasion);
        float randomValue = Random.value;
        if (randomValue <= hitRate)
        {
            float actualattack = enemyAttributes.attack + enemyAttributes.attack * buffManager.GetTotalAttackBuff();
            bool isCritical = Random.value <= enemyAttributes.criticalRate;
            float damage = actualattack;
            if (isCritical)
            {
                damage *= enemyAttributes.criticalDamageMultiplier;
                Debug.Log(enemyAttributes.name + " 暴击了！ ");
            }
            target.Defend(damage);
        }
        else
        {
            Debug.Log(enemyAttributes.name + " 攻击落空！ ");
        }
    }

    // 防御方法，用于处理受到的伤害
    public void Defend(float incomingDamage)
    {
        float actualDamage = Mathf.Max(0, incomingDamage - enemyAttributes.defense);
        currentHealthPoints -= (int) actualDamage;
        if (currentHealthPoints < 0)
        {
            currentHealthPoints = 0;
            Debug.Log(enemyAttributes.name + " 鼠掉了 ");
            chessman.ExitFromBoard();
            return;
        }
        Debug.Log(enemyAttributes.name + " 受到伤害！ ");
    }

    // 增加英雄生命值的方法
    public void IncreaseHealthPoints(int amount)
    {
        // 增加英雄的生命值
        currentHealthPoints += amount;
        currentHealthPoints = currentHealthPoints > enemyAttributes.maxHealthPoints ? enemyAttributes.maxHealthPoints : currentHealthPoints;
    }

    // 每回合结束时调用，处理 BUFF 的剩余回合数
    public void EndOfRound()
    {
        buffManager.EndOfRound();
    }
}    