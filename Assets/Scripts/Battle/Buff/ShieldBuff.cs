using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShieldBuff : I_BuffBase
{
    [Header("外部传参获得数据部分")]
    //存当前角色的引用
    [Tooltip("当前角色引用")]GameObject buffKeeper;
    //存当前角色的BuffManager
    [Tooltip("当前角色Buff管理器")]CharacterBuffManager characterBuffManager;
    //当前BUFF种类
    [Tooltip("当前Buff类型")]E_BuffKind buffType;
    //当前角色的类型
    [Tooltip("当前角色类型")]E_ChararcterType chararcterType;

    [Tooltip("当前Buff等级")]public int currentLevel;

    [Header("Buff数据部分")]
    [Tooltip("存放技能等级函数")]public Action<CharacterData> realEffect;
    [Tooltip("函数等级列表")] public List<Action<CharacterData>> levelEffect;

    [Tooltip("护盾类型")] public GameObject sheildPrefab;
    [Tooltip("缓存护盾的控制器")] public ShieldRipples shieldRipples;
    [Tooltip("缓存当前数据")]public CharacterData characterData;

    public ShieldBuff(E_ChararcterType _chararcterType,int level = 1)
    {
        this.currentLevel = level;
        buffType = E_BuffKind.ShieldBuff;
        chararcterType = _chararcterType;

        //初始化列表
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
/// 初始化应该调用的函数
/// </summary>
    public void Init()
    {
        levelEffect.Add(level1ShieldUp);
        levelEffect.Add(level2ShieldUp);
        levelEffect.Add(level3ShieldUp);
        levelEffect.Add(level4ShieldUp);

        if(currentLevel > levelEffect.Count)
        {
            Debug.Log("赋予的等级超过Buff当前等级限制");
            return;
        }

        for(int i=0;i<currentLevel;i++)//根据技能等级添加效果
        {
            realEffect += levelEffect[i];
        }
    }

    /// <summary>
    /// 赋值完之后清空委托
    /// </summary>
    public void ClearDelegate()
    {
        for (int i = currentLevel - 1; i >= 0; i--)
        {
            realEffect -= levelEffect[i];
        }
    }


/// 得到不同的类型然后添加Buff
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
///////Buff移除的时候还原状态
    public void PlayerShieldRemove()
    {
        GameObject.Destroy(sheildPrefab);
        shieldRipples = null;
        Physics.IgnoreLayerCollision(8,12,false);//还原忽略的碰撞
    }

    public void EnemyShieldRemove()
    {
        BaseEnemyControl enemyControl = buffKeeper.GetComponent<BaseEnemyControl>();
        enemyControl.SetShieldVisble(false);
        GameObject.Destroy(sheildPrefab);
        shieldRipples = null;
        Physics.IgnoreLayerCollision(6,12,false);//还原忽略的碰撞t
    }

    public void bossShieldRemove()
    {
        GameObject.Destroy(sheildPrefab);
        shieldRipples = null;
    }


////////Buff具体效果实现
    private void level1ShieldUp(CharacterData characterData)
    {
        if(chararcterType == E_ChararcterType.player)
        {
            sheildPrefab = GameObject.Instantiate(BuffDataManager.Instance.playerShield,buffKeeper.transform);
            shieldRipples = sheildPrefab.GetComponent<ShieldRipples>();
            shieldRipples.chararcterType = E_ChararcterType.player;
            buffKeeper.GetComponent<PlayerControl>().characterBuffManager.shieldRipples = shieldRipples;//挂载实体
            shieldRipples.maxHealth = BuffDataManager.Instance.playerShieldStartHp;
            shieldRipples.currentHealth = BuffDataManager.Instance.playerShieldStartHp;

            //忽略玩家武器和护盾的碰撞
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
            //忽略敌人武器和护盾的碰撞
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
        //将护盾位置抬高
        if(sheildPrefab == null)
        {
            Debug.LogWarning("没有添加上护盾");
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

//////一些特殊实现
    public void DamageReflect(CharacterData attackData, float damage)
    {
        float reflectDamage = damage * BuffDataManager.Instance.damageReflectPercent;
        attackData.currentHealth -= reflectDamage;
    }

}



