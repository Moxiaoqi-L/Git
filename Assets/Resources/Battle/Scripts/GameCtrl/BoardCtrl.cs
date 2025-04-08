using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 棋盘控制器：单例模式
public class BoardCtrl : MonoBehaviour
{
    public static BoardCtrl Get = null;

    // 保存着地图上所有的棋盘列表
    public List<Square> squares;
 
    public Square this[int x, int y]
    {
        get
        {
            foreach (Square square in squares)
            {
                if (square.location.x == x && square.location.y == y)
                {
                    return square;
                }
            }
            return null;
        }
    }
    
    public Square this[Location location]
    {
        get
        {
            return this[location.x, location.y];
        }
    }
 
    private void Awake()
    {
        Get = this;
    }
}