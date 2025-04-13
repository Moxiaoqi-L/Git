using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    // 当前波次
    public int currentStepNum;
    // 最大波次
    public int maxStepNum;
    private void Start()
    {
        // 初始波次为 1
        currentStepNum = 1;
        LoadLevel();
    }

    private void LoadLevel()
    {
        // 读取JSON文件
        TextAsset jsonFile = Resources.Load<TextAsset>("Battle/BattleLevel/Course-1");

        // 解析JSON数据
        LevelData levelData = JsonUtility.FromJson<LevelData>(jsonFile.text);
        StepData stepData = levelData.steps;
        if (stepData.step4 == null) maxStepNum = 3;
        if (stepData.step3 == null) maxStepNum = 2;
        if (stepData.step2 == null) maxStepNum = 1;
        if (currentStepNum == 1 && currentStepNum <= maxStepNum)
        {
            foreach (var enemy in stepData.step1)
            {
                InstantiateEnemyChessman(new Location(enemy.locationX, enemy.locationY), enemy.avatarImage, enemy.attributes);
            }
        }
        if (currentStepNum == 2 && currentStepNum <= maxStepNum)
        {
            foreach (var enemy in stepData.step2)
            {
                InstantiateEnemyChessman(new Location(enemy.locationX, enemy.locationY), enemy.avatarImage, enemy.attributes);
            }
        }
        if (currentStepNum == 3 && currentStepNum <= maxStepNum)
        {
            foreach (var enemy in stepData.step3)
            {
                InstantiateEnemyChessman(new Location(enemy.locationX, enemy.locationY), enemy.avatarImage, enemy.attributes);
            }
        }
        if (currentStepNum == 4 && currentStepNum <= maxStepNum)
        {
            foreach (var enemy in stepData.step4)
            {
                InstantiateEnemyChessman(new Location(enemy.locationX, enemy.locationY), enemy.avatarImage, enemy.attributes);
            }
        }
    }

    // 实例化敌方预制体
    public void InstantiateEnemyChessman(Location location,string avatarImageFileName, string attributeFileName)
    {
        // 加载预制体 , 配置文件, 图片
        GameObject enemyChessmanPrefab = Resources.Load<GameObject>("Battle/Prefab/EnemyChessmanPrefab");
        EnemyAttributes enemyAttributes = Resources.Load<EnemyAttributes>("Battle/Scripts/EnemyAttributes/" + attributeFileName);
        Sprite sprite = Resources.Load<Sprite>("Battle/Image/Avatar/" + avatarImageFileName);

        if (enemyChessmanPrefab == null)
        {
            Debug.LogError("无法加载 EnemyChessmanPrefab 预制体。");
            return;
        }

        // 2. 查找目标方格，假设存在一个 BoardCtrl 类来管理棋盘
        Square targetSquare = BoardCtrl.Get[location];

        if (targetSquare == null)
        {
            Debug.LogError($"未找到坐标为 {location} 的方格。");
            return;
        }

        // 实例化预制体
        GameObject enemyChessmanInstance = Instantiate(enemyChessmanPrefab);
        // 设置父物体
        enemyChessmanInstance.transform.SetParent(targetSquare.transform, false);
        // 设置 Chessman Location
        enemyChessmanInstance.GetComponent<Chessman>().location = location;
        // 设置 Enemy Attributes
        enemyChessmanInstance.GetComponent<Enemy>().enemyAttributes = enemyAttributes;
        // 设置头像图片
        enemyChessmanInstance.GetComponent<Image>().sprite = sprite;
    }
}

// 定义对应JSON结构的类
[System.Serializable]
public class LevelData
{
    public string levelName;
    public string level;
    public StepData steps;
}

[System.Serializable]
public class StepData
{
    public EnemyData[] step1;
    public EnemyData[] step2;
    public EnemyData[] step3;
    public EnemyData[] step4;
}

[System.Serializable]
public class EnemyData
{
    public string attributes;
    public string avatarImage;
    public int locationX;
    public int locationY;
    public int rank;
    public int level;
}