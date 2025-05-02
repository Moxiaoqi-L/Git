using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    // 物品 ID
    public int itemID;
    // 物品名称
    public string itemName;
    // 物品描述
    public string itemDetail;
    // 物品图片
    public string itemSprite;
    // 提供的 xp
    public int xp = 0;
    // 提供的体力
    public int stamina;
}

[Serializable]
public class Items
{
    public List<Item> items;
}