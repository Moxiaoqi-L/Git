using UnityEngine;

// 自我治疗技能
[CreateAssetMenu(fileName = "治疗", menuName = "技能/治疗")]
public class SelfHealingSkill : Skill
{
    public Color[] costs = {Constants.BLUEPOINT,Constants.BLUEPOINT};
    public int healPoints;
    public int healPerLayer;
    public int layers;

    public override void Use(Hero hero, Enemy target = null)
    {
        if (hero.hasAttacked || !ColorPointCtrl.Get.RemoveColorPointsByColors(costs)) return;
        // 获取前方的 hero
        Hero targetHero = MethodsForSkills.GetFrontHero(hero.chessman.location);
        if (targetHero == null)
        {
            hero.IncreaseHealthPoints(healPoints);
            hero.AddBuff("自愈", healPerLayer, layers);
            hero.AddBuff("中毒", 3, 3);
            
        }
        else
        {
            targetHero.IncreaseHealthPoints(healPoints);
            targetHero.AddBuff("自愈", healPerLayer, layers);
            hero.IncreaseHealthPoints((int)(healPoints * 0.5));
        }
        hero.FinishAttack();
    }
}