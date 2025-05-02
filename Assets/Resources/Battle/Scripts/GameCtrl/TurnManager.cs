using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.PlayerLoop;
using System;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }
    public List<Chessman> heroes;
    public List<Chessman> all;
    public bool isPlayerTurn = true; // 当前是否是玩家回合

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // 开启玩家回合
    public void StartPlayerTurn()
    {
        // 当所有敌方行动完毕，开启玩家回合
        all = Chessman.All();
        foreach (var chessman in Chessman.All(Camp.Enemy))
        {
            chessman.enemy.EndOfRound();
        }
        List<Hero> allHeroes = Chessman.AllHeros();
        foreach (Hero hero in allHeroes)
        {
            hero.StartOfTurn();
        }
        isPlayerTurn = true;
        GameInit.Instance.UpdateStepAndRound();
        APMPManager.Get.ResetPointsAtTurnStart();
    }

    // 刷新玩家回合
    public void RefreshPlayerTurn()
    {
        List<Hero> allHeroes = Chessman.AllHeros();
        foreach (Hero hero in allHeroes)
        {
            hero.RefreshSelf();
        }
        APMPManager.Get.ResetPointsAtTurnStart();
    }

    // 结束玩家回合
    public void EndPlayerTurn()
    {
        if (isPlayerTurn)
        {
            StartCoroutine(WaitForBuffSettlement());
            isPlayerTurn = false;
        }
    }

    // 开始敌人回合
    private void StartEnemyTurn()
    {
        StartCoroutine(EnemyAttackSequence());
    }

    // 结束敌人回合
    private void EndEnemyTurn()
    {

    }

    // 协程方法，控制敌人依次攻击
    private IEnumerator EnemyAttackSequence()
    {
        foreach (Enemy enemy in Chessman.AllEnemies())
        {
            if (enemy != null)
            {
                enemy.TakeTurn();
                yield return new WaitForSeconds(0.3f);
            }
        }
        // 当所有敌方行动完毕，结束敌人回合
        EndEnemyTurn();
        // 当所有敌方行动完毕，开启玩家回合
        StartPlayerTurn();
    }

    // 协程方法, 玩家回合结束时等待buff结算完毕
    private IEnumerator WaitForBuffSettlement()
    {
        // 获取所有英雄并触发回合结束逻辑
        List<Hero> allHeroes = Chessman.AllHeros();
        foreach (Hero hero in allHeroes)
        {
            hero.FinishAttack();
            hero.EndOfRound(); // 触发英雄的回合结束（包括Buff结算）
            yield return null; // 等待Buff协程开始
        }

        // 等待所有英雄的BuffManager完成回合结算（根据实际协程逻辑调整）
        // 假设BuffManager使用协程处理层数，这里需要确保协程完成
        yield return new WaitUntil(() => AreAllBuffsSettled(allHeroes));

        StartEnemyTurn(); // 所有Buff结算完成后启动敌人回合
    }

    // 检查所有英雄的BuffManager是否无正在处理的协程
    private bool AreAllBuffsSettled(List<Hero> heroes)
    {
        // 检查所有英雄的BuffManager是否无正在处理的协程
        // 实际需根据BuffManager的状态判断
        foreach (Hero hero in heroes)
        {
            if (hero.buffManager.IsProcessingRoundEnd) // 假设添加状态标记
                return false;
        }
        return true;
    }
    
}