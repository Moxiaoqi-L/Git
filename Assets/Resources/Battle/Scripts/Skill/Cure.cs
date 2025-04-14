using UnityEngine;

// 自我治疗技能
[CreateAssetMenu(fileName = "治疗", menuName = "技能/治疗")]
public class SelfHealingSkill : Skill
{
    public Color[] costs = {Constants.BLUEPOINT,Constants.BLUEPOINT};
    public int healPoints;
    public override void Use(Hero hero, Enemy target = null)
    {
        if (hero.hasAttacked || !ColorPointCtrl.Get.RemoveColorPointsByColors(costs)) return;
        Hero targetHero = MethodsForSkills.GetFrontHero(hero.chessman.location);
        if (targetHero == null)
        {
            hero.IncreaseHealthPoints(healPoints);
        }
        else
        {
            targetHero.IncreaseHealthPoints(healPoints);
            hero.IncreaseHealthPoints((int)(healPoints * 0.5));
        }
        hero.FinishAttack();
    }
}