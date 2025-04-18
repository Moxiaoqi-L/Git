using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class Chessman : MonoBehaviour
{
    // 棋子的坐标
    public Location location;
    // 棋子的阵营
    public Camp camp;
    // 棋子所在的方格
    public Square Square => BoardCtrl.Get[location];

    public Hero hero;
    public Enemy enemy;


    // 初始化棋子
    public void Start()
    {
        if (camp == Camp.Neutral)
        {
            Debug.LogError("棋子阵营不能为中立。");
            return;
        }
        if (camp == Camp.Player)
        {
            hero = GetComponent<Hero>();
        }
        if (camp == Camp.Enemy)
        {
            enemy = GetComponent<Enemy>();
        }
        InitMove(location);
        GetComponent<Button>().onClick.AddListener(OnChessmanClicked);
    }
 
    // 默认输出
    public override string ToString()
    {
        return $"棋子坐标:{location} 阵营:{camp}";
    }
 
    // 获取当前场上的全部棋子，或者某一方的全部棋子。
    public static List<Chessman> All(Camp camp = Camp.Neutral)
    {
        List<Chessman> ret = new List<Chessman>();
        var chessmen = FindObjectsOfType<Chessman>();
        foreach (var chessman in chessmen)
        {
            if (camp == Camp.Neutral || camp == chessman.camp)
            {
                ret.Add(chessman);
            }
        }
        return ret;
    }

    // 获取当前场上的全部英雄
    public static List<Hero> AllHeros()
    {
        List<Hero> ret = new();
        var chessmen = FindObjectsOfType<Chessman>();
        foreach (var chessman in chessmen)
        {
            if (chessman.camp == Camp.Player)
            {
                ret.Add(chessman.hero);
            }
        }
        return ret;
    }

    // 获取当前场上的全部敌人
    public static List<Enemy> AllEnemies()
    {
        List<Enemy> ret = new();
        var chessmen = FindObjectsOfType<Chessman>();
        foreach (var chessman in chessmen)
        {
            if (chessman.camp == Camp.Enemy)
            {
                ret.Add(chessman.enemy);
            }
        }
        return ret;
    }

    // 清除场上的全部棋子
    public static void ClearAll()
    {
        var all = All();
        for (int i = all.Count - 1; i >= 0; i--)
        {
            all[i].ExitFromBoard();
        }
    }

    // 依照坐标查询，找到位于相应坐标上的棋子
    public static Chessman GetChessman(Location location)
    {
        foreach (var chessman in All())
        {
            if (chessman.location.Equals(location))
            {
                return chessman;
            }
        }
        return null;
    }
 
    // 使棋子移动到指定坐标
    public void InitMove(Location target)
    {
        //定位目标棋盘格
        Square square = BoardCtrl.Get[target.x, target.y];
        //修改自身坐标为新的坐标
        location = target;
        // 瞬间移动
        transform.position = square.transform.position;
    }
 
    // 棋子被选中时
    private void OnChessmanClicked()
    {
        if (SelectCore.Selection == null)
        {
            SelectCore.TrySelect(this);
            if (camp == Camp.Player)
            {
                ShowAttackRange(hero);
            }
            return;
        }
        // 重复点击棋子,取消选中
        if (SelectCore.Selection == this)
        {
            SelectCore.DropSelect();
            // 如果是玩家,取消显示攻击范围
            if (camp == Camp.Player)
            {
                CancelShowAttackRange(hero);
            }
            return;
        }

        if (camp == Camp.Player)
        {
            if (SelectCore.Selection.camp == Camp.Player)
            {
                CancelShowAttackRange(SelectCore.Selection.hero);
            }
            ShowAttackRange(hero);
            SelectCore.TrySelect(this);
            return;
        }
        if (SelectCore.Selection.camp == Camp.Player && camp == Camp.Enemy)
        {
            // 获取当前选中英雄的攻击范围
            Hero selectedHero = SelectCore.Selection.hero;
            // 检查目标敌人的位置是否在攻击范围内
            if (selectedHero.GetAttackRange().Contains(enemy.chessman.location))
            {
                selectedHero.Attack(enemy); // 执行攻击
            }
            else
            {
                Debug.LogWarning("目标超出攻击范围！");
                // 可在此添加UI提示（如红色提示文字）
            }
        }
    }

    public void ShowAttackRange(Hero hero)
    {
        foreach (Square square in BoardCtrl.Get.squares)
        {
            if (hero.GetAttackRange().Contains(square.location))
            {
                square.SetAttackRangeHighlight(true);
            }
        }
    }

    public void CancelShowAttackRange(Hero hero)
    {
        foreach (Square square in BoardCtrl.Get.squares)
        {
            if (hero.GetAttackRange().Contains(square.location))
            {
                square.SetAttackRangeHighlight(false);
            }
        }
    }

    public void ExitFromBoard()
    {
        StartCoroutine(WaitForExit());
    }
    private IEnumerator WaitForExit()
    {
        yield return new WaitForSeconds(0.5f);
        // 播放退场动画
        transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
        {
            if (camp == Camp.Enemy)
            {
                GameInit.Instance.OnEnemyExitHandler();
                Destroy(gameObject);
            }
            if (camp == Camp.Player)
            {
                GameInit.Instance.OnHeroExitHandler(hero);
                Destroy(gameObject);                
            }

        });
    }
}