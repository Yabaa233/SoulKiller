using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeControl : MonoBehaviour
{
    [Header("最大生命值")]
    public float maxHealth;
    [Header("当前生命值")]
    public float curHealth;
    [Header("打爆特效")]
    public GameObject effect;
    [Tooltip("受到近战攻击时伤害值")] public float getSwordDamage = 20.0f;
    [Tooltip("受到子弹攻击时伤害值")] public float getShotDamage = 1.0f;
    [Tooltip("受到魔法攻击时伤害值")] public float getMagicDamage = 15.0f;
    private StateBar stateBar;
    private void Start()
    {
        stateBar = PanelManager.Instance.GenerateCommonStatePanel(transform);
        stateBar.SetlocalScale(new Vector3(2f, 2f, 2f));
        stateBar.SetPositionBias(Vector3.up * 4);
    }
    private void OnDisable()
    {
        if (stateBar != null) stateBar.DestroyThis();
        stateBar = null;
    }
    private void OnDestroy()
    {
        if (stateBar != null) stateBar.DestroyThis();
        stateBar = null;
    }
    private void Update()
    {
        stateBar.UpdateState(curHealth, maxHealth);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon"))
        {
            if (other.tag == "PlayerWeapon")
            {
                curHealth -= getSwordDamage;
            }
            else if (other.tag == "PlayerBullet")
            {
                curHealth -= getShotDamage;
            }
            else if (other.tag == "PlayerMagic")
            {
                curHealth -= getMagicDamage;
            }
            else
            {
                return;
            }
            FMODUnity.RuntimeManager.PlayOneShot("event:/Level/BaoShi/pipeHit");
            if (curHealth <= 0)
            {
                effect.SetActive(true);
                gameObject.SetActive(false);
                FMODUnity.RuntimeManager.PlayOneShot("event:/Level/BaoShi/pipeBoom");
            }
        }
    }
}
