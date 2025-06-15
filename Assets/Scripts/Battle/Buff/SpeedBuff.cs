using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpeedBuff : I_BuffBase
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
    [Tooltip("The current number of times that can be dodged")]public int ableDoge;
    [Tooltip("The speed of saving")] public Vector2 storeSpeed;

    // コントロールのスキームを一時保存し、GetComponentを何度も行うのを避けます。
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

        //初期化リスト
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
        Init();//初期化の順序に関する理由から、順序を変更する必要があります。
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
        levelEffect.Add(level1SpeedUp);
        levelEffect.Add(level2SpeedUp);
        levelEffect.Add(level3SpeedUp);
        levelEffect.Add(level4SpeedUp);
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


//////////異なるタイプの敵から敵のデータを取得し、その後にBuffを適用します。
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
        //TODO 速度はまだ保存されていません
    } 

/// <summary>
/// バフの終了
/// </summary>
    private void PlayerSpeedBuffRemove()
    {
        playerControl.speed = storeSpeed;
        ableDoge = 1;
        // Physics.IgnoreLayerCollision(7,9,false);//検出をリセット
    }

    private void EnemySpeedBuffRemove()
    {
        enemyControl.moveSpeed = storeSpeed.x;
    }

    private void BossSpeedBuffRemove()
    {

    }


//////////Buffの具体的な効果の実現
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
            // Debug.LogWarning("Attention! The player is currently ignoring the collision volume of the unit.");
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
            ableDoge = 3;//一度の回避回数
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

/////////特別なメカニズムの実装

    /// <summary>
    /// スプリントの数を返します
    /// </summary>
    /// <returns></returns>
    public int GetDogeTimes()
    {
        return ableDoge;
    }
}
