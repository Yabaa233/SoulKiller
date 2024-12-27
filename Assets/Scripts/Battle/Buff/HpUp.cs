using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class HpUp : I_BuffBase
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
    [Tooltip("存放数据")] CharacterData characterData;
    [Tooltip("存放技能等级函数")] public Action<CharacterData> realEffect;
    [Tooltip("函数等级列表")] public List<Action<CharacterData>> levelEffect;
    [Tooltip("是否开启吸血")] public bool isHpSteal;


    public HpUp(E_ChararcterType _chararcterType, int level = 1)
    {
        //赋值
        this.currentLevel = level;
        buffType = E_BuffKind.HpUp;
        chararcterType = _chararcterType;
        isHpSteal = false;//默认不开启吸血

        //初始化列表
        levelEffect = new List<Action<CharacterData>>();
    }


    //添加血量增幅
    public void OnAdd(GameObject _buffKeeper)
    {
        this.buffKeeper = _buffKeeper;
        // Debug.Log("HpUp被添加");
        //翻倍当前血量和最大血量
        Init();
        switch (chararcterType)
        {
            case E_ChararcterType.player: PlayerHpUp(); break;
            case E_ChararcterType.enemy: EnemyHpUp(); break;
            case E_ChararcterType.boss: BossHpUp(); break;
        }

        ClearDelegate();
    }

    /// <summary>
    /// 跟随实体每一帧进行更新
    /// </summary>
    public void OnUpdate(float deltaTime)
    {

    }
    /// <summary>
    /// 当从实体移除时
    /// </summary>
    public void OnRemove()
    {
        switch (chararcterType)
        {
            case E_ChararcterType.player: PlayerHpUpRemove(); break;
            case E_ChararcterType.enemy: EnemyHpUpRemove(); break;
            case E_ChararcterType.boss: BossHpUpRemove(); break;
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
    /// 初始化调用的函数
    /// </summary>
    public void Init()
    {
        levelEffect.Add(level1HpUp);
        levelEffect.Add(level2HpUp);
        levelEffect.Add(level3HpUp);
        levelEffect.Add(level4HpUp);

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

    //具体操作
    private void PlayerHpUp()
    {
        PlayerControl playerControl = buffKeeper.GetComponent<PlayerControl>();
        characterData = playerControl.characterData;
        characterBuffManager = playerControl.characterBuffManager;

        realEffect(characterData);
    }

    private void EnemyHpUp()
    {
        BaseEnemyControl enemyControl = buffKeeper.GetComponent<BaseEnemyControl>();
        characterData = enemyControl.enemyData;
        characterBuffManager = enemyControl.characterBuffManager;

        realEffect(characterData);
    }

    private void BossHpUp()
    {
        BossControl bossControl = buffKeeper.GetComponent<BossControl>();
        characterData = bossControl.bossData;
        characterBuffManager = bossControl.characterBuffManager;

        realEffect(characterData);
    }
    ////Buff移除的时候还原属性状态

    private void PlayerHpUpRemove()
    {
        characterData.maxHealth = characterData.BaseHealth;
        if (characterData.currentHealth > characterData.maxHealth)
        {
            characterData.currentHealth = characterData.maxHealth;
        }
        isHpSteal = false;
    }

    private void EnemyHpUpRemove()
    {
        characterData.maxHealth = characterData.BaseHealth;
        if (characterData.currentHealth > characterData.maxHealth)
        {
            characterData.currentHealth = characterData.maxHealth;
        }
        isHpSteal = false;
    }

    private void BossHpUpRemove()
    {
        characterData.maxHealth = characterData.BaseHealth;
        if (characterData.currentHealth > characterData.maxHealth)
        {
            characterData.currentHealth = characterData.maxHealth;
        }
        isHpSteal = false;
    }

    ////////////Buff具体实现
    public void level1HpUp(CharacterData characterData)
    {
        if (chararcterType == E_ChararcterType.player)
        {
            characterData.maxHealth += BuffDataManager.Instance.playerlevel1HpUp;
            characterData.currentHealth += BuffDataManager.Instance.playerlevel1HpUp;
        }
        else if (chararcterType == E_ChararcterType.enemy)
        {
            characterData.maxHealth += BuffDataManager.Instance.enemylevel1HpUp;
            characterData.currentHealth += BuffDataManager.Instance.enemylevel1HpUp;
        }
        else if (chararcterType == E_ChararcterType.boss)
        {
            characterData.maxHealth += BuffDataManager.Instance.bosslevel1HpUp;
            characterData.currentHealth += BuffDataManager.Instance.bosslevel1HpUp;
        }
    }

    public void level2HpUp(CharacterData characterData)
    {
         if (chararcterType == E_ChararcterType.player)
        {
            characterData.maxHealth += BuffDataManager.Instance.playerlevel2HpUp;
            characterData.currentHealth += BuffDataManager.Instance.playerlevel2HpUp;
        }
        else if (chararcterType == E_ChararcterType.enemy)
        {
            characterData.maxHealth += BuffDataManager.Instance.enemylevel2HpUp;
            characterData.currentHealth += BuffDataManager.Instance.enemylevel2HpUp;
        }
        else if (chararcterType == E_ChararcterType.boss)
        {
            characterData.maxHealth += BuffDataManager.Instance.bosslevel2HpUp;
            characterData.currentHealth += BuffDataManager.Instance.bosslevel2HpUp;
        }
    }

    public void level3HpUp(CharacterData characterData)
    {
        if (chararcterType == E_ChararcterType.player)
        {
            isHpSteal = true;
        }
        else if (chararcterType == E_ChararcterType.enemy)
        {
            characterData.maxHealth += BuffDataManager.Instance.enemylevel3HpUp;
            characterData.currentHealth += BuffDataManager.Instance.enemylevel3HpUp;
        }
        else if (chararcterType == E_ChararcterType.boss)
        {
            characterData.maxHealth += BuffDataManager.Instance.bosslevel4HpUp;
            characterData.currentHealth += BuffDataManager.Instance.bosslevel4HpUp;
        }
    }

    public void level4HpUp(CharacterData characterData)//ToDo 等级四的操作
    {
        if (chararcterType == E_ChararcterType.player)
        {

        }
        else if (chararcterType == E_ChararcterType.enemy)
        {
            characterData.maxHealth += BuffDataManager.Instance.enemylevel4HpUp;
            characterData.currentHealth += BuffDataManager.Instance.enemylevel4HpUp;
        }
        else if (chararcterType == E_ChararcterType.boss)
        {
            characterData.maxHealth += BuffDataManager.Instance.bosslevel4HpUp;
            characterData.currentHealth += BuffDataManager.Instance.bosslevel4HpUp;
        }
    }

    ///////////特殊机制的实现


    /// <summary>
    /// 吸血
    /// </summary>
    /// <param name="damage">当次伤害值</param>
    public void ReturnHp(float damage)
    {
        if (!isHpSteal)//没有开启HpSteal
        {
            return;
        }
        float steal = 0;
        if(currentLevel == 3)
        {
            steal = BuffDataManager.Instance.playerlevel3HpSteal;
        }
        if(currentLevel == 4)
        {
            steal = BuffDataManager.Instance.playerlevel4HpSteal;
        }
        float returnHp = damage * steal;
        characterData.currentHealth += returnHp;

        if (characterData.currentHealth > characterData.maxHealth)//如果吸血超过了上限，则归位
        {
            characterData.currentHealth = characterData.maxHealth;
        }
    }
}
