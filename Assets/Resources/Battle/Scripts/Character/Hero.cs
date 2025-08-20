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
    private new void Start()
    {
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
        if (hasAttacked || isStunned || cantAttack) yield break;
        // 检查是否行动过 TODO
        // 获取攻击点数
        ColorPointCtrl.Get.GetColorPoint(transform, chessman.location.y);
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
        // 行动完成
        CompleteAction();
        // 等待动画回调触发
        yield return null; 
    }

    // 行动完成（包括攻击或释放技能）
    public void CompleteAction()
    {
        // 使头像变灰色
        MakeImageGray();
        // 禁止移动
        chessmanMove.enabled = false;
        // 完成攻击
        hasAttacked = true;
    }

    // 玩家回合开始时会执行
    public void StartOfTurn()
    {
        // 恢复头像
        RestoreImageColor();
        // 允许攻击
        hasAttacked = false;
        // 恢复移动
        chessmanMove.enabled = true;
    }

    // 玩家回合结束时调用
    public override void EndOfRound()
    {
        base.EndOfRound();
    }
    
    // 攻击完成事件
    public void FinishAttack(Enemy targetEnemy = null)
    {
        OnAttackCompleted?.Invoke(this, targetEnemy);
    }

    // 重置自身状态（清除BUFF, 允许攻击, 允许移动）
    public override void RefreshSelf()
    {
        base.RefreshSelf();
        // 恢复头像
        RestoreImageColor();
        // 恢复位移
        chessmanMove.enabled = true;
        // 允许攻击
        hasAttacked = false;
    }

    // 使头像变灰, 一般用于行动后
    private void MakeImageGray()
    {
        Color grayColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        image.DOColor(grayColor, 0.5f);
    }

    // 使头像恢复正常
    public void RestoreImageColor()
    {
        Color normalColor = Color.white;
        image.DOColor(normalColor, 0.5f);
    }

    // 触发死亡台词
    protected override void OnDeath()
    {
        TriggerLine(LineEventType.Death);
    }

    // 攻击方法协程
    private IEnumerator CalculateDamage(Enemy target)
    {

        // 触发攻击前事件
        EventManager.TriggerBeforeAttack(target);

        bool isCritical = UnityEngine.Random.value * 100 <= GetActualCriticalRate();
        float damage = GetActualAttack() * (1 + GetActualDamagePower() / 100f);
        if (isCritical)
        {
            // 计算暴击伤害。暴击伤害 = 攻击伤害 * ( 暴击伤害修改 / 100 )
            damage *= GetActualCriticalDamageMultiplier() / 100;
        }
        // 触发攻击时事件
        // 将伤害送给目标进行防御结算
        target.Defend(damage, characterAttributes.damageType, from : this);
        // 触发攻击后事件
        EventManager.TriggerAfterAttack(target, damage);
        // 完成攻击
        FinishAttack(target);
        yield return null;
    }
}    