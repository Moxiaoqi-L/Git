using UnityEngine;
using System.Collections.Generic;

// 英雄类，代表游戏中的英雄角色，包含英雄的各种属性和行为
public class Enemy : BasicCharacter
{
    // 敌人的属性，使用 ScriptableObject 存储，可在编辑器中配置
    public EnemyAttributes enemyAttributes;
    // 攻击目标
    public Hero targetHero; 

    protected override void InitializeSkills()
    {

    }

    // 当对象启用时调用的方法，用于初始化和执行一些操作
    private new void Start() {
        base.Start();
        // 初始化生命值
        currentHealthPoints = enemyAttributes.maxHealthPoints;       
    }

    // 敌方的攻击方法，用于对敌人造成伤害，新增 selfAttack 参数用于控制是否自我攻击
    public void Attack(Enemy self)
    {
        // 获取所有 Hero 棋子
        List<Chessman> allHeroes = Chessman.All(Camp.Player);
        // 将要攻击的目标英雄
        Hero targetHero = null;
        // 如果没有能攻击的英雄就中断
        if (allHeroes.Count == 0)
        {
            Debug.Log("没有可攻击的英雄！");
            return;
        }

        // 查找当前列的英雄
        foreach (Chessman heroChessman in allHeroes)
        {
            Hero hero = heroChessman.hero;
            if (hero != null && heroChessman.location.x == chessman.location.x)
            {
                targetHero = hero;
                break;
            }
        }

        // 找到距离自己最近的 Hero，如果 targetHero 存在就跳过这步
        if (targetHero == null)
        {
            float closestDistance = float.MaxValue;
            foreach (Chessman heroChessman in allHeroes)
            {
                Hero hero = heroChessman.hero;
                if (hero != null)
                {
                    float distance = Vector3.Distance(transform.position, hero.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        targetHero = hero;
                    }
                }
            }
        }

        // 攻击动画
        attackAnimation.PlayAttackAnimation(this.transform, targetHero.transform, true);

        // 计算命中率
        float hitRate = enemyAttributes.accuracy / (enemyAttributes.accuracy + targetHero.heroAttributes.evasion);
        float randomValue = Random.value;
        if (randomValue <= hitRate)
        {
            float actualattack = enemyAttributes.attack + enemyAttributes.attack * buffManager.GetTotalAttackBuff() + provisionalAttack;
            bool isCritical = Random.value <= enemyAttributes.criticalRate;
            float damage = actualattack;
            if (isCritical)
            {
                damage *= enemyAttributes.criticalDamageMultiplier;
                Debug.Log(enemyAttributes.name + " 暴击了！ ");
            }
            targetHero.Defend(damage);
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
        // 展示伤害动画
        ShowDamageNumber((int)actualDamage);
        // 受伤震动
        GetDamageShake();
        currentHealthPoints -= (int) actualDamage;
        if (currentHealthPoints < 0)
        {
            currentHealthPoints = 0;
            Debug.Log(enemyAttributes.name + " 鼠掉了 ");
            chessman.ExitFromBoard();
            // 检查是否还有其他敌人存活
            if (Chessman.AllEnemies().Count == 0)
            {
                // 通知 GameInit 进入下一阶段
                GameInit.Instance.NextStep();
                Debug.Log("进入下一阶段");
            }
            
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

    // 每回合结束时调用
    public void EndOfRound()
    {
        // 处理 BUFF 的剩余回合数
        buffManager.EndOfRound();
    }

    // 敌人回合
    public void TakeTurn()
    {
        // 实施攻击
        Attack(this);
        EndOfRound();
    }

}    