using System.Collections;
using System.IO;
using UnityEngine;

public class HeroDataSaveLoadManager : MonoBehaviour
{
    public static HeroDataSaveLoadManager Get = null;
    
    private void Awake() {
        if (Get == null)
        {
            Get = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private const string HEROFILE = "heroData.json";

    // 加载英雄文件
    public HeroDataList LoadHeroData()
    {
        string fullPath = Application.persistentDataPath + "/" + HEROFILE;

        if (File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            return JsonUtility.FromJson<HeroDataList>(json);
        }

        return new HeroDataList();
    }

    // 获取新英雄
    public void GetNewHero(int heroID)
    {
        // 打开文件
        HeroDataList heroDataList = LoadHeroData();
        // 查找是否已经存在该英雄
        int existIndex = heroDataList.heroes.FindIndex(h => h.heroID == heroID);
        if (existIndex == -1) 
        {
            HeroDataStruct heroDataStruct = new(heroID);
            heroDataList.heroes.Add(new HeroData(heroDataStruct));
            Debug.Log("获得新英雄！");
        }
        else Debug.Log("该英雄已存在！无法重复获得");
        // 存入文件
        string json = JsonUtility.ToJson(heroDataList, true);
        File.WriteAllText(Application.persistentDataPath + "/" + HEROFILE, json);
    }

    // 英雄获取经验值
    public void HeroGetXP(int heroID, int amount)
    {
        // 打开文件
        HeroDataList heroDataList = LoadHeroData();
        // 查找是否已经存在该英雄
        int existIndex = heroDataList.heroes.FindIndex(h => h.heroID == heroID);
        // -1 说明不存在
        if (existIndex == -1) Debug.Log("该英雄不存在！无法保存数据,请先获得英雄");
        else
        {
            // 协程获取/计算经验
            heroDataList.heroes[existIndex].currentXp += amount;
            StartCoroutine(CalculateXP(heroDataList.heroes[existIndex]));
        }
        // 存入文件
        string json = JsonUtility.ToJson(heroDataList, true);
        File.WriteAllText(Application.persistentDataPath + "/" + HEROFILE, json);
    }

    // 英雄升级
    public void HeroLevelUp(int heroId)
    {
        // 打开文件
        HeroDataList heroDataList = LoadHeroData();
        // 查找是否已经存在该英雄
        int existIndex = heroDataList.heroes.FindIndex(h => h.heroID == heroId);        
        if (existIndex == -1) Debug.Log("该英雄不存在");
        else
        {
            heroDataList.heroes[existIndex].level += 1;
            Debug.Log("升级成功！");
        }
        // 存入文件
        string json = JsonUtility.ToJson(heroDataList, true);
        File.WriteAllText(Application.persistentDataPath + "/" + HEROFILE, json);
    }

    private IEnumerator CalculateXP(HeroData heroData)
    {
        while(heroData.currentXp >= 100)
        {
            heroData.currentXp -= 100;
            heroData.level += 1;
            CheckRank(heroData);
            Debug.Log("升级！当前等级" + heroData.level);
        }
        yield return null;
    }

    private void CheckRank(HeroData heroData)
    {
        switch (heroData.rank)
        {
            case 0:
                if(heroData.level == 45)
                {
                    heroData.rank += 1;
                    heroData.level = 1;
                }
                return;
            case 1:
                if(heroData.level == 65)
                {
                    heroData.rank += 1;
                    heroData.level = 1;
                }
                return;
            case 2:
                if(heroData.level >= 90) heroData.level = 90;
                return;
            default:
                return;
        }
    }
}