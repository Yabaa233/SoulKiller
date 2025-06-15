using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

///<summary>


////// 部屋のトリガー


///</summary>
public enum E_RoomType
{
    enemy,
    crime,
    boss
}

public class RoomTrigger : MonoBehaviour
{
    [Header("Monster Loading Strategy")]
    public List<float> loadPolicy = new List<float>();
    [Header("Has this map been loaded before?")]
    public bool lorded = false;
    [Header("Has this map been cleared?")]
    public bool cleared = false;
    public E_RoomType roomType = E_RoomType.enemy;
    public E_BuffKind buffKind;
    protected Transform enemys;   //小さなモンスターは最上位の親オブジェクトを生成します。
    public int enemyCount;  //モンスターの数
    public Func<bool> clearCheck;   //大罪の関のクリア条件を確認してください
    public GameObject crimeRoomPrefab;  //大罪のステージメカニズムのプレファブ
    public Transform resurrectionPoint;    //プレイヤーが死亡した後の復活ポイントは、大罪のレベルだけが設定が必要です。
    private GameObject curCrimeRoom;    //現在の大罪のステージ
    protected GameObject closeCollider;   //閉鎖時の衝突体
    protected GameObject openCollider;    //開放時の衝突体
    private GameObject clearTrigger;    //クリアポータル
    public UnityAction ClearScenc;        //クリアシーンの整理
    private void Awake()
    {
        enemys = transform.Find("Enemys");
        openCollider = transform.Find("AirWallOpen").gameObject;
        closeCollider = transform.Find("AirWallClose").gameObject;
        enemyCount = enemys.childCount;
        if (roomType == E_RoomType.crime)
        {
            resurrectionPoint = transform.Find("ResurrectionPoint");
        }
        else if (roomType == E_RoomType.boss)
        {
            clearTrigger = transform.Find("ClearTrigger").gameObject;
        }
    }
    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.BossDie -= Clear;
        }
    }

    ///<summary>


    ////// パスを開く


    ///</summary>
    public void OpenWallCollider()
    {
        openCollider.SetActive(true);
        closeCollider.SetActive(false);
    }

    ///<summary>


    ////// 通路を閉じる


    ///</summary>
    public void CloseWallCollider()
    {
        openCollider.SetActive(false);
        closeCollider.SetActive(true);
    }

    /// <summary>
    /// プレイヤーが入場し、モンスターやボスのロードを開始します。
    /// </summary>
    /// <param name="other"> トリガーに入るオブジェクトの情報 </param>
    virtual protected void OnTriggerEnter(Collider other)
    {
        if (!lorded && other.tag == "Player")
        {
            lorded = true;
            (PanelManager.Instance.GetPanel("BattleMainPanel") as BattleMainPanel).FadeChange(true);
            GameManager.Instance.resetRoomList.Add(this);
            if (roomType == E_RoomType.enemy)
            {
                RoomManager.Instance.LoadEnemy(enemys, loadPolicy);
            }
            else if (roomType == E_RoomType.crime)
            {
                RoomManager.Instance.LoadEnemy(enemys, loadPolicy);
                curCrimeRoom = Instantiate(crimeRoomPrefab, transform.position, crimeRoomPrefab.transform.rotation, transform);
                int randomBGMINT = UnityEngine.Random.Range(1, 4);
                FmodManager.Instance.PlayBGM(FmodManager.Instance.BGMPathDefinitions[randomBGMINT].ambientAudioType);
                PanelManager.Instance.currentE_BuffKind = buffKind;
                PanelManager.Instance.Open(new RoomTipPanel());
            }
            else
            {
                //ボスをロード中
                RoomManager.Instance.LoadBoss(enemys);
                FmodManager.Instance.PlayBGM(FmodManager.Instance.BGMPathDefinitions[0].ambientAudioType);

            }
            CloseWallCollider();    //プレイヤーが入場し、通路を閉じます。
        }
    }

    /// <summary>
    /// プレイヤーが壁にぶつかり、プレイヤーに大罪のステージをクリアしていないことを示します。
    /// </summary>
    private void OnCollisionEnter(Collision other)
    {
        if (RoomManager.Instance.CheckCrimeRoom()) return;
        if (roomType == E_RoomType.boss)
        {
            if (!lorded && other.transform.tag == "Player")
            {
                // Debug.Log("You have not cleared all levels of the deadly sins.");
                Vector3 dir = GameManager.Instance.currentPlayer.transform.position - other.GetContact(0).point;
                dir.y = 0;
                GameManager.Instance.currentPlayer.rb.AddForce(dir.normalized * 1000, ForceMode.Impulse);
                PanelManager.Instance.Open(new BottomTips(), null, "Your sins have not yet been absolved.... [There are still major sin levels that have not been cleared.]");
            }
        }
    }

    protected void Start()
    {
        // if (loadPolicy.Count == 1) return;
        /// 計算生成戦略
        for (int i = 1; i < loadPolicy.Count; i++)
        {
            loadPolicy[i] += loadPolicy[i - 1];
        }
        if (roomType == E_RoomType.crime)
        {
            RoomManager.Instance.crimeList.Add(this);
        }
        else if (roomType == E_RoomType.boss)
        {
            GameManager.Instance.BossDie += Clear;
            RoomManager.Instance.bossRoom = this;
        }
    }

    ///<summary>


    ////// 部屋情報をリセットします


    ///</summary>
    public void ResetRoom()
    {
        lorded = false;
        cleared = false;
        if (roomType == E_RoomType.enemy)
        {
            RoomManager.Instance.DestroyEnemy(enemys);
        }
        else if (roomType == E_RoomType.crime)
        {
            RoomManager.Instance.DestroyEnemy(enemys);
            Destroy(curCrimeRoom);
            curCrimeRoom = null;
            clearCheck = null;  //イベントリスナーをクリアする
        }
        else if (roomType == E_RoomType.boss)
        {
            if (GameManager.Instance.currentBoss != null)
            {
                Destroy(GameManager.Instance.currentBoss.gameObject);
                GameManager.Instance.currentBoss = null;
            }
        }
        enemyCount = enemys.childCount;
        OpenWallCollider();
    }

    ///<summary>


    ////// 小さなモンスターが死亡し、同時にクリアしたかどうかを判断します。


    ///</summary>
    virtual public void EnemyDie()
    {
        enemyCount--;
        // Debug.Log("The monster has died.");
        if (enemyCount == 0)
        {
            Debug.Log("All the monsters are dead.");
            if (roomType == E_RoomType.enemy)
            {
                Clear();
                return;
            }
            else if (roomType == E_RoomType.crime && clearCheck())
            {
                Clear();
            }
        }
    }

    /// <summary>
    /// すべての仕掛けが破壊され、同時にモンスターがすべて死んだかどうか判断します。
    /// </summary>
    public void TrapClear()
    {
        Debug.Log("All mechanisms are destroyed.");
        if (enemyCount == 0)
        {
            Clear();
        }
    }

    /// <summary>
    /// プレイヤーがすべてのモンスターと全ての装置を倒した後のロジック
    /// </summary>
    protected void Clear()
    {
        if (cleared) return;
        cleared = true;
        (PanelManager.Instance.GetPanel("BattleMainPanel") as BattleMainPanel).FadeChange(false);
        if (roomType == E_RoomType.enemy)
        {
            // FmodManager.Instance.PlayNormalLevelEnd();
        }
        else if (roomType == E_RoomType.crime)
        {
            FmodManager.Instance.PlayNormalLevelEnd();
            GameManager.Instance.ClearLevel(this);
            if (RoomManager.Instance.CheckCrimeRoom())
            {
                RoomManager.Instance.bossRoom.OpenWallCollider();
            }
            else
            {
                RoomManager.Instance.bossRoom.CloseWallCollider();
            }
            if (ClearScenc != null)
            {
                ClearScenc();
            }
        }
        // Time.timeScale = 0.1f;
        StartCoroutine(GameManager.Instance.PlayerStop(1.5f));
        CM_Effect.Instance.CM_TransitionDim(8, 1.2f);
        Invoke("ResetTime", 2f);  //リセット時間を1秒遅延して呼び出す
        OpenWallCollider();
    }

    /// <summary>
    /// スロープレイが終了し、クリアUIが表示されます
    /// </summary>
    protected void ResetTime()
    {
        CM_Effect.Instance.CM_TransitionDim(18, 1.5f);
        if (roomType == E_RoomType.enemy)
        {
            Invoke("ShowEnemyRoomClearUI", 1.5f);
        }
        else if (roomType == E_RoomType.crime)
        {
            Invoke("ShowCrimeRoomClearUI", 1.5f);
        }
        else
        {
            Invoke("ShowBossRoomClear", 3.0f);
        }
    }

    /// <summary>
    /// ミニモンスタークリアUIを表示する
    /// </summary>
    private void ShowEnemyRoomClearUI()
    {
        PanelManager.Instance.Open(new SelectPanel(), null, "Advanced");
    }

    ///<summary>


    ////// 大罪クリアUIを表示します


    ///</summary>
    private void ShowCrimeRoomClearUI()
    {
        BuffDataManager.Instance.RecordBuffList();
        PanelManager.Instance.Open(new SelectPanel(), null, "Abandon");
    }

    /// <summary>
    /// ボスクリア特殊エフェクトを表示する
    /// </summary>
    private void ShowBossRoomClear()
    {
        clearTrigger.SetActive(true);
    }
}
