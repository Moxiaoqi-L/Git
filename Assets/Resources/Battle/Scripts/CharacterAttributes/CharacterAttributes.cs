using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 角色属性基类（Hero/Enemy通用）
public abstract class CharacterAttributes : ScriptableObject
{
    // AB包名
    public string AssetBundlePath;
    // 姓名
    public string characterName = "Default";
    // ID
    public string characterID;
    // 立绘
    public string characterImage;
    // 头像
    public string avatarImage;
    // 等级
    public int level = 1;
    // 最大等级             45/65/90
    public int maxLevel = 45;
    // 等阶
    public int rank = 0;

    // 职业类型
    public Job job;

    // 攻击动画
    public string attackAnime = null;

    // 攻击力
    public int attack;
    // 伤害类型
    public DamageType damageType;
    // 暴击几率
    public int criticalRate = 0;
    // 暴击伤害
    public int criticalDamageMultiplier = 150;
    // 技能增幅
    public int skillPower = 0;
    // 伤害增幅
    public int damagePower = 0;


    public int armor = 0;
    // 防御力
    public int defense;
    // 魔法防御力
    public int magicDefense;
    // 承伤修改
    public int damageTakenMultiplier = 100;

    // 最大生命值
    public int maxHealthPoints;

    // 攻击力成长
    public float attackGrowth;
    // 防御力增长
    public float defenseGrowth;
    // 最大生命值成长
    public float maxHealthPointsGrowth;

    // 主动技能
    public List<string> activeSkills = new List<string>{};

    // 被动技能
    public List<string> passiveSkills = new List<string>{};
    // 大招
    // TODO

    // 攻击范围
    public List<Location> attackRange;

    // 初始化属性（子类实现具体逻辑）
    public abstract void InitAttributes();
}

public enum DamageType
{
    // 物理伤害
    Physical,
    // 精神伤害
    Mental,
    // Buff伤害
    Buff
}

public enum Job
{
    // 无职业
    None,
    // 战士
    Warior,
    // 刺客
    Assassin,
    // 狙击
    Sniper,
    // 辅助
    Supporter
}

public enum MoveRangeType
{
    SquareA,
    XA,
    AddA,
    SquareB,
    XB,
    AddB
}


