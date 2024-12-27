using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 场景类型
/// </summary>
public enum E_RoomType
{
    enemy,
    crime,
    boss
}

public class RoomTrigger : MonoBehaviour
{
    [Header("怪物加载策略")]
    public List<float> loadPolicy = new List<float>();
    [Header("该地图是否已被加载过")]
    public bool lorded = false;
    [Header("该地图是否已通关")]
    public bool cleared = false;
    public E_RoomType roomType = E_RoomType.enemy;
    public E_BuffKind buffKind;
    protected Transform enemys;   //小怪生成最顶层父物体
    public int enemyCount;  //小怪数量
    public Func<bool> clearCheck;   //检查大罪关通关条件
    public GameObject crimeRoomPrefab;  //大罪关卡机关预制体
    public Transform resurrectionPoint;    //玩家死亡后的复活点 只有大罪关卡需要配置
    private GameObject curCrimeRoom;    //当前的大罪关卡
    protected GameObject closeCollider;   //封闭时碰撞体
    protected GameObject openCollider;    //开放时碰撞体
    private GameObject clearTrigger;    //通关传送门
    public UnityAction ClearScenc;        //通关清理场景
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

    /// <summary>
    /// 开启通路
    /// </summary>
    public void OpenWallCollider()
    {
        openCollider.SetActive(true);
        closeCollider.SetActive(false);
    }

    /// <summary>
    /// 关闭通路
    /// </summary>
    public void CloseWallCollider()
    {
        openCollider.SetActive(false);
        closeCollider.SetActive(true);
    }

    /// <summary>
    /// 玩家进入，开始加载小怪或Boss
    /// </summary>
    /// <param name="other"> 进入Trigger的物体信息 </param>
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
                //加载Boss
                RoomManager.Instance.LoadBoss(enemys);
                FmodManager.Instance.PlayBGM(FmodManager.Instance.BGMPathDefinitions[0].ambientAudioType);

            }
            CloseWallCollider();    //玩家进入，关闭通路
        }
    }

    /// <summary>
    /// 玩家撞墙，提示玩家没有通关大罪关
    /// </summary>
    private void OnCollisionEnter(Collision other)
    {
        if (RoomManager.Instance.CheckCrimeRoom()) return;
        if (roomType == E_RoomType.boss)
        {
            if (!lorded && other.transform.tag == "Player")
            {
                // Debug.Log("没有通关所有大罪关");
                Vector3 dir = GameManager.Instance.currentPlayer.transform.position - other.GetContact(0).point;
                dir.y = 0;
                GameManager.Instance.currentPlayer.rb.AddForce(dir.normalized * 1000, ForceMode.Impulse);
                PanelManager.Instance.Open(new BottomTips(), null, "你的罪恶尚未洗清....【还有未通关的大罪关卡】");
            }
        }
    }

    protected void Start()
    {
        // if (loadPolicy.Count == 1) return;
        /// 计算生成策略
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

    /// <summary>
    /// 重置房间信息
    /// </summary>
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
            clearCheck = null;  //清空事件监听
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

    /// <summary>
    /// 小怪死亡，同时判断是否通关
    /// </summary>
    virtual public void EnemyDie()
    {
        enemyCount--;
        // Debug.Log("小怪死亡");
        if (enemyCount == 0)
        {
            Debug.Log("小怪全部死亡");
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
    /// 机关全部被破坏，同时判断小怪是否全部死亡
    /// </summary>
    public void TrapClear()
    {
        Debug.Log("机关全部破坏");
        if (enemyCount == 0)
        {
            Clear();
        }
    }

    /// <summary>
    /// 玩家击败所有小怪和全部机关后逻辑
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
        Invoke("ResetTime", 2f);  //延迟1秒调用重置时间
        OpenWallCollider();
    }

    /// <summary>
    /// 缓速播放结束 显示通关UI
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
    /// 显示小怪通关UI
    /// </summary>
    private void ShowEnemyRoomClearUI()
    {
        PanelManager.Instance.Open(new SelectPanel(), null, "进阶");
    }

    /// <summary>
    /// 显示大罪通关UI
    /// </summary>
    private void ShowCrimeRoomClearUI()
    {
        BuffDataManager.Instance.RecordBuffList();
        PanelManager.Instance.Open(new SelectPanel(), null, "舍弃");
    }

    /// <summary>
    /// 显示Boss通关特效
    /// </summary>
    private void ShowBossRoomClear()
    {
        clearTrigger.SetActive(true);
    }
}
