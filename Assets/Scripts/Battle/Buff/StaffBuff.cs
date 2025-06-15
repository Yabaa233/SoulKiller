using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StaffBuff : I_BuffBase
{
    [Header("Obtaining data part through external parameter passing")]
    //現在のキャラクターの参照を保存する
    [Tooltip("Current role reference")] GameObject buffKeeper;
    //現在のキャラクターのBuffManagerを保存します
    [Tooltip("Current Role Buff Manager")] CharacterBuffManager characterBuffManager;
    //現在のBUFFの種類
    [Tooltip("Current Buff Type")] E_BuffKind buffType;
    //現在のキャラクターのタイプ
    [Tooltip("Current role type")] E_ChararcterType chararcterType;

    [Tooltip("Current Buff Level")] public int currentLevel;

    [Header("Buff data section")]
    [Tooltip("Store Skill Level Function")] public Action<CharacterData> realEffect;
    [Tooltip("Function Level List")] public List<Action<CharacterData>> levelEffect;
    [Tooltip("PlayerContorl")] public PlayerControl playerControl;

    public StaffBuff(E_ChararcterType _chararcterType, int level = 1)
    {
        //代入
        this.currentLevel = level;
        buffType = E_BuffKind.StaffBuff;
        chararcterType = _chararcterType;

        //初期化リスト
        levelEffect = new List<Action<CharacterData>>();
    }

    public void OnAdd(GameObject _buffKeeper)
    {
        this.buffKeeper = _buffKeeper;
        Init();//Buffの初期化と代入
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

    /// <summary>
    /// バフの更新
    /// </summary>
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
    /// バフの初期化
    /// </summary>
    public void Init()
    {
        levelEffect.Add(level1StaffBuff);
        levelEffect.Add(level2StaffBuff);
        levelEffect.Add(level3StaffBuff);
        levelEffect.Add(level4StaffBuff);

        if (currentLevel > levelEffect.Count)
        {
            Debug.Log("The assigned level exceeds the current level limit of the Buff.");
            return;
        }

        for (int i = 0; i < currentLevel; i++)//スキルレベルに応じて効果を追加する
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

    //////異なるタイプの敵のデータを取得し、それに対応するBuffを適用します。
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

    ///////Buffが削除されたときに属性を復元します。
    private void PlayerStaffRemove()
    {
        Debug.Log("Already removed");
        if (currentLevel >= 2)
        {
            playerControl.staffHoldTime /= (1 - BuffDataManager.Instance.playerStaffStoragePercent);
        }
        GunControl gunControl = playerControl.gunControl;
        gunControl.magicBallSize = 1;
        //ボールのサイズを削除する
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

    //////////バフ効果の具体的な実装

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
