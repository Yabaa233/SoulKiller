using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SwordBuff : I_BuffBase
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
    [Tooltip("The number of times a player can combo.")]public int swordComboTime = 2;

    public SwordBuff(E_ChararcterType _chararcterType,int level = 1)
    {
        //代入
        this.currentLevel = level;
        buffType = E_BuffKind.SwordBuff;
        chararcterType = _chararcterType;

        //初期化リスト
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
/// ソードバフ
/// </summary>
    public void Init()
    {
        levelEffect.Add(level1SwordBuff);
        levelEffect.Add(level2SwordBuff);
        levelEffect.Add(level3SwordBuff);
        levelEffect.Add(level4SwordBuff);

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


/////////異なるタイプの敵のデータを取得し、その後、Buffを適用します。
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

//////buFFが削除されるとき、状態を復元します。
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

///////////Buffの具体的な効果の実現
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
