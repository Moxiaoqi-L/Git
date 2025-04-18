using System.Collections.Generic;
using UnityEngine;

// 创建一个 ScriptableObject 类来存储英雄属性
[CreateAssetMenu(fileName = "新英雄属性", menuName = "创建英雄属性")]
public class HeroAttributes : CharacterAttributes
{
    // 经验
    public int xp = 0;
    // 最大经验
    public int maxXp = 100;

    // 根据等级初始化属性
    public override void InitAttributes()
    {
        maxLevel = 45 + (rank >= 1 ? 20 : 0) + (rank >= 2 ? 30 : 0);
        maxXp = level * 100 + (rank >= 1 ? 45 : 0) * 100 + (rank >= 2 ? 65 : 0) * 100;

        attack += (int)(level * attackGrowth + (rank >= 1 ? 45 : 0) * attackGrowth + (rank >= 2 ? 65 : 0) * attackGrowth);
        defense += (int)(level * defenseGrowth + (rank >= 1 ? 45 : 0) * defenseGrowth + (rank >= 2 ? 65 : 0) * defenseGrowth);
        maxHealthPoints += (int)(level * maxHealthPointsGrowth + (rank >= 1 ? 45 : 0) * maxHealthPointsGrowth + (rank >= 2 ? 65 : 0) * maxHealthPointsGrowth);  
    }
}    