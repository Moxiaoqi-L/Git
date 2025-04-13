using System.Collections.Generic;
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
            return;
        }
        if (SelectCore.Selection == this)
        {
            SelectCore.DropSelect();
            return;
        }
        if (camp == Camp.Player)
        {
            SelectCore.TrySelect(this);
            return;
        }
        if (SelectCore.Selection.camp == Camp.Player && camp == Camp.Enemy)
        {
            SelectCore.Selection.hero.Attack(enemy);
        }
    }
 
    // 使这个棋子退场
    public void ExitFromBoard()
    {
        Destroy(gameObject);
    }
}