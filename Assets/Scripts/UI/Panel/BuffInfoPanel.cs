using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 角色当前的 Buff（能力） 面板
/// xushi
/// </summary>
public class BuffInfoPanel : BasePanel
{
    static readonly string path = "UI/Panel/BuffInfoPanel";

    public BuffInfoPanel() : base(new UIType(path)) { }

    //左侧属性buff列表
    GameObject propertyList;
    //右侧武器buff列表
    GameObject weaponList;
    //返回按钮
    Button back;

    //是否是第一次打开界面
    bool isFirst = true;

    Area_MainBuff area_MainBuff;

    //音效协程
    Coroutine soundCoro;

    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        propertyList = UITool.FindChildGameObject("PropertyBuff(Scroll View").transform.Find("Viewport").Find("Content").gameObject;
        weaponList = UITool.FindChildGameObject("WeaponBuff(Scroll View").transform.Find("Viewport").Find("Content").gameObject;
        back = UITool.GetOrAddComponentInChildren<Button>("Btn_Back");
        
        back.onClick.AddListener(Close);

        InitPropertyBuff();
        InitWeaponBuff();

        Time.timeScale = 0f;

        PanelManager.Instance.KeyBoardUpdateAction += EscClose;

        soundCoro = MonoHelper.Instance.StartCoroutine(openSound());
        coroutines.Add(soundCoro);
    }

    IEnumerator openSound()
    {

        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/buffInfoOpen");

        float firstTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - firstTime < 0.4f)
        {
            yield return null;
        }

        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/panelOpen");
    }

    public override void OnClose()
    {
        base.OnClose();
        Time.timeScale=1.0f;
        //销毁之前的ui
        foreach (var item in PanelManager.Instance.GetAllPanel("Buff_Property"))
        {
            item.Close();
        }
        foreach (var item in PanelManager.Instance.GetAllPanel("Buff_Weapon"))
        {
            item.Close();
        }

        PanelManager.Instance.KeyBoardUpdateAction -= EscClose;

        area_MainBuff = PanelManager.Instance.GetPanel("Area_MainBuff") as Area_MainBuff;

        if (PanelManager.Instance.GetPanel("PausePanel") != null)
        {
            //在打开暂停的情况下打开能力面板
            Time.timeScale = 0f;
            coroutines.Add(MonoHelper.Instance.StartCoroutine(ReListenPause()));
        }
        else
        {
            coroutines.Add(MonoHelper.Instance.StartCoroutine(ReListenMain()));
        }
            
        


        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/panelClose");
    }


    IEnumerator ReListenMain()
    {
        float firstTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - firstTime < 0.1f)
        {
            yield return null;
        }
        PanelManager.Instance.KeyBoardUpdateAction += area_MainBuff.OpenBuffInfo;
    }

    IEnumerator ReListenPause()
    {
        float firstTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - firstTime < 0.1f)
        {
            yield return null;
        }
        PanelManager.Instance.KeyBoardUpdateAction += (PanelManager.Instance.GetPanel("PausePanel") as PausePanel).EscClose;
    }


    //左侧填充item（测试）
    public void InitPropertyBuff()
    {
        //销毁之前的ui
        foreach (var item in PanelManager.Instance.GetAllPanel("Buff_Property"))
        {
            item.Close();
        }

        //生成
        foreach (var item in BuffDataManager.Instance.playerCurrentBuff)
        {
            E_BuffKind buffKind=item.buffKind;
            if (buffKind == E_BuffKind.HpUp || buffKind == E_BuffKind.Damage || buffKind == E_BuffKind.SpeedBuff || buffKind == E_BuffKind.ShieldBuff)
            {
                foreach (var buffItem in PanelManager.Instance.buffInfoListSO.buffItems)
                {
                    if (buffItem.buffKind == buffKind)
                        PanelManager.Instance.Open(new Buff_Property(), propertyList.transform, buffItem,item.level);
                }
            }
        }
        //Debug.Log("init");
    }
    //右侧填充item（测试）
    public void InitWeaponBuff()
    {
        //销毁之前的ui
        foreach (var item in PanelManager.Instance.GetAllPanel("Buff_Weapon"))
        {
            item.Close();
        }
        foreach (var item in BuffDataManager.Instance.playerCurrentBuff)
        {
            E_BuffKind buffKind = item.buffKind;
            if (buffKind == E_BuffKind.SwordBuff || buffKind == E_BuffKind.GunBuff || buffKind == E_BuffKind.StaffBuff)
            {
                foreach (var buffItem in PanelManager.Instance.buffInfoListSO.buffItems)
                {
                    if(buffItem.buffKind==buffKind)
                        PanelManager.Instance.Open(new Buff_Weapon(), weaponList.transform, buffItem,item.level);
                }
            }
                
        }
    }

    //ESC关闭面板
    public void EscClose(KeyCode keyCode)
    {
        if ((keyCode == KeyCode.Escape||keyCode==KeyCode.B)&&!isFirst)
        {
            Close();
        }
        isFirst = false;
    }
}
