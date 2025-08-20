using UnityEngine;

public class RetaliationAttackReduction : Skill
{
    // 降低的攻击力数值
    public int attackReductionAmount = 0;
    // 持续回合数
    public int durationRounds = 2;

    public override string SkillName
    {
        get { return "粘液"; }
    }

    public override string SkillDetail
    {
        get { 
            return "受到攻击时，降低攻击者 <color=#FF4500>" + attackReductionAmount + " 攻击力</color>\n" +
                   "持续 <color=#FF4500>" + durationRounds + "</color> 回合"; 
        }
    }

    public override void Init(SkillManager skillManager, BasicCharacter character)
    {
        base.Init(skillManager, character);
        attackReductionAmount = skillManager.character.characterAttributes.attack;
        skillType = SkillType.Passive;
        
        // 订阅角色被攻击事件
        skillManager.character.EventManager.OnAttackedBy += OnAttacked; // 假设事件名为OnAttackedBy，参数为攻击者
    }

    private void OnAttacked(BasicCharacter attacker)
    {
        if (attacker == null) return;
        
        // 创建攻击力下降 BUFF并应用给攻击者
        attacker.buffManager.AddBuff("攻击力下降", skillManager.character.GetActualAttack(), durationRounds) ;
        // 显示提示（可选）
        attacker.ShowText($"攻击力下降 {attackReductionAmount}！", Constants.REDPOINT);
    }

    public override bool Use(Hero hero, Enemy target = null)
    {
        // 被动技能无需主动使用
        return true;
    }
}