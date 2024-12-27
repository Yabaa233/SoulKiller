using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    [Header("复制出的数值模板（正在使用的）")]
    public CharacterData_SO characterData;
    
    /// <summary>
    /// 构造函数，用于创建一个角色数值类
    /// </summary>
    public CharacterData(CharacterData_SO tempCharaterData)
    {
        if (tempCharaterData != null)
        {
            characterData = tempCharaterData;
        }
        else
        {
            Debug.LogWarning("没有模板属性");
        }
        DataInit();
    }

    #region BaseData
    public float BaseHealth //角色基础生命值
    {
        get
        {
            if (characterData != null)
                return characterData.baseHealth;
            else
                Debug.LogWarning("没有数据文件可供读取");
            return 0;
        }
    }
    public float maxHealth; //计算等级、装备、属性加成后的生命值
    public float currentHealth; //当前血量

    public float BaseAttack //角色基础攻击力
    {
        get
        {
            if (characterData != null)
                return characterData.baseAttack;
            else
                Debug.LogWarning("没有数据文件可供读取");
            return 0;
        }
    }
    public float currentAttack;
    public float currentComboAttack;

    public float BaseDefend //角色基础防御力
    {
        get
        {
            if (characterData != null)
                return characterData.baseDefend;
            else
                Debug.LogWarning("没有数据文件可供读取");
            return 0;
        }
    }
    public float currentDefend;

    public float BaseCritical //角色基础暴击率
    {
        get
        {
            if (characterData != null)
                return characterData.baseCritical;
            else
                Debug.LogWarning("没有数据文件可供读取");
            return 0;
        }
    }
    public float currentCritical;

    public float BaseCriticalDamage //角色基础暴击伤害
    {
        get
        {
            if (characterData != null)
                return characterData.baseCriticalDamage;
            else
                Debug.LogWarning("没有数据文件可供读取");
            return 0;
        }
    }
    public float currentCriticalDamage;

    public float BaseCriticalDefend //角色基础暴击抗性
    {
        get
        {
            if (characterData != null)
                return characterData.baseCriticalDefend;
            else
                Debug.LogWarning("没有数据文件可供读取");
            return 0;
        }
    }
    public float currentCriticalDefend;

    public float BaseStopTime
    {
        get
        {
            if (characterData != null)
                return characterData.baseStopTime;
            else
                Debug.LogWarning("没有数据文件可供读取");
            return 0;
        }
    }

    public FMODUnity.EventReference getHitSound
    {
        get
        {
            if (characterData != null)
                return characterData.getHitSound;
            else
                Debug.LogWarning("没有数据文件可供读取");
                return characterData.getHitSound;
        }
    }
    public FMODUnity.EventReference getCriticalSound
    {
        get
        {
            if (characterData != null)
                return characterData.getCriticalSound;
            else
                Debug.LogWarning("没有数据文件可供读取");
            return characterData.getCriticalSound;
        }
    }
    public float currentStopTime;
    #endregion

    /// <summary>
    /// 初始化角色Data
    /// </summary>
    public void DataInit()
    {
        currentAttack = BaseAttack;
        currentDefend = BaseDefend;
        maxHealth = BaseHealth;
        currentHealth = maxHealth;
        currentCritical = BaseCritical;
        currentCriticalDamage = BaseCriticalDamage;
        currentCriticalDefend = BaseCriticalDefend;
        currentStopTime = BaseStopTime;
    }
}