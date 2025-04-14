using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class SkillButtonGenerator : MonoBehaviour
{
    public GameObject skillPrefab; // Skill 预制体
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
            GameObject buttonObj = Instantiate(skillPrefab, buttonParent);
            allSkillsGameObjects.Add(buttonObj);
            Button button = buttonObj.GetComponent<Button>();

            buttonObj.GetComponent<SkillButton>().associatedHero = allHeroes[allSkills.IndexOf(skill)];

            // 查找 TextMeshPro 组件
            TextMeshProUGUI tmpText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpText != null)
            {
                tmpText.text = skill.skillName;
            }

            // 为按钮添加点击事件
            button.onClick.AddListener(() =>
            {
                // 假设英雄是第一个英雄，你可以根据需求修改
                if (allHeroes.Length > 0)
                {
                    skill.Use(allHeroes[allSkills.IndexOf(skill)]);
                }
            });
        }
        RearrangeSkills();
    }
    // 技能排列
    private void RearrangeSkills()
    {
        float skillHeight = skillPrefab.GetComponent<RectTransform>().sizeDelta.y + 100;
        float totalHeight = skillHeight * allSkills.Count;
        float startY = -totalHeight / 2f + skillHeight / 2f;

        for (int i = 0; i < allSkills.Count; i++)
        {
            Vector3 targetPosition = new(0, startY + i * skillHeight, 0);
            allSkillsGameObjects[i].transform.localPosition = targetPosition;
        }
    }
}    