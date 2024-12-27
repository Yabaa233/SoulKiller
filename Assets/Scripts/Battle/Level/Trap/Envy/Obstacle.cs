using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("�ϰ���Ѫ��")]
    public float currentHealth;
    [Header("С���ӵ��˺�")]
    public float flowerBullet;
    [Tooltip("�ܵ���ս����ʱ�˺�ֵ")] public float getSwordDamage = 20.0f;
    [Tooltip("�ܵ��ӵ�����ʱ�˺�ֵ")] public float getShotDamage = 1.0f;
    [Tooltip("�ܵ�ħ������ʱ�˺�ֵ")] public float getMagicDamage = 15.0f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("TrickWeapon"))
        {
            if (other.tag == "Bullet")
            {
                currentHealth -= flowerBullet;
                //TODO:flowerBullet�Ӷ����
                Destroy(other.gameObject);
            }
            else
            {
                currentHealth = 0;
            }
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon"))
        {
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
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
