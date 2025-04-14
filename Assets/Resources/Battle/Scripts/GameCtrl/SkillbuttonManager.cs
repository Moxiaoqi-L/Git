using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class SkillButtonManager : MonoBehaviour
{
    public static SkillButtonManager Get = null;
    public Transform buttonParent; // 技能按钮的父物体
    private List<Button> skillButtons = new List<Button>();

    private void Awake() {
        if (Get == null)
        {
            Get = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        // 初始化技能按钮列表
        foreach (Transform child in buttonParent)
        {
            Button button = child.GetComponent<Button>();
            if (button != null)
            {
                skillButtons.Add(button);
            }
        }
    }

    public void RemoveSkillButton(Hero hero)
    {
        // 找到该英雄对应的技能按钮并移除
        for (int i = 0; i < skillButtons.Count; i++)
        {
            // 这里假设按钮上有一个自定义的 Hero 引用，可以根据实际情况修改判断条件
            if (skillButtons[i].GetComponent<SkillButton>()?.associatedHero == hero)
            {
                RemoveButton(i);
                break;
            }
        }
    }

    private void RemoveButton(int index)
    {
        Button buttonToRemove = skillButtons[index];

        // 左移消失动画
        buttonToRemove.transform.DOLocalMoveX(-buttonToRemove.GetComponent<RectTransform>().sizeDelta.x, 0.3f).OnComplete(() =>
        {
            // 移除按钮
            skillButtons.RemoveAt(index);
            Destroy(buttonToRemove.gameObject);

            // 重新排序按钮
            RearrangeButtons();
        });
    }

    private void RearrangeButtons()
    {
        float buttonWidth = skillButtons[0].GetComponent<RectTransform>().sizeDelta.x + 20;
        float startX = -buttonWidth * (skillButtons.Count - 1) / 2f;

        for (int i = 0; i < skillButtons.Count; i++)
        {
            Vector3 targetPosition = new(startX + i * buttonWidth, 0, 0);
            skillButtons[i].transform.DOLocalMove(targetPosition, 0.3f);
        }
    }
}