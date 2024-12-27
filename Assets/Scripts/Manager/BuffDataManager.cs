using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 记录所有的Buff情况
/// </summary>
[System.Serializable]
public class BuffDataManager : singleton<BuffDataManager> //后续可能不用继承Mono
{
    [SerializeField]public List<S_BuffKindAndLevel> playerCurrentBuff = new List<S_BuffKindAndLevel>();//当前玩家有的Buff，第一参数为名字，第二参数为等级
    [SerializeField]public List<S_BuffKindAndLevel> enemyCurrentBuff = new  List<S_BuffKindAndLevel>();//当前敌人应该有的Buff，第一参数为名字，第二参数为等级
    [SerializeField]public List<S_BuffKindAndLevel> bossCurrentBuff = new List<S_BuffKindAndLevel>();//当前Boss应该有的Buff，第一参数为名字，第二参数为等级
    [SerializeField] public List<I_BuffBase> enemyBuffList = new List<I_BuffBase>();
    [SerializeField] public List<I_BuffBase> playerBuffList = new List<I_BuffBase>(); 
    [SerializeField] public List<I_BuffBase> bossBuffList = new List<I_BuffBase>();
    [SerializeField] public List<I_BuffBase> lastPlayerBuffList = new List<I_BuffBase>(); //记录的是上一个大罪关的Buff
    [SerializeField] public List<I_BuffBase> lastEnemyBuffList = new List<I_BuffBase>();
 
    [Header("玩家的护盾类型")] public GameObject playerShield;
    [Header("敌人的护盾类型")] public GameObject enemyShield;

    [Header("回血道具的回血量")] public float playerRaiseHealthy = 30;
    [Header("回血道具每秒持续掉血量")] public float playerHpReduce = 1;

    [Header("玩家Level2蓄力时间降低百分比")] public float playerStaffStoragePercent = 0.3f;
    [Header("玩家Level3法杖Aoe范围提升")]public float playerStaffAoeUpPercent = 2f;

    [Header("玩家Level3提升子弹数量")] public float playerBulletUpNum = 60;

    [Header("玩家护盾的初始血量")] public float playerShieldStartHp = 1f;
    [Header("玩家level3护盾存在周期伤害CD")] public float playerAoeTimeBtw = 2f;
    [Header("玩家level4反弹伤害比例")] public float damageReflectPercent = 0.1f;

    [Header("敌人护盾的初始血量")] public float enemyShieldStartHp = 1f;
    [Header("敌人level2护盾提升数值")]public float enemyShieldlevel2Up = 10;
    [Header("敌人level3护盾提升数值")]public float enemyShieldlevel3Up = 10;
    [Header("敌人level4护盾提升数值")]public float enemyShieldlevel4Up = 10;

    [Header("Boss护盾的初始血量")] public float bossShieldStartHp = 1f;
    [Header("Bosslevel2护盾提升数值")]public float bossShieldlevel2Up = 10;
    [Header("Bosslevel3护盾提升数值")]public float bossShieldlevel3Up = 10;
    [Header("Bosslevel4护盾提升数值")]public float bossShieldlevel4Up = 10;
    
    [Header("玩家level1的移动速度加成")] public float playerlevel1SpeedUp = 10;
    [Header("玩家level2的移动速度加成")] public float playerlevel2SpeedUp = 10;
    [Header("玩家level3的移动速度加成")] public float playerlevel3SpeedUp = 10;
    [Header("玩家level4的移动速度加成")] public float playerlevel4SpeedUp = 10;

    [Header("敌人level1的移动速度加成")] public float enemyleve1SpeedUp = 2;
    [Header("敌人level2的移动速度加成")] public float enemyleve2SpeedUp = 2;
    [Header("敌人level3的移动速度加成")] public float enemylevel3SpeedUp = 2;
    [Header("敌人level4的移动速度加成")] public float enemylevel4SpeedUp = 2;

