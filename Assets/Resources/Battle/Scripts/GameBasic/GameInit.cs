using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    // 当前回合
    public int currentArround = 1;

    // 预制体
    public GameObject enemyChessmanPrefab;

    // Step 文字组件
    public TextMeshProUGUI stepText;
    // Round 文字组件
    public TextMeshProUGUI roundText;

    // 场景加载的实例
    private SceneLoaderWithAnimation sceneLoader;

    private static TextAsset jsonFile;
    private static LevelData levelData;
    private static StepData stepData;

    // 回合增加事件
    public event Action OnNextRound;
    
    // 确保唯一
    private void Awake()
    {
        // 检查实例是否已经存在
        if (Instance == null) Instance = this;
        // 获取场景加载
        sceneLoader = FindObjectOfType<SceneLoaderWithAnimation>();
    }

    private void OnDestroy()
    {
    }

    // 棋子退场事件处理方法
    public void OnEnemyExitHandler()
    {
        StartCoroutine(Check());
    }
    // 棋子退场事件处理方法
    public void OnHeroExitHandler(Hero hero)
    {
        // TODO
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
        UpdateStepAndRound(true);
        if (currentStepNum == 1 && currentStepNum <= maxStepNum)
        {
            foreach (var enemy in stepData.step1)
            {
                InstantiateEnemyChessman(new Location(enemy.locationX, enemy.locationY), enemy.avatarImage, enemy.rarity, enemy.attributes);
            }
        }
        if (currentStepNum == 2 && currentStepNum <= maxStepNum)
        {
            foreach (var enemy in stepData.step2)
            {
                InstantiateEnemyChessman(new Location(enemy.locationX, enemy.locationY), enemy.avatarImage, enemy.rarity, enemy.attributes);
            }
        }
        if (currentStepNum == 3 && currentStepNum <= maxStepNum)
        {
            foreach (var enemy in stepData.step3)
            {
                InstantiateEnemyChessman(new Location(enemy.locationX, enemy.locationY), enemy.avatarImage, enemy.rarity, enemy.attributes);
            }
        }
        if (currentStepNum == 4 && currentStepNum <= maxStepNum)
        {
            foreach (var enemy in stepData.step4)
            {
                InstantiateEnemyChessman(new Location(enemy.locationX, enemy.locationY), enemy.avatarImage, enemy.rarity, enemy.attributes);
            }
        }
        StartCoroutine(WaitFor());
    }

    // 实例化敌方预制体
    public void InstantiateEnemyChessman(Location location,string avatarImageFileName, int rarityIndex, string attributeFileName)
    {
        // 加载预制体 , 配置文件, 图片
        EnemyAttributes enemyAttributes = Resources.Load<EnemyAttributes>("Battle/Scripts/CharacterAttributes/EnemyAttributes/" + attributeFileName);
        Debug.Log(enemyAttributes);
        Sprite avatarSprite = Resources.Load<Sprite>("Battle/Image/Avatar/" + avatarImageFileName);
        Sprite raritySprite = Resources.Load<Sprite>("Battle/Image/Rarity/Rarity Enemy " + rarityIndex);

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
        enemyChessmanInstance.GetComponent<Enemy>().characterAttributes = enemyAttributes;
        // 设置头像图片
        enemyChessmanInstance.transform.Find("Avatar").GetComponent<Image>().sprite = avatarSprite;
        // 设置稀有度图片
        enemyChessmanInstance.transform.Find("Rarity").GetComponent<Image>().sprite = raritySprite;
    }

    // 进入下一阶段的方法
    public void NextStep()
    {
        // 处理进入下一阶段的逻辑
        currentStepNum += 1;
        LoadLevel();
        // 可以在这里添加场景切换、数据更新等操作
    }

    public void UpdateStepAndRound(bool refreshRound = false)
    {
        if (refreshRound) currentArround = 0;
        currentArround += 1;
        OnNextRound?.Invoke();
        stepText.text = currentStepNum + " / " + maxStepNum;
        roundText.text = currentArround.ToString();
    }
    private IEnumerator Check()
    {
        // 等待一帧，确保 Destroy 操作完成
        yield return null;
        // 检查是否还有其他敌人存活
        List<Chessman> remainingEnemies = Chessman.All(Camp.Enemy);
        if (remainingEnemies.Count == 0)
        {
            if(currentStepNum >= maxStepNum)
            {
                sceneLoader.LoadScene("MainMenu");
            }
            else
            {
                // 进入下一阶段
                sceneLoader.LoadScene(null, NextStep);
                // 刷新 hero 状态
                TurnManager.Instance.RefreshPlayerTurn();
            }
        }
    }

    private IEnumerator WaitFor()
    {   
        yield return new WaitForSeconds(0.1f);
        LineManager.Get.LoadAllCharacterLines();
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
    public int rarity;
    public string rarityImage;
    public int locationX;
    public int locationY;
    public int rank;
    public int level;
}