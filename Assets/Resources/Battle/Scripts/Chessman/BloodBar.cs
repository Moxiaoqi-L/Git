using UnityEngine;
using UnityEngine.UI;

public class DelayedHealthSystem : MonoBehaviour
{
    [Header("References")]
    public Slider immediateBar;    // 即时血条
    public Slider delayedBar;       // 延迟血条

    [Header("Settings")]
    [Range(0, 100)] public float maxHealth = 100;
    public float damageDelaySpeed = 3f;   // 扣血延迟动画速度
    public float healSpeed = 8f;          // 加血动画速度
    public float reduceSpeed=20f;
    public float addSpeed = 20f;
    private float currentHealth;
    private float delayedHealth;

    void Start()
    {
        currentHealth = delayedHealth = maxHealth;
        immediateBar.maxValue = delayedBar.maxValue = maxHealth;
    }

    void Update()
    {
        HandleInput();
        UpdateBars();
    }
    //这里改伤害
    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
            currentHealth = Mathf.Max(0, currentHealth - reduceSpeed);

        if (Input.GetMouseButtonDown(1))
            currentHealth = Mathf.Min(maxHealth, currentHealth + addSpeed);
    }

    void UpdateBars()
    {
        // 即时血条直接跟随当前血量
        immediateBar.value = currentHealth;

        // 延迟血条使用平滑过渡
        delayedHealth = Mathf.Lerp(
            delayedHealth,
            currentHealth,
            (currentHealth < delayedHealth ? damageDelaySpeed : healSpeed) * Time.deltaTime
        );
        delayedBar.value = delayedHealth;
    }
   
}