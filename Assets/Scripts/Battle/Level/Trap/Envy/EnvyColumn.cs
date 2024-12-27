using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnvyColumn : MonoBehaviour
{
    [Tooltip("死亡事件")] public UnityAction Rely;
    [Tooltip("大花死亡")] public GameObject bossFlower;
    [Tooltip("小花所在柱子的血量")] public float currentHealth;
    [Tooltip("小花最大血量")] public float maxHealth;
    [Tooltip("替换柱子")] public GameObject column;
    [Tooltip("受到近战攻击时伤害值")] public float getSwordDamage = 20.0f;
    [Tooltip("受到子弹攻击时伤害值")] public float getShotDamage = 1.0f;
    [Tooltip("受到魔法攻击时伤害值")] public float getMagicDamage = 15.0f;
    private StateBar stateBar;
    private bool isDead;
    private void OnEnable()
    {
        bossFlower.GetComponent<FlowerRotate>().broadcastDead += Dead;
        stateBar = PanelManager.Instance.GenerateCommonStatePanel(this.transform);
        stateBar.SetPositionBias(new Vector3(0, 3, 0));
        stateBar.SetlocalScale(Vector3.one * 2);
    }
    private void OnDisable()
    {
        if (stateBar != null)
        {
            stateBar.DestroyThis();
            stateBar = null;
        }
        bossFlower.GetComponent<FlowerRotate>().broadcastDead -= Dead;
    }
    private void OnDestroy()
    {
        if (stateBar != null)
        {
            stateBar.DestroyThis();
            stateBar = null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isDead)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon"))
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Level/AoMan/qiziHit");
                if (other.tag == "PlayerWeapon")
                {
                    currentHealth -= getSwordDamage;
                }
                else if (other.tag == "PlayerBullet")
                {
                    currentHealth -= getShotDamage;
                }
                else if (other.tag == "PlayerMagic")
                {
                    currentHealth -= getMagicDamage;
                }
                else
                {
                    return;
                }
            }
            if (other.gameObject.layer == LayerMask.NameToLayer("TrickWeapon"))
            {
                if (other.name == "Laser")
                {
                    currentHealth = 0;
                }
            }
            if (currentHealth <= 0)
            {
                Dead();
            }
        }
    }
    private void Update()
    {
        stateBar.UpdateState(currentHealth, maxHealth);
    }
    void Dead()
    {
        isDead = true;
        if (Rely != null)
        {
            Rely();
        }
        stateBar.DestroyThis();
        stateBar = null;
        column.SetActive(true);
        Destroy(gameObject);
    }
}
