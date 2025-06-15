using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class HpUp : I_BuffBase
{
    [Header("Obtaining data part through external parameter passing")]
    //現在のキャラクターの参照を保存する
    [Tooltip("Current role reference")] GameObject buffKeeper;
    //現在のキャラクターのBuffManagerを保存します
    [Tooltip("Current Role Buff Manager")] CharacterBuffManager characterBuffManager;
    //現在のBUFFの種類
    [Tooltip("Current Buff Type")] E_BuffKind buffType;
    //現在のキャラクタータイプ
    [Tooltip("Current role type")] E_ChararcterType chararcterType;

    [Tooltip("Current Buff Level")] public int currentLevel;

    [Header("Buff data section")]
    [Tooltip("Store data")] CharacterData characterData;
    [Tooltip("Store Skill Level Function")] public Action<CharacterData> realEffect;
    [Tooltip("Function Level List")] public List<Action<CharacterData>> levelEffect;
    [Tooltip("Is the vampirism enabled?")] public bool isHpSteal;


    public HpUp(E_ChararcterType _chararcterType, int level = 1)
    {
        //代入
        this.currentLevel = level;
        buffType = E_BuffKind.HpUp;
        chararcterType = _chararcterType;
        isHpSteal = false;//デフォルトでは、吸血はオフに設定されています。

        //初期化リスト
        levelEffect = new List<Action<CharacterData>>();
    }


    //体力を増幅する機能を追加する
    public void OnAdd(GameObject _buffKeeper)
    {
        this.buffKeeper = _buffKeeper;
        // Debug.Log("HpUp has been added.");
        //現在の体力と最大体力を倍にします
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
    /// バフの更新
    /// </summary>
    public void OnUpdate(float deltaTime)
    {

    }
    /// <summary>
    /// バフの終了
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
    /// バフの初期化
    /// </summary>
    public void Init()
    {
        levelEffect.Add(level1HpUp);
        levelEffect.Add(level2HpUp);
        levelEffect.Add(level3HpUp);
        levelEffect.Add(level4HpUp);

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
    ///<summary>

    ////// 割り当てた後で、委託をクリアします。

    ///</summary>
    public void ClearDelegate()
    {
        for (int i = currentLevel - 1; i >= 0; i--)
        {
            realEffect -= levelEffect[i];
        }
    }

    //具体的な操作
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
    ////バフが削除された時に属性状態を復元します

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

    ////////////Buffの具体的な実装
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

    public void level4HpUp(CharacterData characterData)//ToDo レベル4の操作
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

    ///////////特別なメカニズムの実装


    ///<summary>



    ////// 吸血



    ///</summary>
    /// <param name="damage">一回のダメージ値</param>
    public void ReturnHp(float damage)
    {
        if (!isHpSteal)//HpStealは有効になっていません
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

        if (characterData.currentHealth > characterData.maxHealth)//上限を超えて吸血した場合、元の位置に戻ります。
        {
            characterData.currentHealth = characterData.maxHealth;
        }
    }
}
