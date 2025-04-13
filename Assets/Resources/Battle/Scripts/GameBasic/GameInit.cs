using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInit : MonoBehaviour
{
    // 单例
    public static GameInit Instance { get; private set; }
    // 当前波次
    public int currentStepNum;
    // 最大波次
    public int maxStepNum;

    private static TextAsset jsonFile;
    private static LevelData levelData;
    private static StepData stepData;
    // 确保唯一
    private void Awake()
    {
        // 检查实例是否已经存在
        if (Instance == null)
        {
            // 如果不存在，则将当前实例赋值给 Instance
            Instance = this;
        }
        else
        {
            // 如果已经存在，则销毁当前对象
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
    }

    // 棋子退场事件处理方法
    public void OnChessmanExitHandler()
    {
        StartCoroutine(Check());
    }

    private void Start() {
        // 初始波次为 1
        currentStepNum = 1;
        // 读取JSON文件
        jsonFile = Resources.Load<TextAsset>("Battle/BattleLevel/Course-1");
        // 解析JSON数据
        levelData = JsonUtility.FromJson<LevelData>(jsonFile.text);
        stepData = levelData.steps;
        // 判断最大波次
        if (stepData.step4 == null) maxStepNum = 3;
        if (stepData.step3 == null) maxStepNum = 2;
        if (stepData.step2 == null) maxStepNum = 1;
        LoadLevel();
    }

    public void LoadLevel()
    {
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

    // 进入下一阶段的方法
    public void NextStep()
    {
        // 处理进入下一阶段的逻辑
        currentStepNum += 1;
        LoadLevel();
        Debug.Log("进入下一阶段");
        // 可以在这里添加场景切换、数据更新等操作
    }

    private IEnumerator Check()
    {
        // 等待一帧，确保 Destroy 操作完成
        yield return null;
        // 检查是否还有其他敌人存活
        List<Chessman> remainingEnemies = Chessman.All(Camp.Enemy);
        Debug.Log(remainingEnemies.Count);
        if (remainingEnemies.Count == 0)
        {
            // 进入下一阶段
            TransitionManager.Instance.StartTransition(NextStep);
            TurnManager.Instance.RefreshPlayerTurn();
        }
    }

}

// 定义对应JSON结构的类
[Serializable]
public class LevelData
{
    public string levelName;
    public string level;
    public StepData steps;
}

[Serializable]
public class StepData
{
    public EnemyData[] step1;
    public EnemyData[] step2;
    public EnemyData[] step3;
    public EnemyData[] step4;
}

[Serializable]
public class EnemyData
{
    public string attributes;
    public string avatarImage;
    public int locationX;
    public int locationY;
    public int rank;
    public int level;
}