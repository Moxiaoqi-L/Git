using UnityEngine;
using DG.Tweening;
using System;
using System.Collections;

// 英雄类，代表游戏中的英雄角色，包含英雄的各种属性和行为
public class Hero : BasicCharacter
{
    // 英雄的图片
    public ChessmanMove chessmanMove;
    // 攻击完成事件（在造成伤害后触发）
    public event Action<Hero, Enemy> OnAttackCompleted;

    // 当对象启用时调用的方法，用于初始化和执行一些操作
    private new void Start() {
        base.Start();
        // 缓存图片
        characterImage = Resources.Load<Sprite>("General/Image/CharacterImage/" + characterAttributes.characterImage);
        // 初始化生命值
        currentHealthPoints = characterAttributes.maxHealthPoints;
        // 获取移动方式
        chessmanMove = GetComponent<ChessmanMove>(); 
        // 初始化技能列表
        InitializeSkills();      
    }

    // 英雄的攻击方法，用于对敌人造成伤害
    public IEnumerator Attack(Enemy target)
    {
        // 异常状态
        if (isStunned || cantAttack) yield break;
        // 检查行动点是否足够
        if (!APMPManager.Get.ConsumeAP())
        {
            ShowText("行动点不足！", Constants.REDPOINT);
            yield break;
        }
        // 获取攻击点数
        ColorPointCtrl.Get.GetColorPoint(this.transform, this.chessman.location.y);
        // 隐藏攻击范围
        chessman.HighlightAttackRange(this, false);
        // 取消选中
        SelectCore.DropSelect();
        // 攻击台词
        TriggerLine(LineEventType.Attack);
        // 播放攻击动画并等待完成
        attackAnimation.PlayAttackAnimation(
            transform,
            target.transform,
            false,
            characterAttributes.attackAnime,
            () => StartCoroutine(CalculateDamage(target)) // 动画完成后触发伤害计算
        );
        yield return null; // 等待动画回调触发
    }

    public void StartOfTurn()
    {
        // TODO
    }

    // 每回合结束时调用，
    public override void EndOfRound()
    {
        base.EndOfRound();
        // 恢复头像
        RestoreImageColor();
    }

    public void FinishAttack(Enemy targetEnemy = null){
        // 攻击完成事件
        OnAttackCompleted?.Invoke(this, targetEnemy);
    }

    public override void RefreshSelf()
    {
        base.RefreshSelf();
        // 恢复头像
        RestoreImageColor();
        // 恢复位移
        chessmanMove.enabled = true;
    }

    // 图片变灰的方法
    // private void MakeImageGray()
    // {
    //     Color grayColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    //     image.DOColor(grayColor, 0.5f);
    // }

    // 图片恢复正常的方法
    public void RestoreImageColor()
    {
        Color normalColor = Color.white;
        image.DOColor(normalColor, 0.5f);
    }

    protected override void OnDeath(){
        TriggerLine(LineEventType.Death);
    }

    // 攻击方法协程
    private IEnumerator CalculateDamage(Enemy target)
    {
        Debug.Log("进入攻击方法协程");
        // 计算命中率
        float hitRate = characterAttributes.accuracy / (characterAttributes.accuracy + target.characterAttributes.evasion);
        float randomValue = UnityEngine.Random.value;
        if (randomValue <= hitRate)
        {
            float actualattack = GetActualAttack();
            bool isCritical = UnityEngine.Random.value * 100 <= characterAttributes.criticalRate;
            float damage = actualattack * (1 + GetActualDamagePower() / 100f);
            if (isCritical)
            {
                damage *= characterAttributes.criticalDamageMultiplier / 100;
                Debug.Log(characterAttributes.name + " 暴击了！ ");
            }
            target.Defend(damage, characterAttributes.damageType, from : this);
        }
        else
        {
            Debug.Log(characterAttributes.name + " 攻击落空！ ");
        }
        // 完成攻击
        FinishAttack(target);
        yield return null;
    }
}    