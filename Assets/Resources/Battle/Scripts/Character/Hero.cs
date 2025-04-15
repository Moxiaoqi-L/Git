using UnityEngine;
using DG.Tweening;
using System;

// 英雄类，代表游戏中的英雄角色，包含英雄的各种属性和行为
public class Hero : BasicCharacter
{
    // 英雄的图片
    private ChessmanMove chessmanMove;
    // 每回合只能攻击一次
    public bool hasAttacked;

    protected override void InitializeSkills()
    {
        if (characterAttributes.skills != null)
        {
            foreach (Skill skill in characterAttributes.skills)
            {
                if (skill != null)
                {
                    skills.Add(skill);
                }
            }  
        }
    }

    // 当对象启用时调用的方法，用于初始化和执行一些操作
    private new void Start() {
        base.Start();
        // 初始化技能列表
        InitializeSkills();
        // 初始化生命值
        currentHealthPoints = characterAttributes.maxHealthPoints;
        // 获取移动方式
        chessmanMove = GetComponent<ChessmanMove>();
        // 初始允许攻击
        hasAttacked = false;          
    }

    // 英雄的攻击方法，用于对敌人造成伤害，新增 selfAttack 参数用于控制是否自我攻击
    public void Attack(Enemy target)
    {
        if (hasAttacked)
        {
            return;
        }
        // 播放攻击动画
        attackAnimation.PlayAttackAnimation(this.transform, target.transform);
        // 计算命中率
        float hitRate = characterAttributes.accuracy / (characterAttributes.accuracy + target.characterAttributes.evasion);
        float randomValue = UnityEngine.Random.value;
        if (randomValue <= hitRate)
        {
            float actualattack = GetActualAttack();
            bool isCritical = UnityEngine.Random.value <= characterAttributes.criticalRate;
            float damage = actualattack;
            if (isCritical)
            {
                damage *= characterAttributes.criticalDamageMultiplier;
                Debug.Log(characterAttributes.name + " 暴击了！ ");
            }
            Debug.Log(characterAttributes.name + " 攻击了！ ");
            target.Defend(damage);
        }
        else
        {
            Debug.Log(characterAttributes.name + " 攻击落空！ ");
        }
        // 获取攻击点数
        ColorPointCtrl.Get.GetColorPoint(this.transform, this.chessman.location.y);
        // 完成攻击
        FinishAttack();
    }

    public float GetActualAttack(){
        return characterAttributes.attack;
    }

    // 每回合结束时调用，
    public override void EndOfRound()
    {
        base.EndOfRound();
        // 恢复头像
        RestoreImageColor();
        // 设置允许攻击
        hasAttacked = false;
        // 恢复位移
        chessmanMove.enabled = true;
    }

    public void FinishAttack(){
        // 设置已经攻击
        hasAttacked = true;
        // 头像变灰
        MakeImageGray();
        // 禁止位移
        chessmanMove.enabled = false;
    }

    // 图片变灰的方法
    private void MakeImageGray()
    {
        Color grayColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        image.DOColor(grayColor, 0.5f);
    }

    // 图片恢复正常的方法
    public void RestoreImageColor()
    {
        hasAttacked = false; // 重置攻击状态
        Color normalColor = Color.white;
        image.DOColor(normalColor, 0.5f);
    }

    protected override void OnDeath(){}

}    