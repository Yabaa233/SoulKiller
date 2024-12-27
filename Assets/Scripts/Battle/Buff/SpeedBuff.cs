using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpeedBuff : I_BuffBase
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
    [Tooltip("当前可以闪避的次数")]public int ableDoge;
    [Tooltip("保存的速度")] public Vector2 storeSpeed;

    // 暂存Control的方案，避免多次GetComponent
    public PlayerControl playerControl;
    public BaseEnemyControl  enemyControl;
    public BossControl bossControl;
    public CharacterData characterData;

    public SpeedBuff(E_ChararcterType _chararcterType,int level = 1)
    {
        this.currentLevel = level;
        buffType = E_BuffKind.SpeedBuff;
        chararcterType = _chararcterType;
        ableDoge = 1;

        //初始化列表
        levelEffect = new List<Action<CharacterData>>();
    }

    public void OnAdd(GameObject _buffKeeper)
    {
        this.buffKeeper = _buffKeeper;
        switch(chararcterType)
        {
            case E_ChararcterType.player:PlayerSpeedUp();break;
            case E_ChararcterType.enemy:EnemySpeedUp();break;
            case E_ChararcterType.boss:BossSpeedUp();break;
        }
        Init();//出于初始化顺序的原因需要调换一下顺序
        realEffect(characterData);
        ClearDelegate();
    }

    public void OnRemove()
    {
        switch(chararcterType)
        {
            case E_ChararcterType.player:PlayerSpeedBuffRemove();break;
            case E_ChararcterType.enemy:EnemySpeedBuffRemove();break;
            case E_ChararcterType.boss:BossSpeedBuffRemove();break;
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
        levelEffect.Add(level1SpeedUp);
        levelEffect.Add(level2SpeedUp);
        levelEffect.Add(level3SpeedUp);
        levelEffect.Add(level4SpeedUp);
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


//////////得到不同类型敌人的敌人数据，然后施加Buff
    private void PlayerSpeedUp()
    {
        playerControl = buffKeeper.GetComponent<PlayerControl>();
        characterData = playerControl.characterData;
        storeSpeed = playerControl.speed;
    }

    private void EnemySpeedUp()
    {
        enemyControl = buffKeeper.GetComponent<BaseEnemyControl>();
        characterData = enemyControl.enemyData;
        storeSpeed = new Vector2(enemyControl.moveSpeed,enemyControl.moveSpeed);
    }

    private void BossSpeedUp()
    {
        bossControl = buffKeeper.GetComponent<BossControl>();
        characterData = bossControl.bossData;
        //TODO 速度还没存
    } 

///
/// //////Buff移除的时候还原状态
    private void PlayerSpeedBuffRemove()
    {
        playerControl.speed = storeSpeed;
        ableDoge = 1;
        // Physics.IgnoreLayerCollision(7,9,false);//重设检测
    }

    private void EnemySpeedBuffRemove()
    {
        enemyControl.moveSpeed = storeSpeed.x;
    }

    private void BossSpeedBuffRemove()
    {

    }


//////////Buff的具体效果实现
    private void level1SpeedUp(CharacterData characterData)
    {
        if(chararcterType == E_ChararcterType.player)
        {
            float raise = BuffDataManager.Instance.playerlevel1SpeedUp;
            Vector2 speedAdd = new Vector2(raise,raise);
            playerControl.speed += speedAdd;
        }
        else if(chararcterType == E_ChararcterType.enemy)
        {
            float raise = BuffDataManager.Instance.enemyleve1SpeedUp;
            enemyControl.moveSpeed += raise;
        }
        else if(chararcterType == E_ChararcterType.boss)
        {

        }
    }

    private void level2SpeedUp(CharacterData characterData)
    {
        if(chararcterType == E_ChararcterType.player)
        {
            float raise = BuffDataManager.Instance.playerlevel2SpeedUp;
            Vector2 speedAdd = new Vector2(raise,raise);
            playerControl.speed += speedAdd;
            ableDoge = 2;
        }
        else if(chararcterType == E_ChararcterType.enemy)
        {
            float raise = BuffDataManager.Instance.enemyleve2SpeedUp;
            enemyControl.moveSpeed += raise;
        }
        else if(chararcterType == E_ChararcterType.boss)
        {
            
        }
    }

    private void level3SpeedUp(CharacterData characterData)
    {
        if(chararcterType == E_ChararcterType.player)
        {
            // Debug.LogWarning("注意！现在玩家忽略了单位碰撞体积");
            // Physics.IgnoreLayerCollision(7,9);
            float raise = BuffDataManager.Instance.playerlevel3SpeedUp;
            Vector2 speedAdd = new Vector2(raise,raise);
            playerControl.speed += speedAdd;
        }
        else if(chararcterType == E_ChararcterType.enemy)
        {
            float raise = BuffDataManager.Instance.enemylevel3SpeedUp;
            enemyControl.moveSpeed += raise;
        }
        else if(chararcterType == E_ChararcterType.boss)
        {
            
        }
    }

    private void level4SpeedUp(CharacterData characterData)
    {
        if(chararcterType == E_ChararcterType.player)
        {
            ableDoge = 3;//有一次闪避次数
            float raise = BuffDataManager.Instance.playerlevel4SpeedUp;
            Vector2 speedAdd = new Vector2(raise,raise);
            playerControl.speed += speedAdd;
        }
        else if(chararcterType == E_ChararcterType.enemy)
        {
            float raise = BuffDataManager.Instance.enemylevel4SpeedUp;
            enemyControl.moveSpeed += raise;
        }
        else if(chararcterType == E_ChararcterType.boss)
        {
            
        }
    }

/////////特殊机制的实现

    /// <summary>
    /// 返回冲刺次数
    /// </summary>
    /// <returns></returns>
    public int GetDogeTimes()
    {
        return ableDoge;
    }
}
