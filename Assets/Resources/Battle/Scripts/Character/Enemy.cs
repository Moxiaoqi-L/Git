using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using System.Collections;

// 英雄类，代表游戏中的英雄角色，包含英雄的各种属性和行为
public class Enemy : BasicCharacter
{
    // 攻击目标
    public Hero targetHero;

    // 当对象启用时调用的方法，用于初始化和执行一些操作
    private new void Start() {
        base.Start();
        // 初始化生命值
        currentHealthPoints = characterAttributes.maxHealthPoints;       
    }

    // 敌方的攻击方法，用于对敌人造成伤害，新增 selfAttack 参数用于控制是否自我攻击
    public void Attack(Enemy self)
    {
        // 眩晕不攻击
        if (isStunned) return;
        // 获取当前攻击范围
        List<Location> attackRange = GetAttackRange();
        // 检查攻击范围内是否有Hero
        List<Hero> heroesInRange = Chessman.AllHeros().Where(hero => 
            attackRange.Contains(hero.chessman.location)).ToList();
        // 要攻击的英雄
        Hero targetHero = null;
    
    // 若攻击范围内有 Hero，选择同列前排
    // 若攻击范围内无 Hero，自动移动到可攻击位置
    if (heroesInRange.Count > 0)
    {
        // 优先选择同列前排，否则选最近
        int enemyColumn = chessman.location.x;
        int maxY = 0;
        foreach (Hero hero in heroesInRange)
        {
            if (hero.chessman.location.x == enemyColumn && hero.chessman.location.y > maxY)
            {
                targetHero = hero;
                maxY = hero.chessman.location.y;
            }
        }
        ExecuteAttack(targetHero);
    }
    else
    {
        // 寻找所有可移动位置（假设Enemy可移动到任意空的相邻格子，可根据需求调整）
        List<Location> validMoveLocations = GetValidMoveLocations();
        // 筛选移动后攻击范围内有Hero的位置
        List<Location> effectiveMoveLocations = validMoveLocations.Where(location => 
                GetAttackRangeFromLocation(location).Any(heroLoc => 
                Chessman.GetChessman(heroLoc)?.camp == Camp.Player)).ToList(); 
        if (effectiveMoveLocations.Count > 0)
        {
            // 选择距离当前位置最近的可移动位置
            Location nearestLocation = effectiveMoveLocations.OrderBy(loc => 
                Mathf.Abs(loc.x - chessman.location.x) + Mathf.Abs(loc.y - chessman.location.y)).First();
            // 移动Enemy到目标位置（需实现移动逻辑，此处假设使用棋盘格子移动）
            MoveToLocation(nearestLocation);
            // 停顿
            StartCoroutine(WaitAndAttack(nearestLocation, 0.8f)); 
        }
    }
        // 若仍无目标，结束攻击
        if (targetHero == null) return;
    }

    // 协程：延迟后执行攻击
    private IEnumerator WaitAndAttack(Location moveLocation, float delay)
    {
        yield return new WaitForSeconds(delay); // 停顿指定时间
        
        // 重新获取移动后的攻击范围和目标
        List<Location> newAttackRange = GetAttackRangeFromLocation(moveLocation);
        List<Hero> newHeroesInRange = Chessman.AllHeros().Where(hero => 
            newAttackRange.Contains(hero.chessman.location)).ToList();
        
        if (newHeroesInRange.Count > 0)
        {
            Hero targetHero = newHeroesInRange.OrderBy(hero => 
                Vector3.Distance(transform.position, hero.transform.position)).First();
            ExecuteAttack(targetHero);
        }
    }

    // 命令进攻
    private void ExecuteAttack(Hero targetHero)
    {
        Debug.Log(targetHero);
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
            targetHero.AddBuff("眩晕", 1);
        }
        else
        {
            Debug.Log(characterAttributes.name + " 攻击落空！ ");
        }
    }

    // 获取Enemy的有效移动位置（示例：相邻8格且为空）
    private List<Location> GetValidMoveLocations()
    {
        List<Location> validLocations = new List<Location>();
        Location current = chessman.location;
        // 检查周围8个方向（可根据需求调整移动范围）
        for (int x = -2; x <= 2; x++)
        {
            for (int y = -2; y <= 2; y++)
            {
                if (x == 0 && y == 0) continue; // 排除自身位置
                Location newLoc = new Location(current.x + x, current.y + y);
                if (newLoc.IsValid() && BoardCtrl.Get[newLoc].Chessman == null) // 位置有效且无棋子
                {
                    validLocations.Add(newLoc);
                }
            }
        }
        return validLocations;
    }

    // 移动Enemy到目标位置
    private void MoveToLocation(Location targetLocation)
    {
        Square targetSquare = BoardCtrl.Get[targetLocation];
        if (targetSquare == null) return;
        // 实例化预制体（类似GameInit中的Enemy生成逻辑）
        transform.SetParent(targetSquare.transform);
        chessman.location = targetLocation;
        // 播放移动动画（可使用DOTween实现平滑移动）
        transform.DOMove(targetSquare.transform.position, 0.5f);
    }

    // 敌人回合
    public void TakeTurn()
    {
        // 实施攻击
        Attack(this);
    }

    protected override void OnDeath(){}

}    