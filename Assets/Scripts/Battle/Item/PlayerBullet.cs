using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの弾丸
/// </summary>
public class PlayerBullet : MonoBehaviour
{
    [Tooltip("Bullet speed")] public float speed = 2;
    public float recycleTime = 3.0f;
    public float hitEffectRecycleTime = 1.5f;
    private Vector3 dir;
    private float curTime;
    private int canAttackCount = 1;
    private float firstSpeed;

    private void Start()
    {
        firstSpeed = speed;
    }

    ///<summary>


    ////// オブジェクトと衝突を検出した場合、自動的に回収します。


    ///</summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.currentPlayer.IsDead)
        {
            RecycleThis();
            return;
        }
        if (other.tag == "EmyBody")
        {
            if (--canAttackCount == 0)
            {
                RecycleThis();
            }
            EffectManager.Instance.SetBulletHit(transform.position, hitEffectRecycleTime);
            GameManager.Instance.currentPlayer.SetSpecifyComboAttack(E_WeaponType.gun, 2f, 0.25f);
            GameManager.Instance.PlayerAttack(other.transform.parent.GetComponent<BaseEnemyControl>(), transform.position);
            GameManager.Instance.Player_StartShotEffect(this);   //ヒット感の処理を開始します
        }
        if (other.tag == "BossBody")
        {
            if (--canAttackCount == 0)
            {
                RecycleThis();
            }
            EffectManager.Instance.SetBulletHit(transform.position, hitEffectRecycleTime);
            GameManager.Instance.currentPlayer.SetSpecifyComboAttack(E_WeaponType.gun, 2.0f, 0.25f);
            GameManager.Instance.PlayerAttack(other.GetComponent<BossControl>());
            GameManager.Instance.Player_StartShotEffect(this);   //ヒット感の処理を開始します
        }
    }

    ///<summary>


    ////// 弾丸の更新


    ///</summary>
    private void Update()
    {
        curTime += Time.deltaTime;
        if (curTime > recycleTime) RecycleThis();
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
    }

    ///<summary>


    ////// 弾丸の初期化


    ///</summary>
    /// <param name="_dir"> 発射方向 </param>
    /// <param name="level"> バフレベル </param>
    public void Shot(Vector3 _dir, int level)
    {
        dir = _dir;
        if (level == 4)
        {
            canAttackCount = 100;
        }
        else if (level >= 2)
        {
            canAttackCount = 2;
        }
        else
        {
            canAttackCount = 1;
        }
    }

    ///<summary>


    ////// 弾丸の終了


    ///</summary>
    public void RecycleThis()
    {
        curTime = 0;
        ObjectPool.Instance.RecycleObj("PlayerBullet", gameObject);
    }

    /// <summary>
    /// ヒット感時に速度をゼロに
    /// </summary>
    public void PlayerBulletStop()
    {
        speed = 0;
    }
    /// <summary>
    /// ヒット感時に速度を回復
    /// </summary>
    public void PlayerBulletReset()
    {
        speed = firstSpeed;
    }
}
