using UnityEngine;
using System;
 
# region Location:坐标
// 一个棋盘格或棋子的坐标, 结构体
[Serializable]
public struct Location
{
    // 横坐标，表示棋盘上的列，范围从0至5
    public int x;
    // 纵坐标，表示棋盘上的行，范围从0至5
    public int y;
 
    // 坐标值的合法范围
    public const int Xmin = 0;
    public const int Xmax = 5;
    public const int Ymin = 0;
    public const int Ymax = 5;
    //允许以Unity二维向量形式表示一个棋盘坐标——这能够方便坐标之间距离的计算
    public Vector2Int Vector => new Vector2Int(x, y);

    // 构造函数:使用一组x和y的值创建新坐标
    public Location(int x, int y)
    {
        this.x = x;
        this.y = y;
        if (!IsValid(x, y))
        {
            Debug.LogWarning($"正在创建一个超出棋盘范围的方格:({x},{y})");
        }
    }
 
    // 默认输出
    public override string ToString()
    {
        return $"({x},{y})";
    }
 
    // 判断两个坐标是否是相邻坐标
    public bool IsNear(Location other)
    {
        if (Vector2Int.Distance(Vector, other.Vector) == 1)
        {
            return true;
        }
        return false;
    }
    public static bool IsNear(Location a, Location b)
    {
        return a.IsNear(b);
    }
 
    // 判断一个坐标是否是合法坐标，即位于棋盘范围内的坐标
    public bool IsValid()
    {
        if (x >= Xmin && x <= Xmax && y >= Ymin && y <= Ymax)
        {
            return true;
        }
        return false;
    }
    private static bool IsValid(int x, int y)
    {
        if (x >= Xmin && x <= Xmax && y >= Ymin && y <= Ymax)
        {
            return true;
        }
        return false;
    }

    // 比较判断 == !=
    public static bool operator ==(Location a, Location b)
    {
        return a.Vector == b.Vector;
    }
 
    public static bool operator !=(Location a, Location b)
    {
        return a.Vector != b.Vector;
    }

    // 匹配判断
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
#endregion

#region Camp:阵营
/// <summary>
/// 玩家阵营。
/// </summary>
public enum Camp
{
    Neutral, Player, Enemy
}
#endregion

#region SquareType:地形类型
/// <summary>
/// 棋盘方格的地形类型。
/// </summary>
public enum SquareType
{
    Land, River, Trap, Cave
}
#endregion