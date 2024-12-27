using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GunBuff : I_BuffBase
{
    [Header("外部传参获得数据部分")]
    //存当前角色的引用
    [Tooltip("当前角色引用")] GameObject buffKeeper;
    //存当前角色的BuffManager
    [Tooltip("当前角色Buff管理器")] CharacterBuffManager characterBuffManager;
    //当前BUFF种类
    [Tooltip("当前Buff类型")] E_BuffKind buffType;
    //当前角色的类型
    [Tooltip("当前角色类型")] E_ChararcterType chararcterType;

    [Tooltip("当前Buff等级")] public int currentLevel;

    [Header("Buff数据部分")]
    [Tooltip("存放技能等级函数")] public Action<CharacterData> realEffect;
    [Tooltip("函数等级列表")] public List<Action<CharacterData>> levelEffect;

    public GunBuff(E_ChararcterType _chararcterType, int level = 1)
    {
        //赋值
        this.currentLevel = level;
        buffType = E_BuffKind.GunBuff;
        chararcterType = _chararcterType;

        //初始化列表
        levelEffect = new List<Action<CharacterData>>();
    }

    public void OnAdd(GameObject _buffKeeper)
    {
        this.buffKeeper = _buffKeeper;
        Init();//初始化Buff赋值
        switch (chararcterType)
        {
            case E_ChararcterType.player: PlayerGunBuff(); break;
            case E_ChararcterType.enemy: EnemyGunBuff(); break;
            case E_ChararcterType.boss: BossGunBuff(); break;
        }
        ClearDelegate();
    }


    public void OnRemove()
    {
        switch (chararcterType)
        {
            case E_ChararcterType.player: PlayerGunRemove(); break;
            case E_ChararcterType.enemy: EnemyGunRemove(); break;
            case E_ChararcterType.boss: BossGunRemove(); break;
        }
    }

    public void OnUpdate(float deltaTime)
    {

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
        levelEffect.Add(level1GunBuff);
        levelEffect.Add(level2GunBuff);
        levelEffect.Add(level3GunBuff);
        levelEffect.Add(level4GunBuff);

        if (currentLevel > levelEffect.Count)
        {
            Debug.Log("赋予的等级超过Buff当前等级限制");
            return;
        }

        for (int i = 0; i < currentLevel; i++)//根据技能等级添加效果
        {
            realEffect += levelEffect[i];
        }
    }

    public void ClearDelegate()
    {
        for (int i = currentLevel - 1; i >= 0; i--)
        {
            realEffect -= levelEffect[i];
        }
    }

    ////得到不同类型的敌人数据，然后施加Buff
    private void PlayerGunBuff()
    {
        PlayerControl playerControl = buffKeeper.GetComponent<PlayerControl>();
        CharacterData characterData = playerControl.characterData;

        realEffect(characterData);
    }

    private void EnemyGunBuff()
    {
        BaseEnemyControl enemyControl = buffKeeper.GetComponent<BaseEnemyControl>();
        CharacterData characterData = enemyControl.enemyData;

        realEffect(characterData);
    }

    private void BossGunBuff()
    {
        BossControl bossControl = buffKeeper.GetComponent<BossControl>();
        CharacterData characterData = bossControl.bossData;

        realEffect(characterData);
    }
    /////在Buff移除的时候还原属性
    private void PlayerGunRemove()
    {
        if (currentLevel >= 3)
        {
            GunControl gunControl = buffKeeper.GetComponent<PlayerControl>().gunControl;
            gunControl.maxAmmunition -= BuffDataManager.Instance.playerBulletUpNum;
            float percent = BuffDataManager.Instance.playerBulletUpNum / gunControl.maxAmmunition;
            gunControl.curAmmunition = gunControl.maxAmmunition;
            gunControl.autoReloadSpeed /= (1 + percent);
            gunControl.manualReloadSpeed /= (1 + percent);
        }
    }

    private void EnemyGunRemove()
    {

    }

    private void BossGunRemove()
    {

    }
    //////Buff效果的具体实现
    private void level1GunBuff(CharacterData characterData)
    {
        if (chararcterType == E_ChararcterType.player)
        {

        }
        else if (chararcterType == E_ChararcterType.enemy)
        {

        }
        else if (chararcterType == E_ChararcterType.boss)
        {

        }
    }
    private void level2GunBuff(CharacterData characterData)
    {
        if (chararcterType == E_ChararcterType.player)
        {

        }
        else if (chararcterType == E_ChararcterType.enemy)
        {

        }
        else if (chararcterType == E_ChararcterType.boss)
        {

        }
    }
    private void level3GunBuff(CharacterData characterData)
    {
        if (chararcterType == E_ChararcterType.player)
        {
            GunControl gunControl = buffKeeper.GetComponent<PlayerControl>().gunControl;
            float maxAmmo = gunControl.maxAmmunition;
            gunControl.maxAmmunition += BuffDataManager.Instance.playerBulletUpNum;
            float percent = BuffDataManager.Instance.playerBulletUpNum / maxAmmo;
            //现在不需要加快回填
            gunControl.autoReloadSpeed *= (1 + percent);
            gunControl.manualReloadSpeed *= (1 + percent);
        }
        else if (chararcterType == E_ChararcterType.enemy)
        {

        }
        else if (chararcterType == E_ChararcterType.boss)
        {

        }
    }
    private void level4GunBuff(CharacterData characterData)
    {
        if (chararcterType == E_ChararcterType.player)
        {

        }
        else if (chararcterType == E_ChararcterType.enemy)
        {

        }
        else if (chararcterType == E_ChararcterType.boss)
        {

        }
    }
}
