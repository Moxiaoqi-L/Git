using UnityEngine;

// 技能基类，所有具体技能都应继承自此类
// 提供了技能名称和使用技能的抽象方法
public abstract class Skill
{
    // 技能的名称，用于标识不同的技能
    public string skillName;

    // 构造函数，获取技能名称
    public Skill(string name)
    {
        skillName = name;
    }

    // 抽象方法，具体技能需要实现该方法以定义技能的使用逻辑
    public abstract void Use(Hero hero, Enemy target = null);
}  