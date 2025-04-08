using UnityEngine;
using UnityEngine.UI;

public class DelayedHealthSystem : MonoBehaviour
{
    [Header("References")]
    public Slider immediateBar;    // ��ʱѪ��
    public Slider delayedBar;       // �ӳ�Ѫ��

    [Header("Settings")]
    [Range(0, 100)] public float maxHealth = 100;
    public float damageDelaySpeed = 3f;   // ��Ѫ�ӳٶ����ٶ�
    public float healSpeed = 8f;          // ��Ѫ�����ٶ�
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
    //������˺�
    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
            currentHealth = Mathf.Max(0, currentHealth - reduceSpeed);

        if (Input.GetMouseButtonDown(1))
            currentHealth = Mathf.Min(maxHealth, currentHealth + addSpeed);
    }

    void UpdateBars()
    {
        // ��ʱѪ��ֱ�Ӹ��浱ǰѪ��
        immediateBar.value = currentHealth;

        // �ӳ�Ѫ��ʹ��ƽ������
        delayedHealth = Mathf.Lerp(
            delayedHealth,
            currentHealth,
            (currentHealth < delayedHealth ? damageDelaySpeed : healSpeed) * Time.deltaTime
        );
        delayedBar.value = delayedHealth;
    }
   
}