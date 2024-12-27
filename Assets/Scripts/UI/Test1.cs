using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI测试
/// xushi
/// </summary>
public class Test1 : singleton<Test1>
{
    public GameObject player;//当前场景的角色
    public PlayerControl playerC;

    protected override void Awake()
    {
        base.Awake();
        player = GameObject.Find("Player");
    }

    private void Start()
    {
        InitPlayerBuff();
        PanelManager.Instance.Open(new BattleMainPanel());
        //PanelManager.Instance.gameObject.SetActive(true) ;
    }


    public void InitPlayerBuff()
    {
        //新建一些角色当前的buff用来展示
        Damage damageBuff1 = new Damage(E_ChararcterType.player, 1);
        SpeedBuff speedBuff1 = new SpeedBuff(E_ChararcterType.player, 3);
        HpUp hpBuff1 = new HpUp(E_ChararcterType.player, 2);
        ShieldBuff shieldBuff1 = new ShieldBuff(E_ChararcterType.player, 1);
        StaffBuff staffBuff1 = new StaffBuff(E_ChararcterType.player, 1);
        GunBuff gunBuff1 = new GunBuff(E_ChararcterType.player, 2);
        SwordBuff swordBuff1 = new SwordBuff(E_ChararcterType.player,1);

        //给所有buff加载到当前角色上
        playerC = player.GetComponent<PlayerControl>();

        playerC.characterBuffManager.AddBuff(damageBuff1, player);
        playerC.characterBuffManager.AddBuff(speedBuff1, player);
        playerC.characterBuffManager.AddBuff(hpBuff1, player);
        playerC.characterBuffManager.AddBuff(shieldBuff1 , player); 
        playerC.characterBuffManager.AddBuff(staffBuff1, player);
        playerC.characterBuffManager.AddBuff(gunBuff1 , player);
        playerC.characterBuffManager.AddBuff(swordBuff1 , player);


        playerC.characterBuffManager.RefreshData();

        //Transform enemys = transform.Find("Enemys");
        //RoomManager.Instance.LoadBoss(enemys);
    }

}
