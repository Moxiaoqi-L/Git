using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.PlayerLoop;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }
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

    public void EndPlayerTurn()
    {
        if (isPlayerTurn)
        {
            isPlayerTurn = false;
            StartEnemyTurn();
        }
    }

    private void StartEnemyTurn()
    {
        StartCoroutine(EnemyAttackSequence());
        // 当所有敌方行动完毕，开启玩家回合
        isPlayerTurn = true;
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
    }
    
}