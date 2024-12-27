using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SwordBuff : I_BuffBase
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
    [Tooltip("玩家可以连击的次数")]public int swordComboTime = 2;

    public SwordBuff(E_ChararcterType _chararcterType,int level = 1)
    {
        //赋值
        this.currentLevel = level;
        buffType = E_BuffKind.SwordBuff;
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
            case E_ChararcterType.player:PlayerSwordBuff();break;
            case E_ChararcterType.enemy:EnemySwordBuff();break;
            case E_ChararcterType.boss:BossSwordBuff();break;
        }
        ClearDelegate();
    }

    public void OnRemove()
    {
        switch(chararcterType)
        {
            case E_ChararcterType.player:PlayerSwordRemove();break;
            case E_ChararcterType.enemy:EnemySwordRemove();break;
            case E_ChararcterType.boss:BossSwordRemove();break;
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
        levelEffect.Add(level1SwordBuff);
        levelEffect.Add(level2SwordBuff);
        levelEffect.Add(level3SwordBuff);
        levelEffect.Add(level4SwordBuff);

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

    public void ClearDelegate()
    {
        for (int i = currentLevel - 1; i >= 0; i--)
        {
            realEffect -= levelEffect[i];
        }
    }


/////////得到不同类型的敌人数据，然后施加Buff
    private void PlayerSwordBuff()
    {
        PlayerControl playerControl = buffKeeper.GetComponent<PlayerControl>();
        CharacterData characterData = playerControl.characterData;

        realEffect(characterData);
    }

    private void EnemySwordBuff()
    {
        BaseEnemyControl enemyControl = buffKeeper.GetComponent<BaseEnemyControl>();
        CharacterData characterData = enemyControl.enemyData;

        realEffect(characterData);
    }

    private void BossSwordBuff()
    {
        BossControl bossControl = buffKeeper.GetComponent<BossControl>();
        CharacterData characterData = bossControl.bossData;

        realEffect(characterData);
    }

//////buFF移除的时候还原状态
    private void PlayerSwordRemove()
    {
        swordComboTime = 2;
    }

    private void EnemySwordRemove()
    {

    }

    private void BossSwordRemove()
    {

    }

///////////Buff的具体效果实现
    private void level1SwordBuff(CharacterData characterData)
    {
        if(chararcterType == E_ChararcterType.player)
        {
            swordComboTime = 2;
        }
        else if(chararcterType == E_ChararcterType.enemy)
        {
            
        }
        else if(chararcterType == E_ChararcterType.boss)
        {

        }
    }

    private void level2SwordBuff(CharacterData characterData)
    {
        if(chararcterType == E_ChararcterType.player)
        {
            swordComboTime = 3;
        }
        else if(chararcterType == E_ChararcterType.enemy)
        {
            
        }
        else if(chararcterType == E_ChararcterType.boss)
        {
            
        }
    }

    private void level3SwordBuff(CharacterData characterData)
    {
        if(chararcterType == E_ChararcterType.player)
        {
            
        }
        else if(chararcterType == E_ChararcterType.enemy)
        {
            
        }
        else if(chararcterType == E_ChararcterType.boss)
        {
            
        }
    }   

    private void level4SwordBuff(CharacterData characterData)
    {
        if(chararcterType == E_ChararcterType.player)
        {

        }
        else if(chararcterType == E_ChararcterType.enemy)
        {
            
        }
        else if(chararcterType == E_ChararcterType.boss)
        {
            
        }
    }


    public int GetPlayerTimes()
    {
        return swordComboTime;
    }

}
