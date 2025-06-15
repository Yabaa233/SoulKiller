using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Damage : I_BuffBase
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


    public Damage(E_ChararcterType _chararcterType,int level = 1)
    {  
        //代入
        this.currentLevel = level;
        buffType = E_BuffKind.Damage;
        chararcterType = _chararcterType;

        //初期化リスト
        levelEffect = new List<Action<CharacterData>>();

    }
    public void OnAdd(GameObject _buffKeeper)
    {
        this.buffKeeper = _buffKeeper;
        Init();//Buffの初期化と代入
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
/// ダメージバフ
/// </summary>
    public void Init()
    {
        levelEffect.Add(level1DamageUp);
        levelEffect.Add(level2DamageUp);
        levelEffect.Add(level3DamageUp);
        levelEffect.Add(level4DamageUp);

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
    public void ClearDelegate()
    {
        for (int i = currentLevel - 1; i >= 0; i--)
        {
            realEffect -= levelEffect[i];
        }
    }


///////異なるタイプの敵のデータを取得し、その後、バフを適用します。
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
/////////バフが削除された時、属性状態を元に戻します。
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


/////////Buffの具体的な効果の実現
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

    private void level2DamageUp(CharacterData characterData)//最初のクリティカルヒット率の増加幅
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
    
    private void level3DamageUp(CharacterData characterData)//第二段階のクリティカルヒット率の上昇幅
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

    private void level4DamageUp(CharacterData characterData)//第三段落のクリティカルダメージ増加
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
