using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class CharacterDetail : MonoBehaviour
{
    // 通过监听事件改变攻击力 防御 精神抗性 生命值 
    public TextMeshProUGUI attack;
    public TextMeshProUGUI defense;
    public TextMeshProUGUI magicDefense;
    public TextMeshProUGUI HP;

    // 静态改变
    public Image characterImage;
    public Image damageTypeImage;
    public Image passiveSkillImage;
    public Image activeSkillImage;

    public TextMeshProUGUI characterName;
    public TextMeshProUGUI activeSkillName;
    public TextMeshProUGUI passiveSkillName;
    public TextMeshProUGUI activeSkillDetail;
    public TextMeshProUGUI passiveSkillDetail;
    
    public GameObject needColorPointField;
    public GameObject needColorPointPrefab;
    public GameObject morePassiveSkillField;
    public GameObject morePassiveSkillPrefab;

    // 组件相关
    private Chessman currentChessman;
    private Hero hero;
    private Enemy enemy;

    // 无技能图片缓存
    private Sprite noSkillSprite;

    // 记录上一帧选中的棋子
    private Chessman lastSelectedChessman; 
    // 属性变化触发更新标记
    private bool needUpdateUI = false;     

    // 对象池
    private ObjectPool colorPointPool;
    // 对象池
    private ObjectPool morePassiveButtonPool;
    // 监听图片变化
    public Action spriteChanged;

    // BUFF 图标父容器
    public Transform buffIconContainer;
    // BUFF 图标预制体（需在Inspector赋值）
    public GameObject buffIconPrefab;
    // 缓存已生成的图标实例（避免重复创建）
    private Dictionary<string, GameObject> existingBuffIcons = new();

    private void Start()
    {
        // 初始化对象
        colorPointPool = new ObjectPool(needColorPointPrefab, 4, needColorPointField.transform);
        morePassiveButtonPool = new ObjectPool(morePassiveSkillPrefab, 4, morePassiveSkillField.transform);
        noSkillSprite = Resources.Load<Sprite>("General/Image/UI/NoSkill");
    }

    // 更新技能消耗时复用对象
    private void UpdateSkillCostDisplay()
    {
        colorPointPool.ReturnAll(); // 隐藏所有闲置对象
        if (hero != null)
        {
            foreach (Color color in hero.activeSkill[0].Costs)
            {
                GameObject point = colorPointPool.Get();
                point.GetComponent<Image>().color = color;
            }
        }
    }

private void UpdateMorePassiveSkillDisplay()
{
    morePassiveButtonPool.ReturnAll(); // 回收所有按钮
    if (enemy == null || enemy.passiveSkill.Count == 0) return;

    // 创建临时列表存储按钮，按技能顺序管理
    List<GameObject> buttons = new List<GameObject>();
    int index = 1;

    foreach (Skill currentSkill in enemy.passiveSkill)
    {
        GameObject button = morePassiveButtonPool.Get();
        button.GetComponentInChildren<TextMeshProUGUI>().text = index.ToString();
        buttons.Add(button); // 按顺序添加到列表

        // 捕获当前技能和索引
        button.GetComponent<Button>().onClick.AddListener(() => {
            passiveSkillName.text = currentSkill.SkillName;
            passiveSkillDetail.text = currentSkill.SkillDetail;
            passiveSkillImage.sprite = currentSkill.skillSprite;
        });
        index++;
    }

    // 确保按钮按生成顺序排列（若布局组件导致顺序混乱，可强制排序）
    for (int i = 0; i < buttons.Count; i++)
    {
        buttons[i].transform.SetSiblingIndex(i); // 按索引设置层级顺序
    }
}
    
    private void Update()
    {
        // 仅当选中对象变化时触发更新
        if (SelectCore.Selection != lastSelectedChessman)
        {
            OnDisable();
            lastSelectedChessman = SelectCore.Selection;
            needUpdateUI = true;
        }

        if (needUpdateUI && lastSelectedChessman != null)
        {
            RefreshCharacterDetails();
            needUpdateUI = false; // 重置标记
        }
    }

    private void OnEnable()
    {
        // Debug.Log("开始监听了！");
        if (hero != null) 
        {
            hero.EventManager.OnHealthPointsChanged += UpdateHPDisplay;
            hero.EventManager.OnDefenseChanged += UpdateDefenseDisplay;
            hero.EventManager.OnAttackChanged += UpdateAttackDisplay;
            // hero.OnMDefenseChanged += UpdateMDefenseDisplay;
        }
        if (enemy != null) 
        {
            enemy.EventManager.OnHealthPointsChanged += UpdateHPDisplay;
            enemy.EventManager.OnDefenseChanged += UpdateDefenseDisplay;
            enemy.EventManager.OnAttackChanged += UpdateAttackDisplay;
        }
    }

    private void OnDisable()
    {
        // Debug.Log("取消监听了！");
        if (hero != null) 
        {
            hero.EventManager.OnHealthPointsChanged -= UpdateHPDisplay;
            hero.EventManager.OnDefenseChanged -= UpdateDefenseDisplay;
            hero.EventManager.OnAttackChanged -= UpdateAttackDisplay;
        }
        if (enemy != null) 
        {
            enemy.EventManager.OnHealthPointsChanged -= UpdateHPDisplay;
            enemy.EventManager.OnDefenseChanged -= UpdateDefenseDisplay;
            enemy.EventManager.OnAttackChanged -= UpdateAttackDisplay;
        }
    }

    private void UpdateHPDisplay(BasicCharacter character)
    {
        HP.text = $"生命值：{character.currentHealthPoints} / {character.characterAttributes.maxHealthPoints}";
        
    }

    private void UpdateDefenseDisplay(BasicCharacter character)
    {
        defense.text = "防御：" + character.GetActualDefense();
    }

    private void UpdateMDefenseDisplay(BasicCharacter character)
    {
        magicDefense.text = "精神抗性：" + character.GetActualMagicDefense().ToString();
    }

    private void UpdateAttackDisplay(BasicCharacter character)
    {
        attack.text = "攻击力：" + character.GetActualAttack();
    }

    // 拆分详细更新逻辑（仅在需要时调用）
    private void RefreshCharacterDetails()
    {
        hero = SelectCore.Selection.hero;
        enemy = SelectCore.Selection.enemy;

        // 清空旧图标
        existingBuffIcons.Clear();
        foreach (Transform child in buffIconContainer)
        {
            Destroy(child.gameObject);
        }

        // 基础属性更新（仅在选中时执行一次）
        if (hero != null)
        {
            OnEnable();
            UpdateHPDisplay(hero);
            UpdateDefenseDisplay(hero);
            UpdateAttackDisplay(hero);
            UpdateMDefenseDisplay(hero);
            UpdateSkillCostDisplay();
            UpdateMorePassiveSkillDisplay();

            GenerateBuffIcons(hero.buffManager.activeBuffs);

            // 静态属性仅在选中时更新一次
            damageTypeImage.sprite = hero.damageTypeImage;

            characterName.text = hero.characterAttributes.name;
            characterImage.sprite = hero.characterImage;
            spriteChanged?.Invoke();
            if (hero.activeSkill.Count > 0)
            {
                activeSkillName.text = hero.activeSkill[0].SkillName;
                activeSkillDetail.text = hero.activeSkill[0].SkillDetail;
                activeSkillImage.sprite = hero.activeSkill[0].skillSprite;
            }
            if (hero.passiveSkill.Count > 0)
            {
                passiveSkillName.text = hero.passiveSkill[0].SkillName;
                passiveSkillDetail.text = hero.passiveSkill[0].SkillDetail; 
                passiveSkillImage.sprite = hero.passiveSkill[0].skillSprite;
            }
        }
        if (enemy != null)
        {
            OnEnable();
            UpdateHPDisplay(enemy);
            UpdateDefenseDisplay(enemy);
            UpdateAttackDisplay(enemy);
            UpdateMDefenseDisplay(enemy);
            UpdateSkillCostDisplay();
            UpdateMorePassiveSkillDisplay();

            GenerateBuffIcons(enemy.buffManager.activeBuffs);

            // 静态属性仅在选中时更新一次
            damageTypeImage.sprite = enemy.damageTypeImage;
            characterName.text = enemy.characterAttributes.name;
            characterImage.sprite = enemy.characterImage;
            spriteChanged?.Invoke();
            if (enemy.activeSkill.Count > 0)
            {
                activeSkillName.text = enemy.activeSkill[0].SkillName;
                activeSkillDetail.text = enemy.activeSkill[0].SkillDetail;
            }
            else
            {
                activeSkillName.text = "";
                activeSkillDetail.text = "";
                activeSkillImage.sprite = noSkillSprite;
            }
            if (enemy.passiveSkill.Count > 0)
            {
                passiveSkillName.text = enemy.passiveSkill[0].SkillName;
                passiveSkillDetail.text = enemy.passiveSkill[0].SkillDetail; 
                passiveSkillImage.sprite = enemy.passiveSkill[0].skillSprite;
            }
            else
            {
                passiveSkillName.text = "";
                passiveSkillDetail.text = "";         
                passiveSkillImage.sprite = noSkillSprite;
            }
        }
    }
    // 生成 BUFF 图标
    private void GenerateBuffIcons(Dictionary<string, Buff> activeBuffs)
    {
        int index = 0;
        foreach (var buffEntry in activeBuffs)
        {
            Buff buff = buffEntry.Value;
            GameObject iconInstance = Instantiate(buffIconPrefab, buffIconContainer);
            
            // 设置图标（假设每个 BUFF 有对应的图标资源，路径为 "BuffIcons/{buffName}"）
            // 传入详细信息给detail
            Sprite buffSprite = buff.buffSprite;
            iconInstance.GetComponent<Image>().sprite = buffSprite;
            iconInstance.GetComponent<BuffDetail>().title = buff.buffName;
            iconInstance.GetComponent<BuffDetail>().content = buff.buffDetail;
            
            // 显示层数（如果有）
            TextMeshProUGUI layerText = iconInstance.GetComponentInChildren<TextMeshProUGUI>();
            if (layerText != null && buff.stackLayers > 1)
            {
                layerText.text = buff.stackLayers.ToString();
            }
            else
            {
                layerText.gameObject.SetActive(false); // 层数为1时隐藏数字
            }
            // 缓存实例（用于刷新时快速查找）
            existingBuffIcons[buffEntry.Key] = iconInstance;
            index++;
        }
    }   
}
