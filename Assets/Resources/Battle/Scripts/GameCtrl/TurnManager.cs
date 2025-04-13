using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.PlayerLoop;

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

    public void RefreshPlayerTurn()
    {
        // 当所有敌方行动完毕，开启玩家回合
        all = Chessman.All();
        foreach (var chessman in all)
        {
            chessman.enemy?.EndOfRound();
            chessman.hero?.EndOfRound();
        }
        isPlayerTurn = true;
    }

    // 结束玩家回合
    public void EndPlayerTurn()
    {
        if (isPlayerTurn)
        {
            heroes = Chessman.All(Camp.Player);
            foreach (var hero in heroes)
            {
                hero.hero.FinishAttack();
            }
            isPlayerTurn = false;
            StartEnemyTurn();
        }
            List<Chessman> remainingEnemies = Chessman.All(Camp.Enemy);
            Debug.Log(remainingEnemies.Count);
    }

    private void StartEnemyTurn()
    {
        StartCoroutine(EnemyAttackSequence());
    }

    // 协程方法，控制敌人依次攻击
    private IEnumerator EnemyAttackSequence()
    {
        foreach (Enemy enemy in Chessman.AllEnemies())
        {
            if (enemy != null)
            {
                enemy.TakeTurn();
                yield return new WaitForSeconds(0.4f);
            }
        }
        // 当所有敌方行动完毕，开启玩家回合
        RefreshPlayerTurn();
    }
    
}