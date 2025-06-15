using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShieldBuff : I_BuffBase
{
    [Header("Obtaining data part through external parameter passing")]
    //現在のキャラクターの参照を保存する
    [Tooltip("Current role reference")]GameObject buffKeeper;
    //現在のキャラクターのBuffManagerを保存します
    [Tooltip("Current Role Buff Manager")]CharacterBuffManager characterBuffManager;
    //現在のBUFFの種類
    [Tooltip("Current Buff Type")]E_BuffKind buffType;
    //現在のキャラクタータイプ
    [Tooltip("Current role type")]E_ChararcterType chararcterType;

    [Tooltip("Current Buff Level")]public int currentLevel;

    [Header("Buff data section")]
    [Tooltip("Store Skill Level Function")]public Action<CharacterData> realEffect;
    [Tooltip("Function Level List")] public List<Action<CharacterData>> levelEffect;

    [Tooltip("Shield Type")] public GameObject sheildPrefab;
    [Tooltip("Controller of the Cache Shield")] public ShieldRipples shieldRipples;
    [Tooltip("Cache the current data.")]public CharacterData characterData;

    public ShieldBuff(E_ChararcterType _chararcterType,int level = 1)
    {
        this.currentLevel = level;
        buffType = E_BuffKind.ShieldBuff;
        chararcterType = _chararcterType;

        //初期化リスト
        levelEffect = new List<Action<CharacterData>>();
    }

    public void OnAdd(GameObject _buffKeeper)
    {
        this.buffKeeper = _buffKeeper;
        Init();
        switch(chararcterType)
        {
            case E_ChararcterType.player:PlayerShieldUp();break;
            case E_ChararcterType.enemy:EnemyShieldUp();break;
            case E_ChararcterType.boss:bossShieldUp();break;
        }
        ClearDelegate();
    }

    public void OnUpdate(float deltaTime)
    {
        
    }

    public void OnRemove()
    {
        switch(chararcterType)
        {
            case E_ChararcterType.player:PlayerShieldRemove();break;
            case E_ChararcterType.enemy:EnemyShieldRemove();break;
            case E_ChararcterType.boss:bossShieldRemove();break;
        }
    }

    public E_BuffKind GetBuffType()
    {
        return buffType;
    }

    public GameObject GetBuffKeeper()
    {
        return buffKeeper;
    }

    public E_ChararcterType GetChararcterType()
    {
        return chararcterType;
    }

    public int GetLevel()
    {
        return currentLevel;
    }

/// <summary>
/// シールドバフ
/// </summary>
/// <summary>
/// バフの初期化
/// </summary>
    public void Init()
    {
        levelEffect.Add(level1ShieldUp);
        levelEffect.Add(level2ShieldUp);
        levelEffect.Add(level3ShieldUp);
        levelEffect.Add(level4ShieldUp);

        if(currentLevel > levelEffect.Count)
        {
            Debug.Log("The assigned level exceeds the current level limit of the Buff.");
            return;
        }

        for(int i=0;i<currentLevel;i++)//スキルレベルに応じて効果を追加する
        {
            realEffect += levelEffect[i];
        }
    }

    ///<summary>


    ////// 割り当てた後で、委託をクリアします。


    ///</summary>
    public void ClearDelegate()
    {
        for (int i = currentLevel - 1; i >= 0; i--)
        {
            realEffect -= levelEffect[i];
        }
    }


/// 異なるタイプを得た後、Buffを追加します。
    private void PlayerShieldUp()
    {
        PlayerControl playerControl = buffKeeper.GetComponent<PlayerControl>();
        characterData = playerControl.characterData;

        realEffect(null);
    }

    private void EnemyShieldUp()
    {
        BaseEnemyControl enemyControl = buffKeeper.GetComponent<BaseEnemyControl>();
        characterData = enemyControl.enemyData;

        realEffect(null);
    }

    private void bossShieldUp()
    {
        BossControl bossControl = buffKeeper.GetComponent<BossControl>();
        characterData = bossControl.bossData;
        realEffect(null);
    }
///////バフが削除されたときに状態を復元します
    public void PlayerShieldRemove()
    {
        GameObject.Destroy(sheildPrefab);
        shieldRipples = null;
        Physics.IgnoreLayerCollision(8,12,false);//無視された衝突を復元する
    }

    public void EnemyShieldRemove()
    {
        BaseEnemyControl enemyControl = buffKeeper.GetComponent<BaseEnemyControl>();
        enemyControl.SetShieldVisble(false);
        GameObject.Destroy(sheildPrefab);
        shieldRipples = null;
        Physics.IgnoreLayerCollision(6,12,false);//無視された衝突tを復元します
    }

    public void bossShieldRemove()
    {
        GameObject.Destroy(sheildPrefab);
        shieldRipples = null;
    }


////////Buffの具体的な効果の実装
    private void level1ShieldUp(CharacterData characterData)
    {
        if(chararcterType == E_ChararcterType.player)
        {
            sheildPrefab = GameObject.Instantiate(BuffDataManager.Instance.playerShield,buffKeeper.transform);
            shieldRipples = sheildPrefab.GetComponent<ShieldRipples>();
            shieldRipples.chararcterType = E_ChararcterType.player;
            buffKeeper.GetComponent<PlayerControl>().characterBuffManager.shieldRipples = shieldRipples;//エンティティをマウントする
            shieldRipples.maxHealth = BuffDataManager.Instance.playerShieldStartHp;
            shieldRipples.currentHealth = BuffDataManager.Instance.playerShieldStartHp;

            //プレイヤーの武器とシールドの衝突を無視する
            Physics.IgnoreLayerCollision(8,12,true);
        }
        else if(chararcterType == E_ChararcterType.enemy)
        {
            sheildPrefab = GameObject.Instantiate(BuffDataManager.Instance.enemyShield,buffKeeper.transform);
            shieldRipples = sheildPrefab.GetComponent<ShieldRipples>();
            shieldRipples.chararcterType = E_ChararcterType.enemy;
            buffKeeper.GetComponent<BaseEnemyControl>().characterBuffManager.shieldRipples = shieldRipples;
            shieldRipples.maxHealth = BuffDataManager.Instance.enemyShieldStartHp;
            shieldRipples.currentHealth = BuffDataManager.Instance.enemyShieldStartHp;

            BaseEnemyControl enemyControl = buffKeeper.GetComponent<BaseEnemyControl>();
            characterData = enemyControl.enemyData;
            enemyControl.SetShieldVisble(true);
            //敵の武器と盾の衝突を無視する
            Physics.IgnoreLayerCollision(6,12,true);
        }
        else if(chararcterType == E_ChararcterType.boss)
        {
            sheildPrefab = GameObject.Instantiate(BuffDataManager.Instance.enemyShield,buffKeeper.transform);
            shieldRipples = sheildPrefab.GetComponent<ShieldRipples>();
            shieldRipples.chararcterType = E_ChararcterType.boss;
            buffKeeper.GetComponent<BossControl>().characterBuffManager.shieldRipples = shieldRipples;
            shieldRipples.maxHealth = BuffDataManager.Instance.bossShieldStartHp;
            shieldRipples.currentHealth = BuffDataManager.Instance.bossShieldStartHp;
            sheildPrefab.transform.GetChild(0).localScale *= 3;
        }
        //シールドの位置を上げる
        if(sheildPrefab == null)
        {
            Debug.LogWarning("The shield has not been added.");
            return;
        }
        float radius = sheildPrefab.GetComponent<SphereCollider>().radius;
        sheildPrefab.transform.position += new Vector3(0f,radius,0f);
    }

    private void level2ShieldUp(CharacterData characterData)
    {
        if(chararcterType == E_ChararcterType.player)
        {
            shieldRipples.ableBoom = true;
        }
        else if(chararcterType == E_ChararcterType.enemy)
        {
            shieldRipples.maxHealth += BuffDataManager.Instance.enemyShieldlevel2Up;
            shieldRipples.currentHealth += BuffDataManager.Instance.enemyShieldlevel2Up;
        }
        else if(chararcterType == E_ChararcterType.boss)
        {
            shieldRipples.maxHealth += BuffDataManager.Instance.bossShieldlevel2Up;
            shieldRipples.currentHealth += BuffDataManager.Instance.bossShieldlevel2Up;
        }
    }

    private void level3ShieldUp(CharacterData characterData)
    {
        if(chararcterType == E_ChararcterType.player)
        {
            shieldRipples.ableAoe = true;
        }
        else if(chararcterType == E_ChararcterType.enemy)
        {
            shieldRipples.maxHealth += BuffDataManager.Instance.enemyShieldlevel3Up;
            shieldRipples.currentHealth += BuffDataManager.Instance.enemyShieldlevel3Up;
        }
        else if(chararcterType == E_ChararcterType.boss)
        {
            shieldRipples.maxHealth += BuffDataManager.Instance.bossShieldlevel3Up;
            shieldRipples.currentHealth += BuffDataManager.Instance.bossShieldlevel3Up;
        }
    }

    private void level4ShieldUp(CharacterData characterData)
    {
        if(chararcterType == E_ChararcterType.player)
        {

        }
        else if(chararcterType == E_ChararcterType.enemy)
        {
            shieldRipples.maxHealth += BuffDataManager.Instance.enemyShieldlevel4Up;
            shieldRipples.currentHealth += BuffDataManager.Instance.enemyShieldlevel4Up;
        }
        else if(chararcterType == E_ChararcterType.boss)
        {
            shieldRipples.maxHealth += BuffDataManager.Instance.bossShieldlevel4Up;
            shieldRipples.currentHealth += BuffDataManager.Instance.bossShieldlevel4Up;
        }
    }

//////いくつかの特別な実装
    public void DamageReflect(CharacterData attackData, float damage)
    {
        float reflectDamage = damage * BuffDataManager.Instance.damageReflectPercent;
        attackData.currentHealth -= reflectDamage;
    }

}



