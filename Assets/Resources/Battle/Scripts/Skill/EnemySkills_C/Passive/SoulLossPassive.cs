using UnityEngine;

[CreateAssetMenu(fileName = "失魂诅咒", menuName = "技能/敌人被动技能/失魂被动")]
public class SoulLossPassive : Skill
{
    // 触发概率（百分比）
    public int procChance = 100;
    // 附加的BUFF层数
    public int buffLayers = 2;

    public override string SkillName
    {
        get { return "<color=#608BBB>失魂诅咒</color>"; }
    }

    public override string SkillDetail
    {
        get { return $"攻击时有<color=#ff2e63> {procChance}% </color>概率附加<color=#608BBB> 失魂BUFF</color>"; }
    }

    public override void Setup(Hero hero = null, Enemy enemy = null)
    {
        // 确保绑定Enemy的攻击完成事件
        if (enemy != null)
        {
            enemy.OnAttackCompleted += ApplySoulLossBuff;
        }
    }

    public override void Use(Hero hero, Enemy target = null)
    {
        // 无需实现
    }

    private void ApplySoulLossBuff(Hero targetHero, Enemy attacker)
    {
        // 概率判定
        if (Random.Range(0, 100) < procChance)
        {
            targetHero.AddBuff("失魂", buffLayers); // 使用现有BUFF添加接口
        }
    }
}