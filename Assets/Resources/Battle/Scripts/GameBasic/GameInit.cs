using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
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
    private static AudioClip battleMusic;

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
        jsonFile = Resources.Load<TextAsset>("General/BattleLevel/Course-1");
        // 解析JSON数据
        levelData = JsonUtility.FromJson<LevelData>(jsonFile.text);
        // 播放背景音乐
        battleMusic = Resources.Load<AudioClip>(Constants.MUSIC_PATH + levelData.music);
        AudioManager.Instance.PlayBGM(battleMusic);
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
                InstantiateEnemyChessman(enemy);
            }
        }
        if (currentStepNum == 2 && currentStepNum <= maxStepNum)
        {
            foreach (var enemy in stepData.step2)
            {
                InstantiateEnemyChessman(enemy);
            }
        }
        if (currentStepNum == 3 && currentStepNum <= maxStepNum)
        {
            foreach (var enemy in stepData.step3)
            {
                InstantiateEnemyChessman(enemy);
            }
        }
        if (currentStepNum == 4 && currentStepNum <= maxStepNum)
        {
            foreach (var enemy in stepData.step4)
            {
                InstantiateEnemyChessman(enemy);
            }
        }
        StartCoroutine(WaitFor());
    }

    // 实例化敌方预制体
    public void InstantiateEnemyChessman(EnemyData enemy)
    {
        // 加载预制体 , 配置文件, 图片
        EnemyAttributes enemyAttributes = Resources.Load<EnemyAttributes>("Battle/Scripts/CharacterAttributes/EnemyAttributes/" + enemy.attributes);
        Sprite avatarSprite = Resources.Load<Sprite>("General/Image/Avatar/" + enemy.avatarImage);
        Sprite raritySprite = Resources.Load<Sprite>("General/Image/Rarity/Rarity Enemy " + enemy.rarity);
        // 查找目标方格
        Square targetSquare = BoardCtrl.Get[new Location(enemy.locationX, enemy.locationY)];
        // 实例化预制体
        GameObject enemyChessmanInstance = Instantiate(enemyChessmanPrefab);
        // 获取怪物实例的Enemy组件
        Enemy enemyComponent = enemyChessmanInstance.GetComponent<Enemy>();
        // 设置父物体
        enemyChessmanInstance.transform.SetParent(targetSquare.transform, false);
        // 设置 Chessman Location
        enemyChessmanInstance.GetComponent<Chessman>().location = targetSquare.location;
        // 设置 Enemy Attributes
        enemyComponent.characterAttributes = enemyAttributes;
        // 设置Enemy等级与阶级
        enemyComponent.characterAttributes.level = enemy.level;
        enemyComponent.characterAttributes.rank = enemy.rank;
        // 设置头像图片
        StartCoroutine(
            ABLoader.LoadAssetAsync<Sprite>(enemyAttributes.AssetBundlePath.ToLower(), enemyAttributes.avatarImage, sprite =>
            {
                enemyChessmanInstance.transform.Find("Avatar").GetComponent<Image>().sprite = sprite; // 加载完成后赋值
            })
        );   
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
    public string music;
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