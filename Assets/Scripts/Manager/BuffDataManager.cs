using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// すべてのBuff状況を記録する
/// </summary>
[System.Serializable]
public class BuffDataManager : singleton<BuffDataManager> //以降、Monoを継承する必要はないかもしれません。
{
    [SerializeField]public List<S_BuffKindAndLevel> playerCurrentBuff = new List<S_BuffKindAndLevel>();//現在のプレイヤーが持っているBuff、第一パラメータは名前、第二パラメータはレベルです。
    [SerializeField]public List<S_BuffKindAndLevel> enemyCurrentBuff = new  List<S_BuffKindAndLevel>();//現在の敵が持つべきBuff、第一パラメータは名前、第二パラメータはレベルです。
    [SerializeField]public List<S_BuffKindAndLevel> bossCurrentBuff = new List<S_BuffKindAndLevel>();//現在のボスが持つべきBuff、第一パラメータは名前、第二パラメータはレベルです。
    [SerializeField] public List<I_BuffBase> enemyBuffList = new List<I_BuffBase>();
    [SerializeField] public List<I_BuffBase> playerBuffList = new List<I_BuffBase>(); 
    [SerializeField] public List<I_BuffBase> bossBuffList = new List<I_BuffBase>();
    [SerializeField] public List<I_BuffBase> lastPlayerBuffList = new List<I_BuffBase>(); //前の大罪ゲートのバフが記録されています。
    [SerializeField] public List<I_BuffBase> lastEnemyBuffList = new List<I_BuffBase>();
 
    [Header("プレイヤーのシールドタイプ")] public GameObject playerShield;
    [Header("敵のシールドタイプ")] public GameObject enemyShield;

    [Header("回復アイテムの回復量")] public float playerRaiseHealthy = 30;
    [Header("回復アイテムは、毎秒持続的にHPを減らします。")] public float playerHpReduce = 1;

    [Header("プレイヤーのレベル2のチャージ時間がパーセンテージで減少")] public float playerStaffStoragePercent = 0.3f;
    [Header("プレイヤーのレベル3のスタッフのAoE範囲が上昇します。")]public float playerStaffAoeUpPercent = 2f;

    [Header("プレイヤーのレベル3が弾薬の数を増やします")] public float playerBulletUpNum = 60;

    [Header("プレイヤーのシールドの初期ヘルス値")] public float playerShieldStartHp = 1f;
    [Header("プレイヤーのレベル3シールドは、周期的なダメージのCDが存在します。")] public float playerAoeTimeBtw = 2f;
    [Header("プレイヤーレベル4の反射ダメージ比率")] public float damageReflectPercent = 0.1f;

    [Header("敵のシールドの初期ヘルス値")] public float enemyShieldStartHp = 1f;
    [Header("敵のレベル2シールドの上昇値")]public float enemyShieldlevel2Up = 10;
    [Header("敵のレベル3シールドの数値が上昇")]public float enemyShieldlevel3Up = 10;
    [Header("敵のレベル4シールドの数値が上昇")]public float enemyShieldlevel4Up = 10;

    [Header("ボスのシールドの初期HP")] public float bossShieldStartHp = 1f;
    [Header("Bosslevel2のシールド強化値")]public float bossShieldlevel2Up = 10;
    [Header("Bosslevel3のシールド強化値")]public float bossShieldlevel3Up = 10;
    [Header("Bosslevel4のシールド強化値")]public float bossShieldlevel4Up = 10;
    
    [Header("プレイヤーのレベル1の移動速度ボーナス")] public float playerlevel1SpeedUp = 10;
    [Header("プレイヤーのレベル2の移動速度ボーナス")] public float playerlevel2SpeedUp = 10;
    [Header("プレイヤーのレベル3の移動速度ボーナス")] public float playerlevel3SpeedUp = 10;
    [Header("プレイヤーのレベル4の移動速度ボーナス")] public float playerlevel4SpeedUp = 10;

    [Header("敵のレベル1の移動速度ボーナス")] public float enemyleve1SpeedUp = 2;
    [Header("敵のレベル2の移動速度ボーナス")] public float enemyleve2SpeedUp = 2;
    [Header("敵のレベル3の移動速度ボーナス")] public float enemylevel3SpeedUp = 2;
    [Header("敵のレベル4の移動速度ボーナス")] public float enemylevel4SpeedUp = 2;

