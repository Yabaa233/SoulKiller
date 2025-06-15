using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



//HPアイテムが提供するバフ
/// <summary>
/// HPアイテムバフ
/// </summary>
[Serializable]
public class HpItemBuff : I_BuffBase
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
    //現在のBuffが積み重ねられた層の数
    [Tooltip("Current Buff Level")]public int currentLevel;
    [Tooltip("The cooldown of the bleeding buff.")]public CDClass HpItemBuffCD;
    [Tooltip("List of Duration for Blood Loss Buff")]public List<CDClass> HpItemBuffKeepCDList = new List<CDClass>();

    public HpItemBuff(E_ChararcterType _chararcterType,int level = 0)
    {
        //代入
        this.currentLevel = level;
        buffType = E_BuffKind.HpItemBuff;
        chararcterType = _chararcterType;

        //統一されたCDをいくつか追加してください。
        HpItemBuffCD = new CDClass();
        HpItemBuffCD.maxCDTime = 1;
        HpItemBuffCD.flag = true;
        GameManager.Instance.CDList.Add(HpItemBuffCD);

    }

    public void OnAdd(GameObject _buffKeeper)
    {
        //持続時間を追加します
        CDClass temp = new CDClass();
        temp.maxCDTime = 20;
        temp.flag = false;
        HpItemBuffKeepCDList.Add(temp);
        
        //レイヤーを一つ追加します
        currentLevel += 1;

        this.buffKeeper = _buffKeeper;
        switch(chararcterType)
        {
            case E_ChararcterType.player:PlayerHpItemBuff();break;
            case E_ChararcterType.enemy:break;
            case E_ChararcterType.boss:break; 
        }
    }

    public void OnUpdate(float deltaTime)
    {
        CDUpdate();//現在のCDを更新し、リストを処理します。
        if(currentLevel <=0)//レイヤーがなくなったら、このバフを削除してください。
        {
            SelfRemove();
            return;
        }
        if(HpItemBuffCD.flag)
        {
            switch(chararcterType)
            {
                case E_ChararcterType.player:UpdatePlayerHpItemBuff();break;
                case E_ChararcterType.enemy:break;
                case E_ChararcterType.boss:break; 
            }
            HpItemBuffCD.flag = false;
        }
    }

    //CDを更新する
    /// <summary>
    /// バフの更新
    /// </summary>
    public void CDUpdate()
    {
        List<CDClass> HpItemFuzhu = new List<CDClass>(HpItemBuffKeepCDList); //補助配列をスキャンし、元の配列の要素を削除します。
        foreach (CDClass temp in HpItemFuzhu.ToArray())
        {
            if (!temp.flag && temp.curTime < temp.maxCDTime)
            {
                temp.curTime += Time.deltaTime;
                if (temp.curTime > temp.maxCDTime)
                {
                    HpItemBuffKeepCDList.Remove(temp);
                    currentLevel -=1 ;//バフを一つ減らす
                }
            }
        }
    }

    public void OnRemove()
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

////////////
    /// <summary>
    /// バフの初期化
    /// </summary>
    public void PlayerHpItemBuff()//OnAdd
    {
        
        PlayerControl playerControl = buffKeeper.GetComponent<PlayerControl>();
        CharacterData characterData = playerControl.characterData;

        characterData.currentHealth += BuffDataManager.Instance.playerRaiseHealthy;
        if(characterData.currentHealth > characterData.maxHealth)
        {
            characterData.currentHealth = characterData.maxHealth;
        }
    }

    /// <summary>
    /// バフの更新
    /// </summary>
    public void UpdatePlayerHpItemBuff()//Update
    {
        PlayerControl playerControl = buffKeeper.GetComponent<PlayerControl>();
        CharacterData characterData = playerControl.characterData;

        float damage = BuffDataManager.Instance.playerHpReduce * currentLevel;
        characterData.currentHealth -= damage;

        PanelManager.Instance.GenerateDamageNum(damage,playerControl.gameObject.transform,false,true);//ダメージ数値を生成する

        GameManager.Instance.PlayerHealthCheck();//プレイヤーのHPをチェックする
    }

    /// <summary>
    /// バフの終了
    /// </summary>
    //自動削除方法
    public void SelfRemove()
    {
        switch(chararcterType)
        {
            case E_ChararcterType.player:PlayerSelfRemove();break;
            case E_ChararcterType.enemy:break;
            case E_ChararcterType.boss:break; 
        }
    }

    public void PlayerSelfRemove()
    {
        Debug.Log("Executed removal");
        PlayerControl playerControl = buffKeeper.GetComponent<PlayerControl>();
        playerControl.characterBuffManager.RemoveBuff(this);
    }
    
}
