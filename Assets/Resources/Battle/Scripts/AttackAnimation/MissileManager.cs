using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class MissileManager : MonoBehaviour
{

    public static MissileManager Get = null;

    // 默认攻击
    public GameObject[] DefaultAttackPrefab;
    // 默认攻击音效
    public AudioClip defaultAttackHit;


    // 远程攻击的预制体
    public GameObject[] Prefabs;

    // 通过文字获取攻击动画
    private static Dictionary<string, int> getAttackAnime = new Dictionary<string, int>{
        {"黑洞" , 0},{"极客", 1},{"血色荆棘", 2}, {"魔法箭", 3}, {"炮弹", 4}, {"特质球", 5}, {"邪恶箭", 6},
        {"水晶球", 7}, {"冲天炮", 8}, {"星辰炮", 9}, {"圣诞糖果", 10}, {"暗尘箭", 11}, {"冰雹", 12}, {"激光", 13}
    };

    private void Awake() {
        Get = this;
    }

    public void DefaultAttack(Transform attacker, Transform target, bool reverse = false, Action onAnimationComplete = null)
    {
        if (reverse)
        {
            attacker.DOPunchPosition(new Vector3(0,-50,0), 0.15f, 1, 0.1f);
        }
        else
        {
            attacker.DOPunchPosition(new Vector3(0,50,0), 0.15f, 1, 0.1f);
        }
        // 实例化攻击动画
        Instantiate(DefaultAttackPrefab[0], target.position, Quaternion.identity);
        Instantiate(DefaultAttackPrefab[1], target.position, Quaternion.identity);
        onAnimationComplete?.Invoke();
        // 音效处理
        AudioManager.Instance.PlaySFX(defaultAttackHit);
    }

    public void Shoot(Transform attacker, Transform target, string attackType, Action onAnimationComplete = null)
    {
        if (getAttackAnime.TryGetValue(attackType, out int prefabIndex))
        {
            AudioClip clipHit = Resources.Load<AudioClip>(Constants.HIT_AUDIO_PATH + attackType + "Attack");
            AudioManager.Instance.PlaySFX(clipHit);
            GameObject projectile = Instantiate(Prefabs[prefabIndex], attacker.position, Quaternion.identity);
            ProjectileManager mover = projectile.GetComponent<ProjectileManager>();

            mover.SetTargetPosition(target.position, onAnimationComplete);
            mover.hitClip = Resources.Load<AudioClip>(Constants.HIT_AUDIO_PATH + attackType + "Hit");
        }
        else DefaultAttack(attacker, target, false, onAnimationComplete);
    }
}
