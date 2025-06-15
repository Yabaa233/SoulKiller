using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeControl : MonoBehaviour
{
    [Header("Maximum Health Points")]
    public float maxHealth;
    [Header("Current Health Points")]
    public float curHealth;
    [Header("Explosive special effects")]
    public GameObject effect;
    [Tooltip("Damage value when attacked in close combat")] public float getSwordDamage = 20.0f;
    [Tooltip("Damage value when hit by a bullet")] public float getShotDamage = 1.0f;
    [Tooltip("Damage value when attacked by magic")] public float getMagicDamage = 15.0f;
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
