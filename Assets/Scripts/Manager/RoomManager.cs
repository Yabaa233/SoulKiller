using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : singleton<RoomManager>
{
    //RoomManagerにRoomManagerスクリプトをアタッチし、同時に各モンスターのプレハブを設定します。
    //その後、各Room子オブジェクトにColliderを付け、Triggerをチェックして、プレイヤーの入場検出として使用し、同時にRoomTriggerのスクリプトをマウントします。
    //その後、RoomTrigger内でモンスター生成比率を設定するだけです。
    //RoomTriggerとRoomManagerの中のモンスターは一対一の対応関係にあります。つまり、RoomTriggerの最初のモンスターのプレハブは、RoomManagerで保存されている最初のプレハブです。
    //その後、Roomの子オブジェクトの下にさらにEnemysの子オブジェクトを追加します。
    //Enemysの下には各モンスターの出生点が保存されており、その出生点の数に応じて対応する数のモンスターがランダムに生成されます。
    // public List<I_BuffBase> enemyBuffList = new List<I_BuffBase>();
    public List<I_BuffBase> bossBuffList;
    public List<I_BuffBase> bossSortedBuffList = new List<I_BuffBase>();
    [Header("モンスタープレファブストレージリスト")]
    public List<GameObject> enemyTypeList = new List<GameObject>();
    [Header("ボスプレファブストレージ")]
    public GameObject bossPrefab;
    public List<RoomTrigger> crimeList; //大罪全クリアを検出するために使用し、ボスの部屋を開放します。
    public RoomTrigger bossRoom;    //開始待ちのボスルーム
    override protected void Awake()
    {
        base.Awake();
    }
    protected void Start()
    {
        // BuffDataManager.Instance.bossBuffList.Add(new HpUp(E_ChararcterType.boss, 4));//回復
        // BuffDataManager.Instance.bossBuffList.Add(new Damage(E_ChararcterType.boss, 4));//狂暴
        // BuffDataManager.Instance.bossBuffList.Add(new ShieldBuff(E_ChararcterType.boss, 1));//シールド
        // BuffDataManager.Instance.bossBuffList.Add(new SwordBuff(E_ChararcterType.boss, 1));//三連撃
        // BuffDataManager.Instance.bossBuffList.Add(new StaffBuff(E_ChararcterType.boss, 1));//召喚/シャベル
        // BuffDataManager.Instance.bossBuffList.Add(new SpeedBuff(E_ChararcterType.boss, 1));//突進
        // BuffDataManager.Instance.bossBuffList.Add(new GunBuff(E_ChararcterType.boss, 1));//弾幕
    }

    ///<summary>


    ////// 現在のプレイヤーが入室した部屋のモンスターをロードしてください。


    ///</summary>
    /// <param name="enemyBirthPoints"> モンスターの出生地点 </param>
    /// <param name="loadPolicy"> モンスターのロード確率 </param>
    public void LoadEnemy(Transform enemyBirthPoints, List<float> loadPolicy)
    {
        int enemyCount = enemyBirthPoints.childCount;
        int enemyTypeCount = enemyTypeList.Count;
        float randNum;
        //モンスターの選択、enemyCount個のモンスターを生成します。
        for (int i = 0; i < enemyCount; i++)
        {
            randNum = Random.Range(0f, loadPolicy[enemyTypeCount - 1]);   //最後のものは全確率です。
            for (int j = 0; j < enemyTypeCount; j++)
            {
                if (randNum < loadPolicy[j])
                {
                    //現在のモンスターが確定し、ロード中です。
                    GameObject newEnemy = Instantiate(enemyTypeList[j], enemyBirthPoints.GetChild(i));
                    //モンスターにバフを追加する
                    newEnemy.SetActive(true);
                    newEnemy.GetComponent<BaseEnemyControl>().characterBuffManager.BuffReBuild(BuffDataManager.Instance.enemyBuffList, newEnemy);
                    break;
                }
            }
        }

    }

    /// <summary>
    /// すべてのモンスターを排除する
    /// </summary>
    /// <param name="enemyBirthPoints"> モンスターの出現地点 </param>
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

    ///<summary>


    ////// すべての大罪の関を通過したかどうかを確認し、もし通過済みであれば、ボスの部屋のコライダーを開始します。


    ///</summary>
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
    /// ボスのバフを段階的な優先順位で並べ替えます。
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
    /// ボスのバフのリビルド方法
    /// </summary>
    /// <param name="bossControl"> BossControlスクリプトは、BuffManagerを取得するために使用されます </param>
    /// <param name="canUseBuffCount"> 現在使用可能なBuffの数 </param>
    public void BossBuffRebuild(BossControl bossControl, int canUseBuffCount)
    {
        bossControl.characterBuffManager.BuffReBuild(bossBuffList.GetRange(0, canUseBuffCount), bossControl.gameObject);
    }

    /// <summary>
    /// ボスをロード中
    /// </summary>
    /// <param name="bossBirthPoints"></param>
    public void LoadBoss(Transform bossBirthPoints)
    {
        GameObject newBoss = Instantiate(bossPrefab, bossBirthPoints);
        // newBoss.SetActive(false);
        newBoss.SetActive(true);
        bossBuffList = BuffDataManager.Instance.BuildBossBuffList();
        SortBossBuffList(); //BossBuffを並べ替える
        //通常のロードでは、最初の2つのBuffのみがロードされます。
        BossBuffRebuild(newBoss.GetComponent<BossControl>(), 2);
    }
}
