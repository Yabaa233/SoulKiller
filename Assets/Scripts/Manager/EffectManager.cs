using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : singleton<EffectManager>
{
    [Header("暂存剑攻击特效的节点")]
    public Transform attackEffectParent;
    public ParticleSystem currentAttackEffect;
    public Transform playerMagicRange;
    [Header("复活特效预制体")]
    public GameObject ResurgenceEffect;

    protected override void Awake()
    {
        base.Awake();
        attackEffectParent = transform.Find("AttackEffect");
        playerMagicRange = transform.Find("PlayerMagicRange");
        DontDestroyOnLoad(this.gameObject);
    }

    #region 玩家相关特效
    /// <summary>
    /// 设置玩家复活特效
    /// </summary>
    /// <param name="pos"> 出生特效显示位置 </param>
    public void SetResurgenceEffect(Vector3 pos)
    {
        GameObject resurgenceEffect = Instantiate(ResurgenceEffect, pos, Quaternion.identity, transform);
        resurgenceEffect.SetActive(true);
        DestroyEffect(resurgenceEffect);
    }

    /// <summary>
    /// 显示冲刺特效
    /// </summary>
    /// <param name="pos"> 显示位置 </param>
    /// <param name="dir"> 显示方向 </param>
    public void SetDashEffect(Vector3 pos, Vector3 dir)
    {
        // GameObject effect_Dash = ObjectPool.Instance.GetObject("Effect_PlayerDash", transform, true);
        GameObject effect_DashBoom = ObjectPool.Instance.GetObject("Effect_PlayerDashBoom", transform, true);
        GameObject effect_DashTrail = ObjectPool.Instance.GetObject("Effect_PlayerDashTrail", GameManager.Instance.currentPlayer.transform, true);
        effect_DashBoom.transform.position = pos;
        effect_DashTrail.transform.position = pos;
        // effect_DashTrail.transform.position = GameManager.Instance.currentPlayer.transform.position;
        // effect_DashBoom.transform.LookAt(dir);
        effect_DashBoom.SetActive(true);
        effect_DashTrail.SetActive(true);
        StartCoroutine(RecycleEffect("Effect_PlayerDashBoom", effect_DashBoom));     //播完自动回收到对象池
        StartCoroutine(RecycleEffect("Effect_PlayerDashTrail", 1.0f, effect_DashTrail));     //播完自动回收到对象池
    }

    /// <summary>
    /// 射击时相关配置
    /// </summary>
    /// <param name="pos"> 特效和子弹生成位置 </param>
    /// <param name="target"> 射击目标点 </param>
    public void SetBullet(Vector3 pos, Vector3 target, int buffLevel)
    {
        GameObject effect_Bullet = ObjectPool.Instance.GetObject("Effect_PlayerShot", transform, true);
        GameObject bullet = ObjectPool.Instance.GetObject("PlayerBullet", transform, true);
        bullet.transform.position = pos;
        effect_Bullet.transform.position = pos;
        bullet.transform.LookAt(target);
        effect_Bullet.transform.LookAt(target);
        bullet.SetActive(true);
        effect_Bullet.SetActive(true);
        bullet.GetComponent<PlayerBullet>().Shot((target - pos).normalized, buffLevel);
        StartCoroutine(RecycleEffect("Effect_PlayerShot", effect_Bullet));     //播完自动回收到对象池
    }

    /// <summary>
    /// 玩家射击命中效果
    /// </summary>
    /// <param name="pos"> 特效生成位置 </param>
    /// <param name="effectRecycleTime"> 特效回收时间 </param>
    public void SetBulletHit(Vector3 pos, float effectRecycleTime)
    {
        GameObject effect_BulletHit = ObjectPool.Instance.GetObject("PlayerBulletHit", transform, true);
        effect_BulletHit.transform.position = pos;
        effect_BulletHit.SetActive(true);
        StartCoroutine(RecycleEffect("PlayerBulletHit", effectRecycleTime, effect_BulletHit));
    }

    /// <summary>
    /// 发射魔法球
    /// </summary>
    /// <param name="pos"> 法球出现的位置 </param>
    /// <param name="magicBallSize"> 法球爆炸范围 </param>
    public void SetMagicBall(Vector3 pos, float magicBallSize)
    {
        GameObject effect_Magic = ObjectPool.Instance.GetObject("Effect_PlayerMagic", transform, true);
        effect_Magic.GetComponent<PlayerMagic>().SetScale(magicBallSize);
        effect_Magic.transform.position = pos;
        effect_Magic.GetComponent<PlayerMagic>().StartMagic();
        StartCoroutine(RecycleEffect("Effect_PlayerMagic", effect_Magic));     //播完自动回收到对象池
    }

    /// <summary>
    /// 攻击特效显示
    /// </summary>
    /// <param name="effect"> 特效预制体 </param>
    /// <param name="pos"> 生成位置 </param>
    /// <param name="dir"> 生成朝向 </param>
    public void SetAttackEffect(GameObject effect, Vector3 pos, Quaternion dir)
    {
        GameObject Effect_Attack = Instantiate(effect, pos, dir, attackEffectParent);
        if (currentAttackEffect != null && currentAttackEffect.isPaused)
        {
            currentAttackEffect.Play();
        }
        currentAttackEffect = Effect_Attack.transform.Find("ParticleSystem").GetComponent<ParticleSystem>();
        StartCoroutine(DestroyEffect(Effect_Attack));   //播完自动删除
    }

    /// <summary>
    /// 暂停攻击特效
    /// </summary>
    public void PauseAttackEffect()
    {
        if (currentAttackEffect != null) currentAttackEffect.Pause();
    }

    /// <summary>
    /// 重启攻击特效
    /// </summary>
    public void PlayAttackEffect()
    {
        if (currentAttackEffect != null) currentAttackEffect.Play();
    }
    #endregion

    #region Boss相关特效

    /// <summary>
    /// 攻击特效显示
    /// </summary>
    /// <param name="effect"> 特效预制体 </param>
    /// <param name="pos"> 生成位置 </param>
    /// <param name="dir"> 生成朝向 </param>
    public void SetBossAttackEffect(GameObject effect, Vector3 pos, Quaternion dir)
    {
        GameObject Effect_Attack = Instantiate(effect, pos, dir, attackEffectParent);
        StartCoroutine(DestroyEffect(Effect_Attack));   //播完自动删除
    }

    /// <summary>
    /// Boss射击子弹
    /// </summary>
    /// <param name="gun"></param>
    public void Boss_SetBullet(Transform gun, BossControl bossControl)
    {
        GameObject effect_Bullet = ObjectPool.Instance.GetObject("Effect_BossShot", transform, true);
        GameObject bullet1 = ObjectPool.Instance.GetObject("BossBullet", transform, true);
        GameObject bullet2 = ObjectPool.Instance.GetObject("BossBullet", transform, true);

        bullet1.transform.position = gun.position;
        bullet2.transform.position = gun.position;
        effect_Bullet.transform.position = gun.position;

        bullet1.transform.rotation = gun.rotation;
        bullet1.transform.Rotate(new Vector3(0, 30, 0), Space.World);
        bullet2.transform.rotation = gun.rotation;
        bullet2.transform.Rotate(new Vector3(0, -30, 0), Space.World);
        effect_Bullet.transform.rotation = gun.rotation;

        bullet1.SetActive(true);
        bullet2.SetActive(true);
        effect_Bullet.SetActive(true);

        bullet1.GetComponent<BossBullet>().Shot(bullet1.transform.forward, bossControl);
        bullet2.GetComponent<BossBullet>().Shot(bullet2.transform.forward, bossControl);
        StartCoroutine(RecycleEffect("Effect_BossShot", effect_Bullet));     //播完自动回收到对象池
    }

    /// <summary>
    /// Boss法术攻击蓄力特效
    /// </summary>
    /// <param name="staff"> 特效显示位置 </param>
    public void Boss_SetMagic_Start(Transform staff)
    {
        // Debug.Log("Boss法术蓄力");
    }

    /// <summary>
    /// Boss法术攻击
    /// </summary>
    /// <param name="position"> 攻击位置 </param>
    /// <param name="bossControl"> Boss脚本，用于获取Buff等级 </param>
    public void Boss_SetMagic_Shot(Vector3 position, BossControl bossControl)
    {
        // Debug.Log("Boss发射一个法术");
        int type = UnityEngine.Random.Range(1, 3);
        // Debug.Log(type);
        if (type == 1)
        {
            GameObject effect_Magic = ObjectPool.Instance.GetObject("Effect_BossSpatula", transform, true);
            effect_Magic.SetActive(true);
            effect_Magic.GetComponent<BossShadowGenerate>().ShadowGenerate(position, bossControl);
        }
        else
        {
            GameObject effect_Magic = ObjectPool.Instance.GetObject("Effect_BossPiece", transform, true);
            effect_Magic.SetActive(true);
            effect_Magic.GetComponent<BossShadowGenerate>().ShadowGenerate(position, bossControl);
        }
    }
    #endregion

    /// <summary>
    /// 对外辅助销毁特效
    /// </summary>
    /// <param name="effect">待删除的特效</param>
    public void LetDestroyEffect(GameObject effect)
    {
        StartCoroutine(DestroyEffect(effect));
    }

    /// <summary>
    /// 对外辅助回收延迟特效
    /// </summary>
    /// <param name="poolKey">特效待回收到的池子</param>
    /// <param name="effect">待回收特效</param>
    /// <param name="time">延迟时间</param>
    public void LetRecycleEffect(string poolKey, GameObject effect, float time)
    {
        StartCoroutine(RecycleEffect(poolKey, time, effect));
    }

    /// <summary>
    /// 播放完自动删除
    /// </summary>
    /// <param name="effect"> 待删除特效物体 </param>
    /// <returns></returns>
    IEnumerator DestroyEffect(GameObject effect)
    {
        ParticleSystem state = effect.transform.Find("ParticleSystem").GetComponent<ParticleSystem>();
        while (state.isPlaying || state.isPaused)
        {
            yield return null;
        }
        Destroy(effect);
        yield break;
    }

    /// <summary>
    /// 播放完自动回收对象池
    /// </summary>
    /// <param name="poolKey"> 对象池索引 </param>
    /// <param name="effect"> 待回收特效物体 </param>
    /// <returns></returns>
    IEnumerator RecycleEffect(string poolKey, GameObject effect)
    {
        ParticleSystem state = effect.GetComponent<ParticleSystem>();
        while (state.isPlaying || state.isPaused)
        {
            yield return null;
        }
        ObjectPool.Instance.RecycleObj(poolKey, effect);
        yield break;
    }


    /// <summary>
    /// 经过固定时间后回收特效
    /// 用于播放结束时间不固定的特效回收
    /// </summary>
    /// <param name="poolKey"> 对象池索引 </param>
    /// <param name="time"> 延迟回收时间 </param>
    /// <param name="effect"> 待回收特效物体 </param>
    /// <returns></returns>
    IEnumerator RecycleEffect(string poolKey, float time, GameObject effect)
    {
        float curTime = 0;
        while (curTime < time)
        {
            curTime += Time.deltaTime;
            yield return null;
        }
        ObjectPool.Instance.RecycleObj(poolKey, effect);
        yield break;
    }
}
