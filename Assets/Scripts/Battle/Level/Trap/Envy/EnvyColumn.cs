using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnvyColumn : MonoBehaviour
{
    [Tooltip("Death event")] public UnityAction Rely;
    [Tooltip("Large Flower Death")] public GameObject bossFlower;
    [Tooltip("The health level of the pillar where Xiao Hua is located.")] public float currentHealth;
    [Tooltip("Little Flower's Maximum Health")] public float maxHealth;
    [Tooltip("Replace the pillar")] public GameObject column;
    [Tooltip("Damage value when attacked in close combat")] public float getSwordDamage = 20.0f;
    [Tooltip("Damage value when hit by a bullet")] public float getShotDamage = 1.0f;
    [Tooltip("Damage value when attacked by magic")] public float getMagicDamage = 15.0f;
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
