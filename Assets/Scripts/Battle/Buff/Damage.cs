using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Damage : I_BuffBase
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


    public Damage(E_ChararcterType _chararcterType,int level = 1)
    {  
        //赋值
        this.currentLevel = level;
        buffType = E_BuffKind.Damage;
        chararcterType = _chararcterType;

        //初始化列表
        levelEffect = new List<Action<CharacterData>>();

    }
    public void OnAdd(GameObject _buffKeeper)
    {
        this.buffKeeper = _buffKeeper;
        Init();//初始化Buff赋值
        switch(chararcterType)
        {
            case E_ChararcterType.player:PlayerDamageUp();break;
            case E_ChararcterType.enemy:EnemyDamageUp();break;
            case E_ChararcterType.boss:BossDamageUp();break;
        }
        ClearDelegate();
    }

    public void OnRemove()
    {
        switch(chararcterType)
        {
            case E_ChararcterType.player:PlayerDamageRemove();break;
            case E_ChararcterType.enemy:EnemyDamageRemove();break;
            case E_ChararcterType.boss:BossDamageRemove();break;
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
        levelEffect.Add(level1DamageUp);
        levelEffect.Add(level2DamageUp);
        levelEffect.Add(level3DamageUp);
        levelEffect.Add(level4DamageUp);

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


///////得到不同类型敌人的数据，然后施加Buff
    private void PlayerDamageUp()
    {
        PlayerControl playerControl = buffKeeper.GetComponent<PlayerControl>();
        CharacterData characterData = playerControl.characterData;

        realEffect(characterData);
    }

    private void EnemyDamageUp()
    {
        BaseEnemyControl enemyControl = buffKeeper.GetComponent<BaseEnemyControl>();
        CharacterData characterData = enemyControl.enemyData;

        realEffect(characterData);
    }

    private void BossDamageUp()
    {
        BossControl bossControl = buffKeeper.GetComponent<BossControl>();
        CharacterData characterData = bossControl.bossData;

        realEffect(characterData);
    }
/////////Buff移除时还原属性状态
    private void PlayerDamageRemove()
    {
        PlayerControl playerControl = buffKeeper.GetComponent<PlayerControl>();
        CharacterData characterData = playerControl.characterData;

        characterData.currentCritical = characterData.BaseCritical;
        characterData.currentCriticalDamage = characterData.BaseCriticalDamage;
    }

    private void EnemyDamageRemove()
    {
        BaseEnemyControl enemyControl = buffKeeper.GetComponent<BaseEnemyControl>();
        CharacterData enemyData = enemyControl.enemyData;

        enemyData.currentCritical = enemyData.BaseCritical;
        enemyData.currentCriticalDamage = enemyData.BaseCriticalDamage;
    }

    private void BossDamageRemove()
    {
        BossControl bossControl = buffKeeper.GetComponent<BossControl>();
        CharacterData characterData = bossControl.bossData;

        characterData.currentCritical = characterData.BaseCritical;
        characterData.currentCriticalDamage = characterData.BaseCriticalDamage;
    }


/////////Buff的具体效果实现
    private void level1DamageUp(CharacterData characterData)
    {
        if(chararcterType == E_ChararcterType.player)
        {
            characterData.currentCritical = BuffDataManager.Instance.level1Critical;
            characterData.currentCriticalDamage = BuffDataManager.Instance.level1CriticalDamage;
        }
        else if(chararcterType == E_ChararcterType.enemy)
        {
            characterData.currentCritical = BuffDataManager.Instance.emylevel1Critical;
            characterData.currentCriticalDamage = BuffDataManager.Instance.emylevel1CriticalDamage;
        }
        else if(chararcterType == E_ChararcterType.boss)
        {
            characterData.currentCritical = BuffDataManager.Instance.bosslevel1Critical;
            characterData.currentCriticalDamage = BuffDataManager.Instance.bosslevel1CriticalDamage;
        }
    }

    private void level2DamageUp(CharacterData characterData)//第一段暴击率增幅
    {
        if(chararcterType == E_ChararcterType.player)
        {
            characterData.currentCritical += BuffDataManager.Instance.level2Critical;
        }
        else if(chararcterType == E_ChararcterType.enemy)
        {
            characterData.currentCritical += BuffDataManager.Instance.emylevel2Critical;
        }
        else if(chararcterType == E_ChararcterType.boss)
        {
            characterData.currentCritical += BuffDataManager.Instance.bosslevel2Critical;
        }
    }
    
    private void level3DamageUp(CharacterData characterData)//第二段暴击率增幅
    {
        if(chararcterType == E_ChararcterType.player)
        {
            characterData.currentCritical += BuffDataManager.Instance.level3Critical;
        }
        else if(chararcterType == E_ChararcterType.enemy)
        {
            characterData.currentCritical += BuffDataManager.Instance.emylevel3Critical;
        }
        else if(chararcterType == E_ChararcterType.boss)
        {
            characterData.currentCritical += BuffDataManager.Instance.bosslevel3Critical;
        }
    }

    private void level4DamageUp(CharacterData characterData)//第三段暴击伤害增幅
    {
        if(chararcterType == E_ChararcterType.player)
        {
            characterData.currentCriticalDamage += BuffDataManager.Instance.level4Critical;
        }
        else if(chararcterType == E_ChararcterType.enemy)
        {
            characterData.currentCriticalDamage += BuffDataManager.Instance.emylevel4Critical;
        }
        else if(chararcterType == E_ChararcterType.boss)
        {
            characterData.currentCriticalDamage += BuffDataManager.Instance.bosslevel4Critical;
        }
    }
}
