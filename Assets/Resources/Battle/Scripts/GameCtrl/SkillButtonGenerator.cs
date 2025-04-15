using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;

public class SkillButtonGenerator : MonoBehaviour
{
    public GameObject skillPrefab; // Skill 预制体
    public GameObject needsColorPrefab; // 需要的颜色点数预制体
    public Transform buttonParent; // 按钮的父对象，用于布局

    private List<Skill> allSkills = new();
    private List<GameObject> allSkillsGameObjects = new();

    private void Start()
    {
        // 获取所有 Hero 实例
        Hero[] allHeroes = FindObjectsOfType<Hero>();

        // 收集所有技能
        foreach (Hero hero in allHeroes)
        {
            allSkills.AddRange(hero.skills);
        }
        // 生成 Skill 预制体
        foreach (Skill skill in allSkills)
        {
            // 实例化技能按钮
            GameObject buttonObj = Instantiate(skillPrefab, buttonParent);
            // 技能按钮列表
            allSkillsGameObjects.Add(buttonObj);
            // 获取技能按钮的 button 组件
            Button button = buttonObj.GetComponent<Button>();
            // 绑定对应的英雄
            buttonObj.GetComponent<SkillButton>().associatedHero = allHeroes[allSkills.IndexOf(skill)];
            // 获取技能按钮的 TextMeshPro 组件
            TextMeshProUGUI tmpText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            // 修改技能名称
            tmpText.text = skill.skillName;
            // 为按钮添加点击事件
            button.onClick.AddListener(() =>
            {
                // 设定技能对应的英雄
                if (allHeroes.Length > 0)
                {
                    skill.Use(allHeroes[allSkills.IndexOf(skill)]);
                }
            });
            // 为技能按钮设置显示需求的颜色点数
            for (int i = 0; i < skill.costs.Length; i++)
            {
                GameObject colorCost = Instantiate(needsColorPrefab, buttonObj.transform.GetChild(0).transform);
                colorCost.GetComponent<Image>().color = skill.costs[i];
                RectTransform rtf = colorCost.GetComponent<RectTransform>();
                rtf.localPosition = new Vector3(rtf.localPosition.x, rtf.localPosition.y - i * 18, rtf.localPosition.z);
                // colorCost.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0.5f);
                // colorCost.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.5f);
            }
        }
        RearrangeSkills();
    }
    // 技能排列
    private void RearrangeSkills()
    {
        float skillHeight = skillPrefab.GetComponent<RectTransform>().sizeDelta.y + 50;
        float totalHeight = skillHeight * allSkills.Count;
        float startY = -totalHeight / 2f + skillHeight / 2f;

        for (int i = 0; i < allSkills.Count; i++)
        {
            Vector3 targetPosition = new(0, startY + i * skillHeight, 0);
            allSkillsGameObjects[i].transform.localPosition = targetPosition;
        }
    }
}    