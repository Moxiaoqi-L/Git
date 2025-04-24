using UnityEngine;

[System.Serializable]
public class RedGroundDefense : Skill
{
    // 目标地块Y值、防御加成
    public int targetY = 2;
    public int defenseBonus = 3;

    // 记录是否已激活（避免重复添加）
    private bool isActive;

    public override string SkillName
    {
        get 
        { 
            return "耐造"; 
        }
    }

    public override string SkillDetail
    {
        get 
        { 
            return "在前排时, 获得 <color=#ff9a00>" + "3 </color>点 <color=#ff9a00>防御</color>";
        }
    }

    public override bool Use(Hero hero, Enemy target = null)
    {
        // 被动技能无需主动使用，仅用于初始化逻辑
        return true;
    }

    // 初始化技能（在Hero加载技能时调用）
    public override void Init(SkillManager skillManager, BasicCharacter character)
    {
        base.Init(skillManager, character);
        var hero =  character as Hero;

        skillType = SkillType.Passive;

        isActive = false;
        // 监听移动完成事件
        hero.chessman.OnMoveCompleted += CheckGroundEffect;
        hero.chessmanMove.OnMoveCompleted += CheckGroundEffect;
    }

    // 地块效果检查
    private void CheckGroundEffect(Hero hero)
    {
        int currentY = hero.chessman.location.y;
        bool isOnTargetGround = currentY == targetY;


        // 进入目标地块：激活加成
        if (isOnTargetGround && !isActive)
        {
            hero.IncreaseDefense(defenseBonus);
            isActive = true;
        }
        // 离开目标地块：移除加成
        else if (!isOnTargetGround && isActive)
        {
            hero.IncreaseDefense(-defenseBonus);
            isActive = false;
        }
    }
}