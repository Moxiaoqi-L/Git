using System;
using System.Collections.Generic;
using UnityEngine;

// 自我治疗技能
[System.Serializable]
public class DelayedTreatment : Skill
{
    // 治疗数值
    public int healPoints = 10;
    // 每层治疗BUFF数值
    public int healPerLayer = 2;
    // BUFF层数
    public int layers = 2;


    public override string SkillName
    {
        get
        {
            return "延时治疗";
        }
    }
    public override string SkillDetail
    {
        get
        {
            return "为前方队友治疗 <color=#30e3ca>" + healPoints + "生命值</color>\n"
            + "并附加 <color=#30e3ca>" + layers + "</color> 层 <color=#30e3ca>治愈BUFF</color>\n"
            + "每层治愈回复 <color=#30e3ca>" + healPerLayer + "生命值</color>";
        }
    }
    public override List<Color> Costs
    {
        get
        {
            return new List<Color> { Constants.BLUEPOINT, Constants.BLUEPOINT};
        }
    }


    public override void Init(SkillManager skillManager, BasicCharacter character)
    {
        base.Init(skillManager, character);
        skillType = SkillType.Active;
        // 无需Setup
    }

    public override bool Use(Hero hero, Enemy target = null)
    {
        // 获取前方的 hero
        Hero targetHero = MethodsForSkills.GetFrontHero(hero.chessman.location);
        if (targetHero == null)
        {
            hero.IncreaseHealthPoints(healPoints, new Color(0.19f, 0.89f, 0.79f));
            hero.AddBuff("自愈", healPerLayer, layers);
        }
        else
        {
            targetHero.IncreaseHealthPoints(healPoints, new Color(0.19f, 0.89f, 0.79f));
            targetHero.AddBuff("自愈", healPerLayer, layers);
            hero.IncreaseHealthPoints((int)(healPoints * 0.5), new Color(0.19f, 0.89f, 0.79f));
        }
        AudioManager.Instance.PlaySFX(skillAudio);
        hero.FinishAttack(target);
        return true;
    }
}