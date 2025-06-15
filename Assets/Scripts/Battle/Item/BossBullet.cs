using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ボスの弾丸
/// </summary>
public class BossBullet : MonoBehaviour
{
    [Tooltip("Bullet Speed")] public float speed = 50.0f;
    [Tooltip("Boss bullet damage")] public float damage = 10.0f;
    public float recycleTime = 3.0f;
    private Vector3 dir;
    private float curTime;

    ///<summary>


    ////// オブジェクトとの衝突を検出した場合、自動的に回収します。


    ///</summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.Instance.currentBoss.bossData.currentComboAttack = damage;
            GameManager.Instance.BossAttack();
            RecycleThis();
        }
    }

    ///<summary>


    ////// 弾丸の初期化


    ///</summary>
    private void Update()
    {
        curTime += Time.deltaTime;
        if (curTime > recycleTime) RecycleThis();
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
    }

    ///<summary>


    ////// 弾の移動方向とバフレベルを設定する


    ///</summary>
    /// <param name="_dir"> 射撃の方向 </param>
    public void Shot(Vector3 _dir, BossControl bossControl)
    {
        dir = _dir;
    }

    ///// <summary>
    ///// 自分自身をリサイクルする
    ///// </summary>
    //public void RecycleThis()
    //{
    //    curTime = 0;
    //    ObjectPool.Instance.RecycleObj("BossBullet", gameObject);
    //}

    ///<summary>


    ////// 弾丸の終了


    ///</summary>
    public void RecycleThis()
    {
        curTime = 0;
        ObjectPool.Instance.RecycleObj("BossBullet", gameObject);
    }
}
