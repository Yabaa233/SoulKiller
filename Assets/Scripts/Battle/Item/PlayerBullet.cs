using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [Tooltip("子弹速度")] public float speed = 2;
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

    /// <summary>
    /// 检测到碰撞到物体就回收自己
    /// </summary>
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
            GameManager.Instance.Player_StartShotEffect(this);   //开始打击感流程
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
            GameManager.Instance.Player_StartShotEffect(this);   //开始打击感流程
        }
    }
    private void Update()
    {
        curTime += Time.deltaTime;
        if (curTime > recycleTime) RecycleThis();
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
    }

    /// <summary>
    /// 设置子弹移动方向和buff等级，并根据buff等级设置子弹状态
    /// </summary>
    /// <param name="_dir"> 射击方向 </param>
    /// <param name="level"> buff等级 </param>
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

    /// <summary>
    /// 回收自己
    /// </summary>
    public void RecycleThis()
    {
        curTime = 0;
        ObjectPool.Instance.RecycleObj("PlayerBullet", gameObject);
    }

    /// <summary>
    /// 打击感时速度清零
    /// </summary>
    public void PlayerBulletStop()
    {
        speed = 0;
    }
    /// <summary>
    /// 打击感时速度恢复
    /// </summary>
    public void PlayerBulletReset()
    {
        speed = firstSpeed;
    }
}
