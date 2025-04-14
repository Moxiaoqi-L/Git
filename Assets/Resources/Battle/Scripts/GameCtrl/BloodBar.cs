using UnityEngine;
using UnityEngine.UI;

public class DelayedHealthSystem : MonoBehaviour
{
    [Header("References")]
    public Slider immediateBar;    // 即时血条
    public Slider delayedBar;       // 延迟血条
    public Image immediateBarFill;  // 即时血条的填充图像
    public Image delayedBarFill;    // 延迟血条的填充图像

    [Header("Settings")]
    public float maxHealth;
    public float damageDelaySpeed = 3f;   // 减血延迟速度
    public float healSpeed = 8f;          // 加血速度
    public float reduceSpeed = 20f;
    public float addSpeed = 20f;
    private float currentHealth;
    private float delayedHealth;

    private Hero hero;
    private Enemy enemy;

    void Start()
    {
        
        hero = GetComponent<Hero>();
        enemy = GetComponent<Enemy>();
        Debug.Log(hero);
        Debug.Log(enemy);
        if (hero != null) maxHealth = hero.heroAttributes.maxHealthPoints;
        else if (enemy != null) maxHealth = enemy.enemyAttributes.maxHealthPoints;
        currentHealth = delayedHealth = maxHealth;
        immediateBar.maxValue = delayedBar.maxValue = maxHealth;
        
    }

    void Update()
    {
        if (hero != null) currentHealth = hero.currentHealthPoints;
        else if (enemy != null) currentHealth = enemy.currentHealthPoints;
        UpdateBars();
        UpdateBarColors();
    }

    void UpdateBars()
    {
        // 即时血条直接更新当前血量
        immediateBar.value = currentHealth;

        // 延迟血条使用平滑过渡
        delayedHealth = Mathf.Lerp(
            delayedHealth,
            currentHealth,
            (currentHealth < delayedHealth ? damageDelaySpeed : healSpeed) * Time.deltaTime
        );
        delayedBar.value = delayedHealth;

        // 根据血量情况显示或隐藏血条
        if (currentHealth >= maxHealth)
        {
            HideBloodBars();
        }
        else
        {
            ShowBloodBars();
        }

        if (currentHealth <= 0)
        {
            HideBloodBars();
        }
    }

    // 更新血条颜色
    void UpdateBarColors()
    {
        // 计算即时血条的血量比例
        float immediateHealthRatio = currentHealth / maxHealth;
        // 计算延迟血条的血量比例
        float delayedHealthRatio = delayedHealth / maxHealth;

        // 根据血量比例计算即时血条的颜色
        Color immediateColor = CalculateColor(immediateHealthRatio);
        // 根据血量比例计算延迟血条的颜色，这里可以通过调整颜色来区分
        Color delayedColor = CalculateColor(delayedHealthRatio);

        // 设置即时血条填充颜色
        immediateBarFill.color = immediateColor;
        // 设置延迟血条填充颜色
        delayedBarFill.color = delayedColor;
    }

    // 计算颜色
    Color CalculateColor(float healthRatio)
    {
        if (healthRatio >= 0.5f)
        {
            // 从绿色过渡到黄色
            return Color.Lerp(Color.yellow, Color.green, (healthRatio - 0.5f) * 2);
        }
        else
        {
            // 从黄色过渡到红色
            return Color.Lerp(Color.red, Color.yellow, healthRatio * 2);
        }
    }

    // 隐藏血条
    void HideBloodBars()
    {
        immediateBar.gameObject.SetActive(false);
        delayedBar.gameObject.SetActive(false);
    }

    // 显示血条
    void ShowBloodBars()
    {
        immediateBar.gameObject.SetActive(true);
        delayedBar.gameObject.SetActive(true);
    }
}