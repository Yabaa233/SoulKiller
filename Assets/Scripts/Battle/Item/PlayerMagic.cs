using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMagic : MonoBehaviour
{
    public float openTriggerTime;
    public ParticleSystem migicParticle;   //粒子系统
    [Header("爆炸伤害判定时间")] private Vector3 firstSize;
    private void Awake()
    {
        migicParticle = GetComponent<ParticleSystem>();
        firstSize = transform.localScale;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.currentPlayer.IsDead)
        {
            return;
        }
        if (other.tag == "EmyBody")
        {
            GameManager.Instance.currentPlayer.SetSpecifyComboAttack(E_WeaponType.staff, 2f, 0.15f);
            GameManager.Instance.PlayerAttack(other.transform.parent.GetComponent<BaseEnemyControl>(), transform.position);
            GameManager.Instance.Player_StartStaffEffect(this);   //开始打击感流程
        }
        if (other.tag == "BossBody")
        {
            GameManager.Instance.currentPlayer.SetSpecifyComboAttack(E_WeaponType.staff, 2f, 0.15f);
            GameManager.Instance.PlayerAttack(other.GetComponent<BossControl>());
            GameManager.Instance.Player_StartStaffEffect(this);   //开始打击感流程
        }
    }
    private void OnDisable()
    {
        GetComponent<Collider>().enabled = false;
    }
    /// <summary>
    /// 开始显示并准备显示Trigger
    /// </summary>
    public void StartMagic()
    {
        Invoke("OpenTrigger", openTriggerTime);
        gameObject.SetActive(true);
    }
    /// <summary>
    /// 延迟调用的显示触发器
    /// </summary>
    private void OpenTrigger()
    {
        GetComponent<Collider>().enabled = true;
        Invoke("CloseTrigger", 0.5f);
    }
    /// <summary>
    /// 延迟调用的关闭触发器
    /// </summary>
    private void CloseTrigger()
    {
        GetComponent<Collider>().enabled = false;
    }
    /// <summary>
    /// 设置法球AOE尺寸
    /// </summary>
    /// <param name="size"></param>
    public void SetScale(float size)
    {
        transform.localScale = firstSize * size;
    }
}
