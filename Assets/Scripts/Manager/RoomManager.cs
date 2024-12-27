using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : singleton<RoomManager>
{
    //RoomManager上挂RoomManager脚本，同时配置各怪物预制体进去
    //然后每一个Room子物体挂一个Collider，然后勾选Trigger，当做玩家进入的检测，同时挂载一个RoomTrigger的脚本
    //之后在RoomTrigger里面配置怪物生成比例就行了
    //RoomTrigger里和RoomManager里的怪物是一一对应的，就是RoomTrigger第一个怪物的预制体就是RoomManager里存储的第一个预制体
    //然后在Room子物体下面再挂一个Enemys的子物体
    //Enemys下面存各个怪物的出生点，到时候就会根据出生点个数去随机生成对应个数的怪物
    // public List<I_BuffBase> enemyBuffList = new List<I_BuffBase>();
    public List<I_BuffBase> bossBuffList;
    public List<I_BuffBase> bossSortedBuffList = new List<I_BuffBase>();
    [Header("怪物预制体存储列表")]
    public List<GameObject> enemyTypeList = new List<GameObject>();
    [Header("Boss预制体存储")]
    public GameObject bossPrefab;
    public List<RoomTrigger> crimeList; //用于检测是否大罪关全通，开启Boss房
    public RoomTrigger bossRoom;    //待开启的Boss房
    override protected void Awake()
    {
        base.Awake();
    }
    protected void Start()
    {
        // BuffDataManager.Instance.bossBuffList.Add(new HpUp(E_ChararcterType.boss, 4));//回血
        // BuffDataManager.Instance.bossBuffList.Add(new Damage(E_ChararcterType.boss, 4));//狂暴
        // BuffDataManager.Instance.bossBuffList.Add(new ShieldBuff(E_ChararcterType.boss, 1));//护盾
        // BuffDataManager.Instance.bossBuffList.Add(new SwordBuff(E_ChararcterType.boss, 1));//三连击
        // BuffDataManager.Instance.bossBuffList.Add(new StaffBuff(E_ChararcterType.boss, 1));//召唤/铲子
        // BuffDataManager.Instance.bossBuffList.Add(new SpeedBuff(E_ChararcterType.boss, 1));//冲刺
        // BuffDataManager.Instance.bossBuffList.Add(new GunBuff(E_ChararcterType.boss, 1));//弹幕
    }

    /// <summary>
    /// 加载当前玩家进入的房间的小怪
    /// </summary>
    /// <param name="enemyBirthPoints"> 小怪的出生点 </param>
    /// <param name="loadPolicy"> 加载小怪的概率 </param>
    public void LoadEnemy(Transform enemyBirthPoints, List<float> loadPolicy)
    {
        int enemyCount = enemyBirthPoints.childCount;
        int enemyTypeCount = enemyTypeList.Count;
        float randNum;
        //怪物选型，生成enemyCount个怪物
        for (int i = 0; i < enemyCount; i++)
        {
            randNum = Random.Range(0f, loadPolicy[enemyTypeCount - 1]);   //最后一个是总概率
            for (int j = 0; j < enemyTypeCount; j++)
            {
                if (randNum < loadPolicy[j])
                {
                    //当前怪物确定，加载
                    GameObject newEnemy = Instantiate(enemyTypeList[j], enemyBirthPoints.GetChild(i));
                    //给怪物添加Buff
                    newEnemy.SetActive(true);
                    newEnemy.GetComponent<BaseEnemyControl>().characterBuffManager.BuffReBuild(BuffDataManager.Instance.enemyBuffList, newEnemy);
                    break;
                }
            }
        }

    }

    /// <summary>
    /// 清除所有小怪
    /// </summary>
    /// <param name="enemyBirthPoints"> 小怪出生点 </param>
    public void DestroyEnemy(Transform enemyBirthPoints)
    {
        int enemyCount = enemyBirthPoints.childCount;
        int enemyChildCount = 0;
        for (int i = 0; i < enemyCount; i++)
        {
            enemyChildCount = enemyBirthPoints.GetChild(i).childCount;
            if (enemyChildCount != 0)
            {
                for (int j = 0; j < enemyChildCount; j++)
                {
                    Destroy(enemyBirthPoints.GetChild(i).GetChild(j).gameObject);
                }
            }
        }
    }

    /// <summary>
    /// 当通关大罪关时检测所有大罪关是否已通关 如果已通关 开启Boss房Collider
    /// </summary>
    public bool CheckCrimeRoom()
    {
        foreach(var i in crimeList)
        {
            if (!i.lorded)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 对Boss的Buff按阶段优先级进行排序
    /// </summary>
    private void SortBossBuffList()
    {
        I_BuffBase[] buffTemp = new I_BuffBase[7];
        E_BuffKind buffType = bossBuffList[0].GetBuffType();
        foreach (I_BuffBase i in bossBuffList)
        {
            buffTemp[(int)i.GetBuffType()] = i;
        }
        for (int i = 0; i < 7; i++)
        {
            if (buffTemp[i] != null)
            {
                bossSortedBuffList.Add(buffTemp[i]);
            }
        }
    }

    /// <summary>
    /// Boss的Buff的Rebuild方法
    /// </summary>
    /// <param name="bossControl"> BossControl脚本 用于获取BuffManager </param>
    /// <param name="canUseBuffCount"> 当前可以使用的Buff数量 </param>
    public void BossBuffRebuild(BossControl bossControl, int canUseBuffCount)
    {
        bossControl.characterBuffManager.BuffReBuild(bossBuffList.GetRange(0, canUseBuffCount), bossControl.gameObject);
    }

    /// <summary>
    /// 加载Boss
    /// </summary>
    /// <param name="bossBirthPoints"></param>
    public void LoadBoss(Transform bossBirthPoints)
    {
        GameObject newBoss = Instantiate(bossPrefab, bossBirthPoints);
        // newBoss.SetActive(false);
        newBoss.SetActive(true);
        bossBuffList = BuffDataManager.Instance.BuildBossBuffList();
        SortBossBuffList(); //对BossBuff进行排序
        //正常加载只加载前两个Buff
        BossBuffRebuild(newBoss.GetComponent<BossControl>(), 2);
    }
}
