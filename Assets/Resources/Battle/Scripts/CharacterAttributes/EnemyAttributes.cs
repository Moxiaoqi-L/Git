using UnityEngine;

// 创建一个 ScriptableObject 类来存储英雄属性
[CreateAssetMenu(fileName = "新敌人属性", menuName = "敌人属性")]
public class EnemyAttributes : CharacterAttributes
{
    public override void InitAttributes()
    {
        maxLevel = 45 + (rank >= 1 ? 20 : 0) + (rank >= 2 ? 30 : 0);

        attack += (int)(level * attackGrowth + (rank >= 1 ? 45 : 0) * attackGrowth + (rank >= 2 ? 65 : 0) * attackGrowth);
        defense += (int)(level * defenseGrowth + (rank >= 1 ? 45 : 0) * defenseGrowth + (rank >= 2 ? 65 : 0) * defenseGrowth);
        maxHealthPoints += (int)(level * maxHealthPointsGrowth + (rank >= 1 ? 45 : 0) * maxHealthPointsGrowth + (rank >= 2 ? 65 : 0) * maxHealthPointsGrowth);
    }
}    