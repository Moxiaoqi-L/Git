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

    // 组件相关
    private Chessman currentChessman;
    private Hero hero;
    private Enemy enemy;

    // 记录上一帧选中的棋子
    private Chessman lastSelectedChessman; 
    // 属性变化触发更新标记
    private bool needUpdateUI = false;     

    // 对象池
    private ObjectPool colorPointPool;

    private void Start()
    {
        // 初始化对象
        colorPointPool = new ObjectPool(needColorPointPrefab, 4, needColorPointField.transform);
    }

    // 更新技能消耗时复用对象
    private void UpdateSkillCostDisplay()
    {
        colorPointPool.ReturnAll(); // 隐藏所有闲置对象
        foreach (Color color in hero.activeSkill.Costs)
        {
            GameObject point = colorPointPool.Get();
            point.GetComponent<Image>().color = color;
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
        Debug.Log("开始监听了！");
        if (hero != null) 
        {
            hero.OnHealthPointsChanged += UpdateHPDisplay;
            hero.OnDefenseChanged += UpdateDefenseDisplay;
            hero.OnAttackChanged += UpdateAttackDisplay;
            // hero.OnMDefenseChanged += UpdateMDefenseDisplay;
        }
        // if (enemy != null) enemy.OnHealthPointsChanged += UpdateHPDisplay;
    }

    private void OnDisable()
    {
        Debug.Log("取消监听了！");
        if (hero != null) 
        {
            hero.OnHealthPointsChanged -= UpdateHPDisplay;
        }
        // if (enemy != null) enemy.OnHealthPointsChanged -= UpdateHPDisplay;
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
        magicDefense.text = "精神抗性：" + hero.GetActualMagicDefense().ToString();
    }

    private void UpdateAttackDisplay(BasicCharacter character)
    {
        attack.text = "攻击力：" + character.GetActualAttack();
    }

    // 拆分详细更新逻辑（仅在需要时调用）
    private void RefreshCharacterDetails()
    {
        hero = SelectCore.Selection.hero;
        // enemy = SelectCore.Selection.enemy;

        // 基础属性更新（仅在选中时执行一次）
        if (hero != null)
        {
            OnEnable();
            UpdateHPDisplay(hero);
            UpdateDefenseDisplay(hero);
            UpdateAttackDisplay(hero);
            UpdateMDefenseDisplay(hero);

            UpdateSkillCostDisplay();
            // 静态属性仅在选中时更新一次
            damageTypeImage.sprite = hero.damageTypeImage;

            characterName.text = hero.characterAttributes.name;
            characterImage.sprite = hero.characterImage;

            activeSkillName.text = hero.activeSkill.SkillName;
            activeSkillDetail.text = hero.activeSkill.SkillDetail;
            passiveSkillName.text = hero.passiveSkill.SkillName;
            passiveSkillDetail.text = hero.passiveSkill.SkillDetail; 
            passiveSkillImage.sprite = hero.passiveSkillImage;
        }
    }
}