    [Header("Bosslevel1の移動速度ボーナス")] public float bossleve1SpeedUp = 5;
    [Header("Bosslevel2の移動速度ボーナス")] public float bossleve2SpeedUp = 5;
    [Header("Bosslevel3の移動速度ボーナス")] public float bosslevel3SpeedUp = 5;
    [Header("Bosslevel4の移動速度ボーナス")] public float bosslevel4SpeedUp = 5;

    [Header("プレイヤーレベル1のクリティカルヒットの確率")] public float level1Critical = 0.2f;
    [Header("プレイヤーのレベル1のクリティカルダメージ")] public float level1CriticalDamage = 1f;
    [Header("プレイヤーレベル2のクリティカルヒットの確率が増加")] public float level2Critical = 0.1f;
    [Header("プレイヤーレベル3のクリティカルヒットの確率が増加")] public float level3Critical = 0.1f;
    [Header("プレイヤーのレベル4のクリティカルダメージ増幅")] public float level4Critical = 0.1f;

    [Header("敵のレベル1のクリティカルヒットの確率")] public float emylevel1Critical = 0.1f;
    [Header("敵のレベル1のクリティカルダメージ")] public float emylevel1CriticalDamage = 0f;
    [Header("敵のレベル2のクリティカルヒットの確率が増加")] public float emylevel2Critical = 0.05f;
    [Header("敵のレベル3のクリティカルヒットの確率が増加")] public float emylevel3Critical = 0.05f;
    [Header("敵のレベル4のクリティカルダメージ増加")] public float emylevel4Critical = 0.25f;

    [Header("Bosslevel1のクリティカルヒットの確率")] public float bosslevel1Critical = 0.15f;
    [Header("Bosslevel1のクリティカルダメージ")] public float bosslevel1CriticalDamage = 0.5f;
    [Header("Bosslevel2のクリティカルヒット率の増幅")] public float bosslevel2Critical = 0.1f;
    [Header("Bosslevel3のクリティカルヒットの確率が増加")] public float bosslevel3Critical = 0.1f;
    [Header("Bosslevel4のクリティカルダメージ増加率")] public float bosslevel4Critical = 0.5f;


    [Header("プレイヤーのレベル1の体力が上昇します。")] public float playerlevel1HpUp = 50f;
    [Header("プレイヤーleve2の体力が上昇しました。")]public float playerlevel2HpUp = 50f;
    [Header("プレイヤーレベル3のライフ吸収比率")]public float playerlevel3HpSteal = 0.1f;
    [Header("プレイヤーleve4のライフ吸収比率")]public float playerlevel4HpSteal = 0.1f;

    [Header("敵のレベル1の体力が上昇しました")] public float enemylevel1HpUp = 10f;
    [Header("敵のレベル2の体力が上昇しました。")]public float enemylevel2HpUp = 10f;
    [Header("敵のレベル3の体力が上昇しました。")]public float enemylevel3HpUp = 0.1f;
    [Header("敵のレベル4の体力が上昇しました")]public float enemylevel4HpUp = 0.3f;

    [Header("Bosslevel1の体力が上昇しました")] public float bosslevel1HpUp = 10f;
    [Header("Bosslevel2の体力が上昇しました")]public float bosslevel2HpUp = 10f;
    [Header("Bossleve3の体力が上昇しました")]public float bosslevel3HpUp = 10f;
    [Header("Bossleve4の体力が上昇しました")]public float bosslevel4HpUp = 10f; 

    ///<summary>
 

    ////// 敵のバフ表示UIを更新する
 

    ///</summary>
    public void RefreshEnemyBuff()
    {
        List<S_BuffKindAndLevel> currentBuffDic = new List<S_BuffKindAndLevel>();
        foreach (var buff in enemyBuffList)
        {
            S_BuffKindAndLevel s = new S_BuffKindAndLevel();
            s.buffKind = buff.GetBuffType();
            s.level = buff.GetLevel();
            currentBuffDic.Add(s);
        }

        enemyCurrentBuff = currentBuffDic;
    }


    /// <summary>
    /// プレイヤーのBuff表示UIを更新する
    /// </summary>
    public void RefreshPlayerBuff()
    {
        List<S_BuffKindAndLevel> currentBuffDic = new List<S_BuffKindAndLevel>();
        bool haveShield = false;
        foreach (var buff in playerBuffList)
        {
            S_BuffKindAndLevel s = new S_BuffKindAndLevel();
            s.buffKind = buff.GetBuffType();
            s.level = buff.GetLevel();
            currentBuffDic.Add(s);
            // Debug.Log("ループ");
            if(s.buffKind == E_BuffKind.ShieldBuff)
            {
                haveShield = true;
            }
        }
        playerCurrentBuff = currentBuffDic;

        //UI表示の同期
        if(haveShield)
        {
            PanelManager.Instance.SetPlayerShieldVisble(true);
        }
        else
        {
            PanelManager.Instance.SetPlayerShieldVisble(false);
        }
    }

