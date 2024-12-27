using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StaffBuff : I_BuffBase
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
    [Tooltip("PlayerContorl")] public PlayerControl playerControl;

    public StaffBuff(E_ChararcterType _chararcterType, int level = 1)
    {
        //赋值
        this.currentLevel = level;
        buffType = E_BuffKind.StaffBuff;
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
            case E_ChararcterType.player: PlayerStaffBuff(); break;
            case E_ChararcterType.enemy: EnemyStaffBuff(); break;
            case E_ChararcterType.boss: BossStaffBuff(); break;
        }
        ClearDelegate();
    }

    public void OnRemove()
    {
        switch (chararcterType)
        {
            case E_ChararcterType.player: PlayerStaffRemove(); break;
            case E_ChararcterType.enemy: EnemyStaffRemove(); break;
            case E_ChararcterType.boss: BossStaffRemove(); break;
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
        levelEffect.Add(level1StaffBuff);
        levelEffect.Add(level2StaffBuff);
        levelEffect.Add(level3StaffBuff);
        levelEffect.Add(level4StaffBuff);

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

    //////得到不同类型敌人的数据，然后施加对应的Buff
    private void PlayerStaffBuff()
    {
        playerControl = buffKeeper.GetComponent<PlayerControl>();
        CharacterData characterData = playerControl.characterData;

        realEffect(characterData);
    }

    private void EnemyStaffBuff()
    {
        BaseEnemyControl enemyControl = buffKeeper.GetComponent<BaseEnemyControl>();
        CharacterData characterData = enemyControl.enemyData;

        realEffect(characterData);
    }

    private void BossStaffBuff()
    {
        BossControl bossControl = buffKeeper.GetComponent<BossControl>();
        CharacterData characterData = bossControl.bossData;

        realEffect(characterData);
    }

    ///////Buff移除的时候还原属性
    private void PlayerStaffRemove()
    {
        Debug.Log("已经移除");
        if (currentLevel >= 2)
        {
            playerControl.staffHoldTime /= (1 - BuffDataManager.Instance.playerStaffStoragePercent);
        }
        GunControl gunControl = playerControl.gunControl;
        gunControl.magicBallSize = 1;
        //移除法球大小
        if (currentLevel >= 3)
        {
            EffectManager.Instance.playerMagicRange.localScale /= BuffDataManager.Instance.playerStaffAoeUpPercent;
        }
    }

    private void EnemyStaffRemove()
    {

    }

    private void BossStaffRemove()
    {

    }

    //////////Buff效果的具体实现

    private void level1StaffBuff(CharacterData characterData)
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

    private void level2StaffBuff(CharacterData characterData)
    {
        if (chararcterType == E_ChararcterType.player)
        {
            playerControl.staffHoldTime *= (1 - BuffDataManager.Instance.playerStaffStoragePercent);
        }
        else if (chararcterType == E_ChararcterType.enemy)
        {

        }
        else if (chararcterType == E_ChararcterType.boss)
        {

        }
    }

    private void level3StaffBuff(CharacterData characterData)
    {
        if (chararcterType == E_ChararcterType.player)
        {
            GunControl gunControl = playerControl.gunControl;
            gunControl.magicBallSize = BuffDataManager.Instance.playerStaffAoeUpPercent;
            EffectManager.Instance.playerMagicRange.localScale *= BuffDataManager.Instance.playerStaffAoeUpPercent;
        }
        else if (chararcterType == E_ChararcterType.enemy)
        {

        }
        else if (chararcterType == E_ChararcterType.boss)
        {

        }
    }

    private void level4StaffBuff(CharacterData characterData)
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
