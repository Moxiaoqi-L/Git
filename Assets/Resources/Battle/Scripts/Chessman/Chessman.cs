using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Chessman : MonoBehaviour, IPointerClickHandler 
{
    // 棋子的坐标
    public Location location;
    // 棋子的阵营
    public Camp camp;
    // 棋子所在的方格
    public Square Square => BoardCtrl.Get[location];
    // 选择边框
    private Image selectionBorder;

    public Hero hero;
    public Enemy enemy;

    // 最高层级UI
    public Transform topUI;

    // 技能按钮实例化
    public GameObject skillButtonPrefab;
    // 当前技能按钮
    private GameObject currentSkillButton;

    // 新增：移动完成事件（供Hero监听）
    public event System.Action<Hero> OnMoveCompleted;


    // 初始化棋子
    public void Start()
    {
        if (camp == Camp.Neutral)
        {
            Debug.LogError("棋子阵营不能为中立。");
            return;
        }
        if (camp == Camp.Player)
        {
            hero = GetComponent<Hero>();
        }
        if (camp == Camp.Enemy)
        {
            enemy = GetComponent<Enemy>();
        }
        InitMove(location);
        selectionBorder = GameObject.Find("Rarity").GetComponentInChildren<Image>();
        topUI = GameObject.Find("Canvas/TopUI").transform;
    }

    // 点击事件
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 右键点击逻辑
            OnRightClick();
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            // 原有左键逻辑
            OnChessmanClicked();
        }        
    }
 
    // 默认输出
    public override string ToString()
    {
        return $"棋子坐标:{location} 阵营:{camp}";
    }
 
    // 获取当前场上的全部棋子，或者某一方的全部棋子。
    public static List<Chessman> All(Camp camp = Camp.Neutral)
    {
        List<Chessman> ret = new List<Chessman>();
        var chessmen = FindObjectsOfType<Chessman>();
        foreach (var chessman in chessmen)
        {
            if (camp == Camp.Neutral || camp == chessman.camp)
            {
                ret.Add(chessman);
            }
        }
        return ret;
    }

    // 获取当前场上的全部英雄
    public static List<Hero> AllHeros()
    {
        List<Hero> ret = new();
        var chessmen = FindObjectsOfType<Chessman>();
        foreach (var chessman in chessmen)
        {
            if (chessman.camp == Camp.Player)
            {
                ret.Add(chessman.hero);
            }
        }
        return ret;
    }

    // 获取当前场上的全部敌人
    public static List<Enemy> AllEnemies()
    {
        List<Enemy> ret = new();
        var chessmen = FindObjectsOfType<Chessman>();
        foreach (var chessman in chessmen)
        {
            if (chessman.camp == Camp.Enemy)
            {
                ret.Add(chessman.enemy);
            }
        }
        return ret;
    }

    // 清除场上的全部棋子
    public static void ClearAll()
    {
        var all = All();
        for (int i = all.Count - 1; i >= 0; i--)
        {
            all[i].ExitFromBoard();
        }
    }

    // 依照坐标查询，找到位于相应坐标上的棋子
    public static Chessman GetChessman(Location location)
    {
        foreach (var chessman in All())
        {
            if (chessman.location.Equals(location))
            {
                return chessman;
            }
        }
        return null;
    }
 
    // 使棋子移动到指定坐标
    public void InitMove(Location target)
    {
        //定位目标棋盘格
        Square square = BoardCtrl.Get[target.x, target.y];
        //修改自身坐标为新的坐标
        location = target;
        // 瞬间移动
        transform.position = square.transform.position;

        // 触发移动完成事件（仅英雄需要）
        if (camp == Camp.Player && hero != null)
        {
            OnMoveCompleted?.Invoke(hero); // 调用事件
        }
    }

    // 棋子选中
    private void OnChessmanClicked()
    {
        // 处理跨阵营交互（如玩家选中敌人）
        if (!HandleCrossCampInteraction())
        {
            // 处理自身选中状态
            HandleSelfSelection();
        }

    }

    private void OnRightClick()
    {
        if (camp == Camp.Player && hero != null)
        {

            ShowSkill(hero);
        }   
    }

    // 处理自身选中状态
    private void HandleSelfSelection()
    {
        if (SelectCore.Selection == this)
        {
            // 重复点击：取消选中
            DeselectSelf();
        }
        else
        {
            // 首次点击：选中并处理玩家专属逻辑
            SelectSelf();
        }
    }

    // 处理跨阵营选取
    private bool HandleCrossCampInteraction()
    {
        // 玩家点击敌人：触发攻击逻辑（仅当当前选中的是玩家英雄）
        if (camp == Camp.Enemy && SelectCore.Selection != null && 
            SelectCore.Selection.camp == Camp.Player && 
            SelectCore.Selection.hero != null)
        {
            Hero selectedHero = SelectCore.Selection.hero;
            if (selectedHero.GetAttackRange().Contains(location))
            {
                StartCoroutine(selectedHero.Attack(enemy)); // 执行攻击
                return true;
            }
            else
            {
                Debug.LogWarning($"目标 {enemy.characterAttributes.name} 超出攻击范围！");
                return false;
                // 播放提示音效或显示UI警告
            }
        }
        return false;
    }

    // 选择棋子
    private void SelectSelf()
    {

        if (SelectCore.Selection != null && SelectCore.Selection.camp == Camp.Enemy && camp == Camp.Enemy) 
        {
            HighlightAttackRange(SelectCore.Selection.enemy, false);
            HighlightAttackRange(enemy, true); // 显示敌人攻击范围
            SelectCore.TrySelect(this);
            return;
        }

        if (SelectCore.Selection != null && SelectCore.Selection != this)
        {
            if (SelectCore.Selection.hero) HighlightAttackRange(SelectCore.Selection.hero, false);
            if (SelectCore.Selection.enemy) HighlightAttackRange(SelectCore.Selection.enemy, false);
        }

        SelectCore.TrySelect(this);
        
        if (camp == Camp.Player && hero != null)
        {
            HighlightAttackRange(hero, true); // 显示玩家攻击范围
        }

        if (camp == Camp.Enemy && enemy != null)
        {
            HighlightAttackRange(enemy, true); // 显示敌人攻击范围
        }
    }

    // 取消选择
    private void DeselectSelf()
    {
        SelectCore.DropSelect();
        
        if (camp == Camp.Player && hero != null)
        {
            HighlightAttackRange(hero, false); // 隐藏攻击范围
        }
        if (camp == Camp.Enemy && enemy != null)
        {
            HighlightAttackRange(enemy, false); // 隐藏攻击范围
        }
    }

    // 高亮显示攻击范围
    public void HighlightAttackRange(BasicCharacter bs, bool isActive)
    {
        if (bs == null || BoardCtrl.Get == null) return;

        bool isEnemy = false;
        if (bs is not Hero) isEnemy = true;
        
        foreach (Square square in BoardCtrl.Get.squares)
        {
            bool shouldHighlight = bs.GetAttackRange().Contains(square.location);
            if (shouldHighlight)
            {
                square.SetAttackRangeHighlight(isActive, isEnemy);
            }
        }
    }

    // 棋子退场的逻辑
    public void ExitFromBoard()
    {
        StartCoroutine(WaitForExit());
    }
    private IEnumerator WaitForExit()
    {
        yield return new WaitForSeconds(0.5f);
        // 播放退场动画
        transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
        {
            if (camp == Camp.Enemy)
            {
                GameInit.Instance.OnEnemyExitHandler();
                Destroy(gameObject);
            }
            if (camp == Camp.Player)
            {
                GameInit.Instance.OnHeroExitHandler(hero);
                Destroy(gameObject);                
            }

        });
    }

    private void ShowSkill(Hero hero)
    {
        // 销毁已存在的技能按钮（如果有）
        if (currentSkillButton != null)
        {
            // 让按钮出现在人物上方
            currentSkillButton.transform.DOMoveY(transform.position.y, 0.2f).OnComplete(() =>{
                Destroy(currentSkillButton);
            });
            return;
        }
        if (hero.activeSkill != null)
        {
            // 实例化技能按钮预制体
            GameObject buttonObj = Instantiate(skillButtonPrefab, topUI.transform);
            // 设定技能按钮的位置
            buttonObj.GetComponent<RectTransform>().position = transform.position;
            // 让按钮出现在人物上方
            buttonObj.transform.DOMoveY(transform.position.y + 80, 0.2f);
            // 配置按钮文本和点击事件
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = hero.activeSkill[0].SkillName;
            buttonObj.GetComponent<Button>().onClick.AddListener(() => UseSkill(hero.activeSkill[0], hero));
            currentSkillButton = buttonObj;
        }
    }

    public void DestroySkillButton()
    {
        if (currentSkillButton != null)
        {
            Destroy(currentSkillButton);
        }
    }

    private void UseSkill(Skill skill, Hero hero)
    {
        // 触发台词
        if (skill.Use(hero)) hero.TriggerLine(LineEventType.SkillActive);
        // 隐藏技能菜单（点击后销毁按钮）
        Destroy(currentSkillButton);
    }
}