using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 英雄类，代表游戏中的英雄角色，包含英雄的各种属性和行为
public abstract class BasicCharacter : MonoBehaviour
{
    // 获取棋子
    protected Chessman chessman;
    // 管理英雄 BUFF 的对象
    protected BuffManager buffManager;
    // 攻击动画
    public IAttackAnimation attackAnimation;
    // 英雄拥有的技能列表
    public List<Skill> skills = new List<Skill>();

    // 当前生命值
    public int currentHealthPoints;
    // 临时攻击力
    public int provisionalAttack;
    // 临时防御
    public int provisionalDefense;
    
    // 初始化英雄的技能列表
    protected abstract void InitializeSkills();

    // 增加攻击力的方法
    public void IncreaseAttack(int amount)
    {
        provisionalAttack += amount;
    }

    // 增加防御力的方法
    public void IncreaseDefense(int amount)
    {
        provisionalDefense += amount;
    }

    // 添加 BUFF 的方法，通过 BUFF 名称调用 BuffManager 的添加方法
    public void AddBuff(string buffName, int duration)
    {
        // 调用 BuffManager 的 AddBuff 方法添加 BUFF
        buffManager.AddBuff(buffName, duration);
    }

    // 移除 BUFF 的方法，通过 BUFF 名称调用 BuffManager 的移除方法
    public void RemoveBuff(string buffName)
    {
        // 调用 BuffManager 的 RemoveAttackBuff 方法移除攻击类 BUFF
        buffManager.RemoveBuff(buffName);
    }
}    