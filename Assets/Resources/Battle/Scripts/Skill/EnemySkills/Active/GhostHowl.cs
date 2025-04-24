// 技能基类，继承自 ScriptableObject
using System.Collections.Generic;
using UnityEngine;

public class GhostHowl : Skill
{
    // 技能名字
    public override string SkillName
    {
        get
        {
            return "鬼嚎";
        }
    }

    // 技能描述
    public override string SkillDetail
    {
        get
        {
            return "双数回合时\n对面前T形范围嚎叫\n造成伤害并封印其技能 2 回合";
        }  
    }

    // 初始化方法
    public override void Init(SkillManager skillManager, BasicCharacter character)
    {
        base.Init(skillManager, character);
        skillType = SkillType.Active;
    }

    // 使用方法
    public override bool Use(Hero hero, Enemy target = null)
    {
        if (!BeforeUse()) return false;
        if (GameInit.Instance.currentArround % 2 != 0) return false;
        Debug.Log("使用鬼嚎");
        List<Location> skillLocations = new() {new Location(0, -1), new Location(-1, -1), new Location(1, -1), new Location(0, -2), new Location(0, -3)};
        List<Hero> heroes = MethodsForSkills.GetHeroesInRange(skillManager.character, skillLocations);
        foreach (Hero targethero in heroes)
        {
            targethero.Defend(skillManager.character.GetActualAttack(), DamageType.Mental);
            targethero.AddBuff("技能封印", 2);
        }
        return true;
    }
}