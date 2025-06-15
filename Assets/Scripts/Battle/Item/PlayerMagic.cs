using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMagic : MonoBehaviour
{
    public float openTriggerTime;
    public ParticleSystem migicParticle;   //パーティクルシステム
    [Header("Explosion damage assessment time")] private Vector3 firstSize;
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
            GameManager.Instance.Player_StartStaffEffect(this);   //ヒット感の処理を開始します
        }
        if (other.tag == "BossBody")
        {
            GameManager.Instance.currentPlayer.SetSpecifyComboAttack(E_WeaponType.staff, 2f, 0.15f);
            GameManager.Instance.PlayerAttack(other.GetComponent<BossControl>());
            GameManager.Instance.Player_StartStaffEffect(this);   //ヒット感の処理を開始します
        }
    }
    private void OnDisable()
    {
        GetComponent<Collider>().enabled = false;
    }
    /// <summary>
    /// プレイヤーの魔法
    /// </summary>
    public void StartMagic()
    {
        Invoke("OpenTrigger", openTriggerTime);
        gameObject.SetActive(true);
    }
    ///<summary>

    ////// 魔法の初期化

    ///</summary>
    private void OpenTrigger()
    {
        GetComponent<Collider>().enabled = true;
        Invoke("CloseTrigger", 0.5f);
    }
    ///<summary>

    ////// 魔法の更新

    ///</summary>
    private void CloseTrigger()
    {
        GetComponent<Collider>().enabled = false;
    }
    ///<summary>

    ////// 魔法の終わり

    ///</summary>
    /// <param name="size"></param>
    public void SetScale(float size)
    {
        transform.localScale = firstSize * size;
    }
}
