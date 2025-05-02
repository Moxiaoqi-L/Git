using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 背包类
[Serializable]
public class Bag
{
    public List<ItemHave> items = new();
}

// 拥有的物品
[Serializable]
public class ItemHave
{
    // ID
    public int itemID;
    // 数量
    public int amount;
    // 有效期
    // TODO

    public ItemHave(int itemID, int amount)
    {
        this.itemID = itemID;
        this.amount = amount;
    }
}
