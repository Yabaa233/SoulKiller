using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    [Header("コピーされた数値テンプレート（使用中）")]
    public CharacterData_SO characterData;
    
    /// <summary>
    /// キャラクター数値クラスを作成するためのコンストラクタ
    /// </summary>
    public CharacterData(CharacterData_SO tempCharaterData)
    {
        if (tempCharaterData != null)
        {
            characterData = tempCharaterData;
        }
        else
        {
            Debug.LogWarning("テンプレート属性がありません");
        }
        DataInit();
    }

    #region BaseData
    public float BaseHealth //キャラクターの基本生命値
    {
        get
        {
            if (characterData != null)
                return characterData.baseHealth;
            else
                Debug.LogWarning("読み取るためのデータファイルがありません");
            return 0;
        }
    }
    public float maxHealth; //レベル、装備、属性ボーナスを計算した後のヒットポイント
    public float currentHealth; //現在のHP

    public float BaseAttack //キャラクターの基本攻撃力
    {
        get
        {
            if (characterData != null)
                return characterData.baseAttack;
            else
                Debug.LogWarning("読み取るためのデータファイルがありません");
            return 0;
        }
    }
    public float currentAttack;
    public float currentComboAttack;

    public float BaseDefend //キャラクターの基本防御力
    {
        get
        {
            if (characterData != null)
                return characterData.baseDefend;
            else
                Debug.LogWarning("読み取るためのデータファイルがありません");
            return 0;
        }
    }
    public float currentDefend;

    public float BaseCritical //キャラクターの基本クリティカル率
    {
        get
        {
            if (characterData != null)
                return characterData.baseCritical;
            else
                Debug.LogWarning("読み取るためのデータファイルがありません");
            return 0;
        }
    }
    public float currentCritical;

    public float BaseCriticalDamage //キャラクターの基本クリティカルダメージ
    {
        get
        {
            if (characterData != null)
                return characterData.baseCriticalDamage;
            else
                Debug.LogWarning("読み取るためのデータファイルがありません");
            return 0;
        }
    }
    public float currentCriticalDamage;

    public float BaseCriticalDefend //キャラクターの基本クリティカル抵抗力
    {
        get
        {
            if (characterData != null)
                return characterData.baseCriticalDefend;
            else
                Debug.LogWarning("読み取るためのデータファイルがありません");
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
                Debug.LogWarning("読み取るためのデータファイルがありません");
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
                Debug.LogWarning("読み取るためのデータファイルがありません");
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
                Debug.LogWarning("読み取るためのデータファイルがありません");
            return characterData.getCriticalSound;
        }
    }
    public float currentStopTime;
    #endregion

    /// <summary>
    /// キャラクターデータの初期化
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