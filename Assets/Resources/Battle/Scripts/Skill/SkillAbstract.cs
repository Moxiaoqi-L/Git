// 技能基类，继承自 ScriptableObject
using UnityEngine;

public abstract class Skill : ScriptableObject
{
    public string skillName;
    public string skillDetail;
    public abstract void Use(Hero hero, Enemy target = null);
}