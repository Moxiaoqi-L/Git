using System.Collections.Generic;
using UnityEngine;

// 自我治疗技能
[CreateAssetMenu(fileName = "延时治疗", menuName = "技能/治疗")]
public class SelfHealingSkill : Skill
{
    // 治疗数值
    public int healPoints;
    // 每层治疗BUFF数值
    public int healPerLayer;
    // BUFF层数
    public int layers;


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


    public override void Setup(Hero hero = null, Enemy enemy = null)
    {
        // 无需Setup
    }

    public override void Use(Hero hero, Enemy target = null)
    {
        if (hero.isStunned) return;
        if (hero.hasAttacked || !ColorPointCtrl.Get.RemoveColorPointsByColors(Costs)) return;
        // 获取前方的 hero
        Hero targetHero = MethodsForSkills.GetFrontHero(hero.chessman.location);
        if (targetHero == null)
        {
            hero.IncreaseHealthPoints(healPoints);
            hero.AddBuff("自愈", healPerLayer, layers);
            
        }
        else
        {
            targetHero.IncreaseHealthPoints(healPoints);
            targetHero.AddBuff("自愈", healPerLayer, layers);
            hero.IncreaseHealthPoints((int)(healPoints * 0.5));
        }
        hero.FinishAttack(target);
    }
}