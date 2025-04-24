// 技能基类，继承自 ScriptableObject
using System.Collections.Generic;
using UnityEngine;


public class Petrified : Skill
{

    private int petrifyLayer = 0;

    // 技能名字
    public override string SkillName
    {
        get
        {
            return "石化";
        }
    }

    // 技能描述
    public override string SkillDetail
    {
        get
        {
            return "第<color=#ff2e63> 5 </color>回合时石化\n\n<color=#ff2e63>无法攻击</color>";
        }  
    }

    // 初始化方法
    public override void Init(SkillManager skillManager, BasicCharacter character)
    {
        base.Init(skillManager, character);
        skillType = SkillType.Passive;
        GameInit.Instance.OnNextRound += PetrifySelf;
    }

    private void PetrifySelf()
    {
        petrifyLayer++;
        if (petrifyLayer >= 5)
        {   
            // 禁止攻击
            skillManager.character.cantAttack = true;
            // 取消监听
            GameInit.Instance.OnNextRound -= PetrifySelf;
            // 展示石化文字
            skillManager.character.ShowText("石化！", Color.white);
            // 音效
            AudioManager.Get.PlaySound(skillAudio);
        }
    }

    // 使用方法
    public override bool Use(Hero hero, Enemy target = null)
    {
        return true;
    }
}