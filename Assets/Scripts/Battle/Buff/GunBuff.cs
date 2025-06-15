using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GunBuff : I_BuffBase
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

    public GunBuff(E_ChararcterType _chararcterType, int level = 1)
    {
        //代入
        this.currentLevel = level;
        buffType = E_BuffKind.GunBuff;
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
            case E_ChararcterType.player: PlayerGunBuff(); break;
            case E_ChararcterType.enemy: EnemyGunBuff(); break;
            case E_ChararcterType.boss: BossGunBuff(); break;
        }
        ClearDelegate();
    }


    public void OnRemove()
    {
        switch (chararcterType)
        {
            case E_ChararcterType.player: PlayerGunRemove(); break;
            case E_ChararcterType.enemy: EnemyGunRemove(); break;
            case E_ChararcterType.boss: BossGunRemove(); break;
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
    /// ガンバフ
    /// </summary>
    /// <summary>
    /// バフの初期化
    /// </summary>
    public void Init()
    {
        levelEffect.Add(level1GunBuff);
        levelEffect.Add(level2GunBuff);
        levelEffect.Add(level3GunBuff);
        levelEffect.Add(level4GunBuff);

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

    ////異なるタイプの敵のデータを取得し、その後、Buffを適用します。
    private void PlayerGunBuff()
    {
        PlayerControl playerControl = buffKeeper.GetComponent<PlayerControl>();
        CharacterData characterData = playerControl.characterData;

        realEffect(characterData);
    }

    private void EnemyGunBuff()
    {
        BaseEnemyControl enemyControl = buffKeeper.GetComponent<BaseEnemyControl>();
        CharacterData characterData = enemyControl.enemyData;

        realEffect(characterData);
    }

    private void BossGunBuff()
    {
        BossControl bossControl = buffKeeper.GetComponent<BossControl>();
        CharacterData characterData = bossControl.bossData;

        realEffect(characterData);
    }
    /////Buffが削除されるときに属性を復元します
    private void PlayerGunRemove()
    {
        if (currentLevel >= 3)
        {
            GunControl gunControl = buffKeeper.GetComponent<PlayerControl>().gunControl;
            gunControl.maxAmmunition -= BuffDataManager.Instance.playerBulletUpNum;
            float percent = BuffDataManager.Instance.playerBulletUpNum / gunControl.maxAmmunition;
            gunControl.curAmmunition = gunControl.maxAmmunition;
            gunControl.autoReloadSpeed /= (1 + percent);
            gunControl.manualReloadSpeed /= (1 + percent);
        }
    }

    private void EnemyGunRemove()
    {

    }

    private void BossGunRemove()
    {

    }
    //////バフ効果の具体的な実装
    private void level1GunBuff(CharacterData characterData)
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
    private void level2GunBuff(CharacterData characterData)
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
    private void level3GunBuff(CharacterData characterData)
    {
        if (chararcterType == E_ChararcterType.player)
        {
            GunControl gunControl = buffKeeper.GetComponent<PlayerControl>().gunControl;
            float maxAmmo = gunControl.maxAmmunition;
            gunControl.maxAmmunition += BuffDataManager.Instance.playerBulletUpNum;
            float percent = BuffDataManager.Instance.playerBulletUpNum / maxAmmo;
            //現在、埋め戻しを速める必要はありません。
            gunControl.autoReloadSpeed *= (1 + percent);
            gunControl.manualReloadSpeed *= (1 + percent);
        }
        else if (chararcterType == E_ChararcterType.enemy)
        {

        }
        else if (chararcterType == E_ChararcterType.boss)
        {

        }
    }
    private void level4GunBuff(CharacterData characterData)
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
