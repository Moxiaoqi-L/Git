// filePath: Git/Assets/Resources/Battle/Scripts/GameCtrl/ActionPointManager.cs
using System.Linq;
using UnityEngine;

public class APMPManager : MonoBehaviour
{
    public static APMPManager Get { get; private set; }
    
    public int maxHeroCount = 5;
    
    public int currentAP; // 当前行动点
    public int currentMP; // 当前移动点

    // 最大值：最大Hero数 + 1
    private int MaxAP;
    private int MaxMP;

    private void Awake()
    {
        if (Get == null) Get = this;
        else Destroy(gameObject);
    }

    // 每回合开始时重置AP/MP（根据存活Hero数量）
    public void ResetPointsAtTurnStart()
    {
        int aliveHeroCount = Chessman.AllHeros()
            .Count(hero => hero.currentHealthPoints > 0);
        
        MaxAP = aliveHeroCount + 1;
        MaxMP = aliveHeroCount + 1;
        
        currentAP = Mathf.Min(currentAP + aliveHeroCount, MaxAP);
        currentMP = Mathf.Min(currentMP + aliveHeroCount, MaxMP);
    }

    // 消耗行动点（返回是否成功）
    public bool ConsumeAP()
    {
       if (currentAP > 0)
       {
            currentAP--;
            return true;
       }
       return false;
    } 

    // 消耗移动点（返回是否成功）
    public bool ConsumeMP()
    {
       if (currentMP > 0)
       {
            currentMP--;
            return true;
       }
       return false;
    }
}