    public void CopyBuffList(List<I_BuffBase> from,List<I_BuffBase> to)
    {
        to.Clear();
        foreach(var buff in from)
        {
            E_ChararcterType chararcterType = buff.GetChararcterType();
            int level = buff.GetLevel();
            E_BuffKind buffKind = buff.GetBuffType();
            to.Add(GenerateBuff(buffKind,chararcterType,level));
        }
    }

    ///<summary>


    ////// 敵の現在のBuffListを使用して、BossBuffListを構築します。


    ///</summary>
    /// <returns></returns>
    public List<I_BuffBase> BuildBossBuffList()
    {
        bossCurrentBuff.Clear();
        List<I_BuffBase> bossBuffList = new List<I_BuffBase>();
        foreach(var buff in enemyBuffList)
        {
            E_ChararcterType chararcterType = E_ChararcterType.boss;
            int level = buff.GetLevel();
            E_BuffKind buffKind = buff.GetBuffType();
            bossBuffList.Add(GenerateBuff(buffKind,chararcterType,level));

            S_BuffKindAndLevel bossInfo = new S_BuffKindAndLevel();
            bossInfo.buffKind = buffKind;
            bossInfo.level = level;
            bossCurrentBuff.Add(bossInfo);
        }
        
        return bossBuffList;
    }

    ///<summary>


    ////// 現在のBuffを記録する


    ///</summary>
    public void RecordBuffList()
    {
        lastPlayerBuffList = new List<I_BuffBase>(playerBuffList);
        lastEnemyBuffList = new List<I_BuffBase>(enemyBuffList);
        RefreshBuff();
        // foreach(var buff in playerBuffList)
        // {
        //     // Debug.Log(buff.GetType().ToString());
        // }
    } 

    ///<summary>
 

    ////// 前のセーブポイントに戻るBuff
 

    ///</summary>
    public void BackBuff()
    {
        playerBuffList = new List<I_BuffBase>(lastPlayerBuffList);
        enemyBuffList = new List<I_BuffBase>(lastEnemyBuffList);
        RefreshBuff();
    } 

    ///<summary>
 

    ////// 表示パネルのBuff情報を更新する
 

    ///</summary>
    public void RefreshBuff()
    {
        RefreshEnemyBuff();
        RefreshPlayerBuff();
        // RefreshShieldUI();
    }

    // public void RefreshShieldUI()
    // {

    // }

    /// <summary>
    /// すべてのバフをクリアする
    /// </summary>
    public void ClearAllBuff()
    {
        playerCurrentBuff.Clear();
        enemyCurrentBuff.Clear();
        bossCurrentBuff.Clear();
        enemyBuffList.Clear();
        playerBuffList.Clear();
        bossBuffList.Clear();
        lastEnemyBuffList.Clear();
        lastPlayerBuffList.Clear();
    }


    //ファクトリーメソッド、対応するBuffを作成します
    public I_BuffBase GenerateBuff(E_BuffKind buffKind,E_ChararcterType chararcterType,int level)
    {
        if(buffKind == E_BuffKind.HpUp)
        {
            HpUp hpUp = new HpUp(chararcterType,level);
            return hpUp;
        }
        else if(buffKind == E_BuffKind.SwordBuff)
        {
            SwordBuff swordBuff = new SwordBuff(chararcterType,level);
            return swordBuff;
        }
        else if(buffKind == E_BuffKind.GunBuff)
        {
            GunBuff gunBuff = new GunBuff(chararcterType,level);
            return gunBuff;
        }
        else if(buffKind == E_BuffKind.Damage)
        {
            Damage damage = new Damage(chararcterType,level);
            return damage;
        }
        else if(buffKind == E_BuffKind.ShieldBuff)
        {
            ShieldBuff shieldBuff = new ShieldBuff(chararcterType,level);
            return shieldBuff;
        }
        else if(buffKind == E_BuffKind.StaffBuff)
        {
            StaffBuff staffBuff = new StaffBuff(chararcterType,level);
            return staffBuff;
        }
        else if(buffKind == E_BuffKind.SpeedBuff)
        {
            SpeedBuff speedBuff = new SpeedBuff(chararcterType,level);
            return speedBuff;
        }
        return null;
    }


}

[System.Serializable]
public struct S_BuffKindAndLevel
{
    public E_BuffKind buffKind;
    public int level;
}
