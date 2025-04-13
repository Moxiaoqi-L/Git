using UnityEngine;
using System.IO;

[System.Serializable]
public class Position
{
    public int x;
    public int y;
}

[System.Serializable]
public class EnemyInfo
{
    public Location location;
    public EnemyAttributes enemyAttributes;
    public int rank;
    public int level;
}

[System.Serializable]
public class EnemyList
{
    public EnemyInfo[] enemies;
}


public class LevelLoader : MonoBehaviour
{
    public GameObject enemyChessmanPrefab;

    void Start()
    {
        LoadLevel("Level/level1.json");
    }

    void LoadLevel(string filePath)
    {
        string json = File.ReadAllText(Application.dataPath + "/" + filePath);
        EnemyList enemyList = JsonUtility.FromJson<EnemyList>("{\"enemies\":" + json + "}");

        foreach (EnemyInfo enemyInfo in enemyList.enemies)
        {
            // 生成敌人棋子
            GameObject enemyChessman = Instantiate(enemyChessmanPrefab);

            // 设置位置
            Vector3 position = new Vector3(enemyInfo.position.x, enemyInfo.position.y, 0);
            enemyChessman.transform.position = position;

            // 获取 Enemy 组件并赋值属性
            Enemy enemy = enemyChessman.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.enemyAttributes = ScriptableObject.CreateInstance<EnemyAttributes>();
                enemy.enemyAttributes.enemyName = enemyInfo.enemyAttributes.enemyName;
                enemy.enemyAttributes.level = enemyInfo.enemyAttributes.level;
                enemy.enemyAttributes.attack = enemyInfo.enemyAttributes.attack;
                enemy.enemyAttributes.defense = enemyInfo.enemyAttributes.defense;
                enemy.enemyAttributes.maxHealthPoints = enemyInfo.enemyAttributes.maxHealthPoints;
                enemy.enemyAttributes.InitAttributes();

                // 初始化生命值
                enemy.currentHealthPoints = enemy.enemyAttributes.maxHealthPoints;
            }
        }
    }
}