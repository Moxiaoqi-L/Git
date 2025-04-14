using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MethodsForSkills : MonoBehaviour
{
    // 查找与英雄相同列的所有敌人
    public static List<Enemy> GetEnemiesInSameColumn(Hero hero)
    {
        List<Enemy> enemiesInSameColumn = new List<Enemy>();
        int heroColumn = hero.chessman.location.x;

        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            int enemyColumn = enemy.chessman.location.x;
            if (enemyColumn == heroColumn)
            {
                enemiesInSameColumn.Add(enemy);
            }
        }

        return enemiesInSameColumn;
    }

    // 查找距离英雄最近的一个敌人
    public static Enemy GetNearestEnemy(Hero hero)
    {
        Enemy nearestEnemy = null;
        float minDistance = float.MaxValue;
        Vector2 heroPosition = hero.transform.position;

        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            Vector2 enemyPosition = enemy.transform.position;
            float distance = Vector2.Distance(heroPosition, enemyPosition);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }
}
