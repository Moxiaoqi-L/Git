using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class MethodsForSkills : MonoBehaviour
{
    // 查找与英雄相同列的所有敌人
    public static List<Enemy> GetEnemiesInSameColumn(Hero hero)
    {
        List<Enemy> enemiesInSameColumn = new();
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

    // 查找与敌人相同列的所有英雄
    public static List<Hero> GetHeroInSameColumn(Enemy enemy)
    {
        List<Hero> heroesInSameColumn = new List<Hero>();
        int enemyColumn = enemy.chessman.location.x;

        foreach (Hero hero in FindObjectsOfType<Hero>())
        {
            int heroColumn = hero.chessman.location.x;
            if (heroColumn == enemyColumn)
            {
                heroesInSameColumn.Add(hero);
            }
        }
        return heroesInSameColumn;
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
    // 获取前方棋格上的Hero
    public static Hero GetFrontHero(Location currentLocation)
    {
        Location frontLocation = new(currentLocation.x, currentLocation.y + 1);
        return GetHeroAtLocation(frontLocation);
    }

    // 获取后方棋格上的Hero
    public static Hero GetBackHero(Location currentLocation)
    {
        Location backLocation = new(currentLocation.x, currentLocation.y - 1);
        return GetHeroAtLocation(backLocation);
    }

    // 获取左方棋格上的Hero
    public static Hero GetLeftHero(Location currentLocation)
    {
        Location leftLocation = new(currentLocation.x - 1, currentLocation.y);
        return GetHeroAtLocation(leftLocation);
    }

    // 获取右方棋格上的Hero
    public static Hero GetRightHero(Location currentLocation)
    {
        Location rightLocation = new(currentLocation.x + 1, currentLocation.y);
        return GetHeroAtLocation(rightLocation);
    }

    // 获取范围内的所有Hero
    public static List<Hero> GetHeroesInRange(BasicCharacter character, List<Location> locations)
    {
        List<Hero> heroesInRange = new();

        foreach (Location location in locations)
        {
            Location targetLocation = new(
                character.chessman.location.x + location.x,
                character.chessman.location.y + location.y
            );
            Square targetSquare = BoardCtrl.Get[targetLocation];
            if (targetSquare != null && targetSquare.Chessman != null && targetSquare.Chessman.camp == Camp.Player)
            {
                heroesInRange.Add(targetSquare.Chessman.hero);
            }
        }

        return heroesInRange;
    }



    // 根据位置获取Hero
    private static Hero GetHeroAtLocation(Location location)
    {
        Square targetSquare = BoardCtrl.Get[location];
        if (targetSquare != null && targetSquare.Chessman != null && targetSquare.Chessman.camp == Camp.Player)
        {
            return targetSquare.Chessman.hero;
        }
        return null;
    }
}
