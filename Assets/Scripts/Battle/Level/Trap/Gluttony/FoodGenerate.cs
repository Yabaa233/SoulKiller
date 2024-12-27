using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class FoodGenerate : MonoBehaviour
{
    [Header("Ҫ���ɵ�ҽ�ư�")]
    public GameObject medicalBag;
    [Header("ʳ���ϰ��ﵱǰѪ��")]
    public float currentHealth;
    [Header("�����ʳ���ϰ�����˺�")]
    public float fireDamage;
    [Tooltip("�ܵ���ս����ʱ�˺�ֵ")] public float getSwordDamage = 20.0f;
    [Tooltip("�ܵ��ӵ�����ʱ�˺�ֵ")] public float getShotDamage = 1.0f;
    [Tooltip("�ܵ�ħ������ʱ�˺�ֵ")] public float getMagicDamage = 15.0f;
    //TODO:���Ʊ�ʳ�ϰ�������ҽ�ư����ƻ��߼�
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("TrickWeapon"))
        {
            if (other.name == "Collider")
            {
                currentHealth -= fireDamage;
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
            if (gameObject.name == "diezi")
            {
                DOTween.Play("1");
            }
            Instantiate(medicalBag, transform.position + new Vector3(0, 0.5f, 0), medicalBag.transform.rotation, transform.parent);
            Destroy(gameObject);
        }
    }
}
