using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BagDataSaveLoadManager : MonoBehaviour
{
    public static BagDataSaveLoadManager Get = null;
    
    private void Awake() {
        if (Get == null)
        {
            Get = this;
            LoadItemData();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private const string BAGFILE = "itemData.json";
    private Dictionary<int, Item> itemDictionary;    

    // 加载物品数据并存储到字典中
    private void LoadItemData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>($"General/Json/ItemsJson");
        Items items = JsonUtility.FromJson<Items>(jsonFile.text);
        itemDictionary = new Dictionary<int, Item>();
        foreach (var item in items.items)
        {
            itemDictionary[item.itemID] = item;
        }
    }

    // 加载背包文件
    public Bag LoadBagData()
    {
        string fullPath = Application.persistentDataPath + "/" + BAGFILE;

        if (File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            return JsonUtility.FromJson<Bag>(json);
        }

        return new Bag();
    }

    // 获取物品
    public void GetItem(int itemID, int amount)
    {
        // 打开文件
        Bag bag = LoadBagData();
        // 检测获取的物品是否合法
        if (!CheckItemLegal(itemID))
        {
            Debug.Log("不存在该物品");
            return;
        } 
        // 查找是否已经存在该物品
        int existIndex = bag.items.FindIndex(i => i.itemID == itemID);
        // 不存在
        if (existIndex == -1) 
        {   
            // 获取物品时需要检测是否存在该物品 （暂未实现）
            bag.items.Add(new ItemHave(itemID, amount));
            Debug.Log("获得新物品！");
        }
        // 存在就叠加数量
        else
        {
            bag.items[existIndex].amount += amount;
            Debug.Log("物品已存在！当前数量：" + bag.items[existIndex].amount);
        }
        // 存入文件
        string json = JsonUtility.ToJson(bag, true);
        File.WriteAllText(Application.persistentDataPath + "/" + BAGFILE, json);
    }

    // 使用物品
    public void UseItems(int itemID, int amount)
    {
        // 打开文件
        Bag bag = LoadBagData();
        // 查找是否已经存在该物品
        int existIndex = bag.items.FindIndex(i => i.itemID == itemID);
        // -1 说明不存在
        if (existIndex == -1) Debug.Log("该物品不存在！无法使用！");
        else if (bag.items[existIndex].amount < amount) Debug.Log("物品存在！但数量不足！");
        else
        {
            // 查看是否刚好用完
            if (bag.items[existIndex].amount == amount) bag.items.RemoveAt(existIndex);
            // 否则直接减去数量
            else bag.items[existIndex].amount -= amount;
            Debug.Log("使用了物品！剩余：" + bag.items[existIndex].amount);
        }
        // 存入文件
        string json = JsonUtility.ToJson(bag, true);
        File.WriteAllText(Application.persistentDataPath + "/" + BAGFILE, json);
    }

    // 检测获得物品是否合法
    private bool CheckItemLegal(int itemID)
    {
        return itemDictionary.ContainsKey(itemID);
    }
}