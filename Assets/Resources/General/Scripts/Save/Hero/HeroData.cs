using System;
using System.Collections.Generic;

[Serializable]
public class HeroData
{
    // 英雄的ID
    public int heroID;
    // 英雄的等级
    public int level;
    // 英雄的阶级
    public int rank;
    // 英雄当前的经验
    public int currentXp;    
    // 好感度
    public int likability;

    public HeroData(HeroDataStruct heroDataStruct)
    {
        heroID = heroDataStruct.heroID;
        level = heroDataStruct.level;
        rank = heroDataStruct.rank;
        currentXp = heroDataStruct.currentXp;
        likability = heroDataStruct.likability;
    }
}

[Serializable]
public class HeroDataList
{
    public List<HeroData> heroes = new();
}

public struct HeroDataStruct
{
    // 英雄的ID
    public int heroID;
    // 英雄的等级
    public int level;
    // 英雄的阶级
    public int rank;
    // 英雄当前的经验
    public int currentXp;    
    // 好感度
    public int likability;

    public HeroDataStruct(int heroID)
    {
        this.heroID = heroID;
        level = 1;
        rank = 0;
        currentXp = 0;
        likability = 0;
    }
}