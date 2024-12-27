using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    [Tooltip("子弹速度")] public float speed = 50.0f;
    [Tooltip("Boss子弹伤害")] public float damage = 10.0f;
    public float recycleTime = 3.0f;
    private Vector3 dir;
    private float curTime;

    /// <summary>
    /// 检测到碰撞到物体就回收自己
    /// </summary>
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

    private void Update()
    {
        curTime += Time.deltaTime;
        if (curTime > recycleTime) RecycleThis();
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
    }

    /// <summary>
    /// 设置子弹移动方向和buff等级
    /// </summary>
    /// <param name="_dir"> 射击方向 </param>
    public void Shot(Vector3 _dir, BossControl bossControl)
    {
        dir = _dir;
    }

    /// <summary>
    /// 回收自己
    /// </summary>
    public void RecycleThis()
    {
        curTime = 0;
        ObjectPool.Instance.RecycleObj("BossBullet", gameObject);
    }
}
