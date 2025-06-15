using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : singleton<EffectManager>
{
    [Header("剣攻撃特殊効果のノードを一時保存する")]
    public Transform attackEffectParent;
    public ParticleSystem currentAttackEffect;
    public Transform playerMagicRange;
    [Header("復活特殊効果のプレハブ")]
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
    /// プレイヤーの復活エフェクトを設定する
    /// </summary>
    /// <param name="pos"> 誕生特殊効果の表示位置 </param>
    public void SetResurgenceEffect(Vector3 pos)
    {
        GameObject resurgenceEffect = Instantiate(ResurgenceEffect, pos, Quaternion.identity, transform);
        resurgenceEffect.SetActive(true);
        DestroyEffect(resurgenceEffect);
    }

    /// <summary>
    /// スプリントエフェクトを表示する
    /// </summary>
    /// <param name="pos"> 表示位置 </param>
    /// <param name="dir"> 表示方向 </param>
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
        StartCoroutine(RecycleEffect("Effect_PlayerDashBoom", effect_DashBoom));     //再生が終了したら自動的にオブジェクトプールに回収されます。
        StartCoroutine(RecycleEffect("Effect_PlayerDashTrail", 1.0f, effect_DashTrail));     //再生が終了したら自動的にオブジェクトプールに回収されます。
    }

    ///<summary>


    ////// 射撃時の関連設定


    ///</summary>
    /// <param name="pos"> 特殊効果と弾の生成位置 </param>
    /// <param name="target"> 射撃目標点 </param>
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
        StartCoroutine(RecycleEffect("Effect_PlayerShot", effect_Bullet));     //再生が終了したら自動的にオブジェクトプールに回収されます。
    }

    /// <summary>
    /// プレイヤーのショットヒット効果
    /// </summary>
    /// <param name="pos"> 特殊効果の生成位置 </param>
    /// <param name="effectRecycleTime"> エフェクトリサイクル時間 </param>
    public void SetBulletHit(Vector3 pos, float effectRecycleTime)
    {
        GameObject effect_BulletHit = ObjectPool.Instance.GetObject("PlayerBulletHit", transform, true);
        effect_BulletHit.transform.position = pos;
        effect_BulletHit.SetActive(true);
        StartCoroutine(RecycleEffect("PlayerBulletHit", effectRecycleTime, effect_BulletHit));
    }

    ///<summary>


    ////// 魔法の球を放つ


    ///</summary>
    /// <param name="pos"> 法球が出現する位置 </param>
    /// <param name="magicBallSize"> 魔法球の爆発範囲 </param>
    public void SetMagicBall(Vector3 pos, float magicBallSize)
    {
        GameObject effect_Magic = ObjectPool.Instance.GetObject("Effect_PlayerMagic", transform, true);
        effect_Magic.GetComponent<PlayerMagic>().SetScale(magicBallSize);
        effect_Magic.transform.position = pos;
        effect_Magic.GetComponent<PlayerMagic>().StartMagic();
        StartCoroutine(RecycleEffect("Effect_PlayerMagic", effect_Magic));     //再生が終了したら自動的にオブジェクトプールに回収されます。
    }

    ///<summary>


    ////// 攻撃特効の表示


    ///</summary>
    /// <param name="effect"> エフェクトのプレハブ </param>
    /// <param name="pos"> 生成位置 </param>
    /// <param name="dir"> 生成方向 </param>
    public void SetAttackEffect(GameObject effect, Vector3 pos, Quaternion dir)
    {
        GameObject Effect_Attack = Instantiate(effect, pos, dir, attackEffectParent);
        if (currentAttackEffect != null && currentAttackEffect.isPaused)
        {
            currentAttackEffect.Play();
        }
        currentAttackEffect = Effect_Attack.transform.Find("ParticleSystem").GetComponent<ParticleSystem>();
        StartCoroutine(DestroyEffect(Effect_Attack));   //再生後自動的に削除されます
    }

    ///<summary>


    ////// 攻撃エフェクトを一時停止します


    ///</summary>
    public void PauseAttackEffect()
    {
        if (currentAttackEffect != null) currentAttackEffect.Pause();
    }

    ///<summary>


    ////// 再起動攻撃特効


    ///</summary>
    public void PlayAttackEffect()
    {
        if (currentAttackEffect != null) currentAttackEffect.Play();
    }
    #endregion

    #region Boss相关特效

    ///<summary>


    ////// 攻撃特効の表示


    ///</summary>
    /// <param name="effect"> エフェクトのプレハブ </param>
    /// <param name="pos"> 生成位置 </param>
    /// <param name="dir"> 生成方向 </param>
    public void SetBossAttackEffect(GameObject effect, Vector3 pos, Quaternion dir)
    {
        GameObject Effect_Attack = Instantiate(effect, pos, dir, attackEffectParent);
        StartCoroutine(DestroyEffect(Effect_Attack));   //再生後自動的に削除されます
    }

    /// <summary>
    /// ボスが弾を撃つ
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
        StartCoroutine(RecycleEffect("Effect_BossShot", effect_Bullet));     //再生が終了したら自動的にオブジェクトプールに回収されます。
    }

    /// <summary>
    /// ボスの魔法攻撃蓄積特殊効果
    /// </summary>
    /// <param name="staff"> 特効表示位置 </param>
    public void Boss_SetMagic_Start(Transform staff)
    {
        // Debug.Log("ボスの魔法チャージ");
    }

    /// <summary>
    /// ボスの魔法攻撃
    /// </summary>
    /// <param name="position"> 攻撃位置 </param>
    /// <param name="bossControl"> ボススクリプト、Buffレベルの取得に使用 </param>
    public void Boss_SetMagic_Shot(Vector3 position, BossControl bossControl)
    {
        // Debug.Log("ボスが魔法を放つ");
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

    ///<summary>


    ////// 外部補助破壊特殊効果


    ///</summary>
    /// <param name="effect">削除する特殊効果</param>
    public void LetDestroyEffect(GameObject effect)
    {
        StartCoroutine(DestroyEffect(effect));
    }

    ///<summary>


    ////// 外部助成によるリサイクル遅延効果


    ///</summary>
    /// <param name="poolKey">エフェクトが回収されるプール</param>
    /// <param name="effect">回収待ちの特殊効果</param>
    /// <param name="time">遅延時間</param>
    public void LetRecycleEffect(string poolKey, GameObject effect, float time)
    {
        StartCoroutine(RecycleEffect(poolKey, time, effect));
    }

    ///<summary>


    ////// 再生後に自動的に削除されます


    ///</summary>
    /// <param name="effect"> 削除待ちの特殊効果オブジェクト </param>
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

    ///<summary>


    ////// 再生が完了したら、自動的にオブジェクトプールを回収します。


    ///</summary>
    /// <param name="poolKey"> オブジェクトプールのインデックス </param>
    /// <param name="effect"> 回収待ちの特殊効果オブジェクト </param>
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


    ///<summary>



    ////// 特定の時間後に特殊効果を回収します
    /// 終了時間が不確定な特殊効果の再生に使用されます



    ///</summary>
    /// <param name="poolKey"> オブジェクトプールのインデックス </param>
    /// <param name="time"> 延期回収時間 </param>
    /// <param name="effect"> 回収待ちの特殊効果オブジェクト </param>
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
