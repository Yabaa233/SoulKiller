using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstRoomTrigger : RoomTrigger
{
    public Transform bossResurrectionPoint;    //ボスの出現地点
    private void Awake()
    {
        enemys = transform.Find("Enemys");
        openCollider = transform.Find("AirWallOpen").gameObject;
        closeCollider = transform.Find("AirWallClose").gameObject;
        enemyCount = enemys.childCount;
    }

    ///<summary>


    ////// 最初の部屋のトリガー


    ///</summary>
    override protected void OnTriggerEnter(Collider other)
    {
        if (!lorded && other.tag == "Player")
        {
            // (PanelManager.Instance.GetPanel("BattleMainPanel") as BattleMainPanel).FadeChange(true);
            lorded = true;
            RoomManager.Instance.LoadEnemy(enemys, loadPolicy);
            CloseWallCollider();
        }
    }

    /// <summary>
    /// トリガーの初期化
    /// </summary>
    /// <param name="other"></param>
    protected void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            (PanelManager.Instance.GetPanel("BattleMainPanel") as BattleMainPanel).FadeChange(false);
            // Debug.Log("The player has left.");
            //まず、Buffをレベル1に回復してください。
            BuffDataManager.Instance.playerBuffList.Clear();
            BuffDataManager.Instance.playerBuffList.Add(new SwordBuff(E_ChararcterType.player, 1));
            BuffDataManager.Instance.playerBuffList.Add(new GunBuff(E_ChararcterType.player, 1));
            BuffDataManager.Instance.playerBuffList.Add(new StaffBuff(E_ChararcterType.player, 1));
            BuffDataManager.Instance.playerBuffList.Add(new Damage(E_ChararcterType.player, 1));
            BuffDataManager.Instance.playerBuffList.Add(new HpUp(E_ChararcterType.player, 1));
            BuffDataManager.Instance.playerBuffList.Add(new ShieldBuff(E_ChararcterType.player, 1));
            BuffDataManager.Instance.playerBuffList.Add(new SpeedBuff(E_ChararcterType.player, 1));

            other.gameObject.GetComponent<PlayerControl>().PlayerBuffRebuild(BuffDataManager.Instance.playerBuffList);
            other.gameObject.GetComponent<PlayerControl>().lockHealth = false;
            GameManager.Instance.currentPlayer.characterData.currentHealth = GameManager.Instance.currentPlayer.characterData.maxHealth;

            //現在のBuffを記録する
            BuffDataManager.Instance.RecordBuffList();

            //Buff選択パネルを呼び出す
            PanelManager.Instance.Open(new SelectPanel(), null, "Abandon");
            GetComponent<Collider>().enabled = false;
        }
    }

    /// <summary>
    /// トリガーの更新
    /// </summary>
    override public void EnemyDie()
    {
        enemyCount--;
        // Debug.Log("The monster has died.");
        if (!cleared && enemyCount == 0)
        {
            cleared = true;
            // Debug.Log("The monsters have already been cleared, and the boss is being prepared to spawn.");
            GameObject newBoss = Instantiate(RoomManager.Instance.bossPrefab, bossResurrectionPoint.position,
                RoomManager.Instance.bossPrefab.transform.rotation, enemys);
            BossControl bossControl = newBoss.GetComponent<BossControl>();
            bossControl.lockHealth = true;

            bossControl.characterBuffManager.AddBuff(new SwordBuff(E_ChararcterType.boss, 4), newBoss);
            bossControl.characterBuffManager.AddBuff(new GunBuff(E_ChararcterType.boss, 4), newBoss);
            bossControl.characterBuffManager.AddBuff(new StaffBuff(E_ChararcterType.boss, 4), newBoss);
            bossControl.characterBuffManager.AddBuff(new Damage(E_ChararcterType.boss, 4), newBoss);
            bossControl.characterBuffManager.AddBuff(new HpUp(E_ChararcterType.boss, 4), newBoss);
            bossControl.characterBuffManager.AddBuff(new ShieldBuff(E_ChararcterType.boss, 4), newBoss);
            bossControl.characterBuffManager.AddBuff(new SpeedBuff(E_ChararcterType.boss, 4), newBoss);

            TimelineManager.Instance.changePlayableTO(1);
            TimelineManager.Instance.PlayCurrentPlayableDirector();
        }
    }

    /// <summary>
    /// トリガーの終了
    /// </summary>
}
