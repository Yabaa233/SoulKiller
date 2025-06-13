using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 花の弾
/// </summary>
public class FlowerBullet : MonoBehaviour
{
    public float speed;
    public float damage = 3f;
    public float recycleTime=3f;
    public GameObject effect;
    private float startTime;
    private Vector3 target;
    private GameObject bulletBoom;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player"||other.gameObject.layer==LayerMask.NameToLayer("Obstacle"))
        {
            if(other.tag == "Player")
            {
                GameManager.Instance.TrickAttackPlayer(AttackPlayer);
            }
            bulletBoom = ObjectPool.Instance.GetObject("FlowerBulletBoom", gameObject.transform.parent.transform, false, true);
            bulletBoom.transform.position = transform.position;
            bulletBoom.SetActive(true);
            EffectManager.Instance.LetRecycleEffect("FlowerBulletBoom", bulletBoom, 0.5f);
            RecycleThis();
        }
    }
    /// <summary>
    /// 弾の更新
    /// </summary>
    private void Update()
    {
        startTime += Time.deltaTime;
        if (startTime >= recycleTime) RecycleThis();
        transform.Translate(target * speed * Time.deltaTime);
    }
    /// <summary>
    /// 弾の初期化
    /// </summary>
    public void Shot(Vector3 position)
    {
        target = (position + Vector3.up * 2 - transform.position).normalized;
        Debug.Log("初期方向"+position);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Level/JiDu/little");
    }
    /// <summary>
    /// 弾の終了
    /// </summary>
    public void RecycleThis()
    {
        startTime = 0;
        ObjectPool.Instance.RecycleObj("FlowerBullet", gameObject);
    }
    private void AttackPlayer(PlayerControl curPlayer)
    {
        if (!curPlayer.characterBuffManager.CalcuSheild(null, damage))
        {
            curPlayer.characterData.currentHealth -= damage;
        }
    }
}
