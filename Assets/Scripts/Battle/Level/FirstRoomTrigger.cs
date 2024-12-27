using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstRoomTrigger : RoomTrigger
{
    public Transform bossResurrectionPoint;    //Boss出生点
    private void Awake()
    {
        enemys = transform.Find("Enemys");
        openCollider = transform.Find("AirWallOpen").gameObject;
        closeCollider = transform.Find("AirWallClose").gameObject;
        enemyCount = enemys.childCount;
    }

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
    /// 玩家离开初始房间 清空玩家Buff
    /// </summary>
    /// <param name="other"></param>
    protected void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            (PanelManager.Instance.GetPanel("BattleMainPanel") as BattleMainPanel).FadeChange(false);
            // Debug.Log("玩家离开");
            //先恢复Buff为1级
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

            //记录当前Buff
            BuffDataManager.Instance.RecordBuffList();

            //调用Buff选择面板
            PanelManager.Instance.Open(new SelectPanel(), null, "舍弃");
            GetComponent<Collider>().enabled = false;
        }
    }

    override public void EnemyDie()
    {
        enemyCount--;
        // Debug.Log("小怪死亡");
        if (!cleared && enemyCount == 0)
        {
            cleared = true;
            // Debug.Log("小怪已清除，准备生成Boss");
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
}
