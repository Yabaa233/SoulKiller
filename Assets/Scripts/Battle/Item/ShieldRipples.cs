using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldRipples : MonoBehaviour
{
    public GameObject ShieldVFX;
    public GameObject RipplesVFX;
    public GameObject HitVFX;
    public GameObject BreakVFX;
    public GameObject ShockWave;
    private Material mat;
    
    public float maxHealth = 100;
    public float currentHealth;

    public SphereCollider sphereCollider;
    public BoxCollider boxCollider;
    public BoxCollider AoeCollider;
    public bool isBoom = false;
    public bool isAoe = false;
    public bool ableAoe = false;
    public bool ableBoom = false;

    public bool isTrue = true;//有效
    public E_ChararcterType chararcterType;

    private float timer;
    private float nextAoeTime;

    private void Awake() {
        boxCollider = transform.GetChild(1).gameObject.GetComponent<BoxCollider>();

        AoeCollider = transform.GetChild(2).gameObject.GetComponent<BoxCollider>();

    }

    private void Start() {
        nextAoeTime = BuffDataManager.Instance.playerAoeTimeBtw;
    }

    private void Update() {

        if(Time.time - timer > nextAoeTime && chararcterType == E_ChararcterType.player && ableAoe)
        {
            AoeCollider.enabled = true;
            boxCollider.enabled = false;
            isAoe = true;
            timer = Time.time;
        }

    }

    private void OnTriggerEnter(Collider collider)
    {
        // Debug.Log(collider.tag);
        string disableTag = "";
        if(chararcterType == E_ChararcterType.player)
        {
            disableTag = "PlayerWeapon";
            //炸盾伤害
            if(isBoom && collider.tag == "EmyBody" && ableBoom)
            {
                calcuDamagewithType(collider);
            }

            //周期性伤害
            if(chararcterType == E_ChararcterType.player && isAoe)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Player/shierdBO");
                calcuDamagewithType(collider);
                var shockEffect = Instantiate(ShockWave,transform.position,Quaternion.identity);
                EffectManager.Instance.LetDestroyEffect(shockEffect);
                isAoe = false;
                AoeCollider.enabled = false;
                boxCollider.enabled = true;
            }
        }
        else if(chararcterType == E_ChararcterType.enemy)
        {
            disableTag = "EmyWeapon";
        }

        //屏蔽攻击的tag
        if(collider.tag == disableTag || collider.tag == "Ground")
        {
            return;
        }
        sphereCollider = gameObject.GetComponent<SphereCollider>();
        Vector3 vector = collider.transform.position - transform.position + new Vector3(0f,sphereCollider.radius,0f);
        Vector3 hitpoint = transform.position + vector.normalized * sphereCollider.radius;
        var ripples = Instantiate(RipplesVFX,transform) as GameObject;
        var psr = ripples.transform.GetChild(0).GetComponent<ParticleSystemRenderer>();
        mat = psr.material;
        mat.SetVector("_SphereCenter", hitpoint);
        Destroy(ripples, 1);
        var hit = Instantiate (HitVFX, hitpoint, Quaternion.identity) as GameObject;
        Destroy(hit, 1);

    }

    public void calcuDamagewithType(Collider collider)
    {
        if(collider.tag == "EmyBody")
        {
            BaseEnemyControl baseEnemyControl = collider.transform.parent.GetComponent<BaseEnemyControl>();

            GameManager.Instance.currentPlayer.GetComponent<PlayerControl>().characterData.currentComboAttack =
            GameManager.Instance.currentPlayer.GetComponent<PlayerControl>().currentComboNode.baseDamage;
            
            GameManager.Instance.PlayerAttack(baseEnemyControl, Vector3.zero);
        }
        else if(collider.tag == "BossBody")
        {
            BossControl bossControl = collider.transform.GetComponent<BossControl>();
            GameManager.Instance.currentPlayer.GetComponent<PlayerControl>().characterData.currentComboAttack =
            GameManager.Instance.currentPlayer.GetComponent<PlayerControl>().currentComboNode.baseDamage;
            
            GameManager.Instance.PlayerAttack(bossControl);
        }
    }

/// <summary>
/// 盾破碎造成伤害
/// </summary>
    public void ShieldDamage()
    {
        //切换判定区域
        sphereCollider.enabled = false;
        AoeCollider.enabled = false;
        boxCollider.enabled = true;
        isBoom = true;
    }

/// <summary>
/// 销毁当前盾牌
/// </summary>
    public void DestroyShield()
    {
        StartCoroutine(EffectPlay());
    }

/// <summary>
/// 设置激活
/// </summary>
    public void SetShieldVisble(bool state)
    {
        this.gameObject.SetActive(state); 
    }

    IEnumerator EffectPlay()
    {
        var breaks = Instantiate(BreakVFX, transform) as GameObject;
        while(breaks.transform.GetChild(0).GetComponent<ParticleSystem>().isPlaying)
        {
            yield return null;
        }
        SetShieldVisble(false);
        Destroy(breaks);
        yield break;
    }   
}
