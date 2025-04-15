using UnityEngine;
using System.Collections.Generic;

// 英雄类，代表游戏中的英雄角色，包含英雄的各种属性和行为
public class Enemy : BasicCharacter
{
    // 攻击目标
    public Hero targetHero;

    protected override void InitializeSkills()
    {

    }

    // 当对象启用时调用的方法，用于初始化和执行一些操作
    private new void Start() {
        base.Start();
        // 初始化生命值
        currentHealthPoints = characterAttributes.maxHealthPoints;       
    }

    // 敌方的攻击方法，用于对敌人造成伤害，新增 selfAttack 参数用于控制是否自我攻击
    public void Attack(Enemy self)
    {
        if (isStunned) return;
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
        float hitRate = characterAttributes.accuracy / (characterAttributes.accuracy + targetHero.characterAttributes.evasion);
        float randomValue = Random.value;
        if (randomValue <= hitRate)
        {
            float actualattack = characterAttributes.attack + provisionalAttack;
            bool isCritical = Random.value <= characterAttributes.criticalRate;
            float damage = actualattack;
            if (isCritical)
            {
                damage *= characterAttributes.criticalDamageMultiplier;
                Debug.Log(characterAttributes.name + " 暴击了！ ");
            }
            targetHero.Defend(damage);
        }
        else
        {
            Debug.Log(characterAttributes.name + " 攻击落空！ ");
        }
    }

    // 敌人回合
    public void TakeTurn()
    {
        // 实施攻击
        Attack(this);
    }

    protected override void OnDeath(){}

}    