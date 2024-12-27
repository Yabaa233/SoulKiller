using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FountainDestory : MonoBehaviour
{
    public float currentHealth;
    [Header("?????งน")]
    public GameObject Fracture;
    [Header("????")]
    public GameObject destruction;
    [Tooltip("???????????????")] public float flowerShotDamage = 1.0f;
    [Tooltip("???????????????")] public float getSwordDamage = 20.0f;
    [Tooltip("???????????????")] public float getShotDamage = 1.0f;
    [Tooltip("???????????????")] public float getMagicDamage = 15.0f;
    private void OnEnable()
    {
        destruction.GetComponent<FoodDestroy>().eliminate += Destruction;
    }
    private void OnDisable()
    {
        destruction.GetComponent<FoodDestroy>().eliminate -= Destruction;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("TrickWeapon"))
        {
            if (other.name == "Laser")
            {
                currentHealth = 0;
            }
            if (other.tag == "Bullet")
            {
                currentHealth -= flowerShotDamage;
                Destroy(other.gameObject);
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
            Close();
        }
    }
    public void Destruction()
    {
        Close();
    }
    public void Close()
    {
        GameObject beDamage = Instantiate(Fracture, gameObject.transform.position, Quaternion.identity);
        Destroy(beDamage, 2f);
        Destroy(gameObject);
    }
}
