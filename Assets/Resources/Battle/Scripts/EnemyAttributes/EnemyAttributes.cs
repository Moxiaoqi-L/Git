using UnityEngine;

// 创建一个 ScriptableObject 类来存储英雄属性
[CreateAssetMenu(fileName = "新敌人属性", menuName = "敌人属性")]
public class EnemyAttributes : ScriptableObject
{
    // 敌人姓名
    public string enemyName = "Default";
    // 等级
    public int level = 1;
    // 最大等级             45/65/90
    public int maxLevel = 45;
    // 等阶
    public int rank = 0;
    // 最大等阶
    public int maxRank = 2;

    // 攻击力
    public int attack;
    // 伤害类型
    public string damageType;
    // 暴击几率
    public float criticalRate = 0;
    // 暴击伤害
    public float criticalDamageMultiplier = 0;
    // 命中率
    public int accuracy = 100;
    // 技能增幅
    public float skillPower = 0;
    // 伤害增幅
    public float damagePower = 0;

    // 防御力
    public int defense;
    // 魔法防御力
    public int magicDefense;
    // 闪避率
    public int evasion = 0;

    // 最大生命值
    public int maxHealthPoints;

    // 攻击力成长
    public float attackGrowth;
    // 防御力增长
    public float defenseGrowth;
    // 最大生命值成长
    public float maxHealthPointsGrowth;

    // 被动技能
    // TODO

    // 主动技能
    // TODO

    // 大招
    // TODO

    // 根据等级初始化属性
    public void InitAttributes()
    {
        maxLevel = 45 + (rank >= 1 ? 20 : 0) + (rank >= 2 ? 30 : 0);

        attack += (int)(level * attackGrowth + (rank >= 1 ? 45 : 0) * attackGrowth + (rank >= 2 ? 65 : 0) * attackGrowth);
        defense += (int)(level * defenseGrowth + (rank >= 1 ? 45 : 0) * defenseGrowth + (rank >= 2 ? 65 : 0) * defenseGrowth);
        maxHealthPoints += (int)(level * maxHealthPointsGrowth + (rank >= 1 ? 45 : 0) * maxHealthPointsGrowth + (rank >= 2 ? 65 : 0) * maxHealthPointsGrowth);
    }
}    