    [Header("Bosslevel1的移动速度加成")] public float bossleve1SpeedUp = 5;
    [Header("Bosslevel2的移动速度加成")] public float bossleve2SpeedUp = 5;
    [Header("Bosslevel3的移动速度加成")] public float bosslevel3SpeedUp = 5;
    [Header("Bosslevel4的移动速度加成")] public float bosslevel4SpeedUp = 5;

    [Header("玩家level1的暴击几率")] public float level1Critical = 0.2f;
    [Header("玩家level1的暴击伤害")] public float level1CriticalDamage = 1f;
    [Header("玩家level2的暴击几率增幅")] public float level2Critical = 0.1f;
    [Header("玩家level3的暴击几率增幅")] public float level3Critical = 0.1f;
    [Header("玩家level4的暴击伤害增幅")] public float level4Critical = 0.1f;

    [Header("敌人level1的暴击几率")] public float emylevel1Critical = 0.1f;
    [Header("敌人level1的暴击伤害")] public float emylevel1CriticalDamage = 0f;
    [Header("敌人level2的暴击几率增幅")] public float emylevel2Critical = 0.05f;
    [Header("敌人level3的暴击几率增幅")] public float emylevel3Critical = 0.05f;
    [Header("敌人level4的暴击伤害增幅")] public float emylevel4Critical = 0.25f;

    [Header("Bosslevel1的暴击几率")] public float bosslevel1Critical = 0.15f;
    [Header("Bosslevel1的暴击伤害")] public float bosslevel1CriticalDamage = 0.5f;
    [Header("Bosslevel2的暴击几率增幅")] public float bosslevel2Critical = 0.1f;
    [Header("Bosslevel3的暴击几率增幅")] public float bosslevel3Critical = 0.1f;
    [Header("Bosslevel4的暴击伤害增幅")] public float bosslevel4Critical = 0.5f;


    [Header("玩家level1的生命值提高")] public float playerlevel1HpUp = 50f;
    [Header("玩家leve2的生命值提高")]public float playerlevel2HpUp = 50f;
    [Header("玩家level3的生命吸取比例")]public float playerlevel3HpSteal = 0.1f;
    [Header("玩家leve4的生命吸取比例")]public float playerlevel4HpSteal = 0.1f;

    [Header("敌人level1的生命值提高")] public float enemylevel1HpUp = 10f;
    [Header("敌人leve2的生命值提高")]public float enemylevel2HpUp = 10f;
    [Header("敌人level3的生命值提高")]public float enemylevel3HpUp = 0.1f;
    [Header("敌人leve4的生命值提高")]public float enemylevel4HpUp = 0.3f;

    [Header("Bosslevel1的生命值提高")] public float bosslevel1HpUp = 10f;
    [Header("Bosslevel2的生命值提高")]public float bosslevel2HpUp = 10f;
    [Header("Bossleve3的生命值提高")]public float bosslevel3HpUp = 10f;
    [Header("Bossleve4的生命值提高")]public float bosslevel4HpUp = 10f; 

    /// <summary>
    /// 刷新敌人的Buff显示UI
    /// </summary>
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
    /// 刷新玩家的Buff显示UI
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
            // Debug.Log("循环");
            if(s.buffKind == E_BuffKind.ShieldBuff)
            {
                haveShield = true;
            }
        }
        playerCurrentBuff = currentBuffDic;

        //同步UI显示
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

    /// <summary>
    /// 通过敌人当前的BuffList去BuildBossBuffList
    /// </summary>
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

    /// <summary>
    /// 记录当前Buff
    /// </summary>
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

    /// <summary>
    /// 回到上一存档点的Buff
    /// </summary>
    public void BackBuff()
    {
        playerBuffList = new List<I_BuffBase>(lastPlayerBuffList);
        enemyBuffList = new List<I_BuffBase>(lastEnemyBuffList);
        RefreshBuff();
    } 

    /// <summary>
    /// 刷新显示面板Buff信息
    /// </summary>
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
    /// 清空所有Buff
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


    //工厂方法，创建对应的Buff
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
