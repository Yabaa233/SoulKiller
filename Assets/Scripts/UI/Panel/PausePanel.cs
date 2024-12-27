using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ��ͣ���棬��Ϸ�ڵ����ͣ
/// </summary>
public class PausePanel : BasePanel
{
    static readonly string path = "UI/Panel/PausePanel";

    public PausePanel() : base(new UIType(path)) { }

    //�������˵�
    Button menu;
    //����-����
    Button control;
    //����-����
    Button sound;

    //����������
    Button buffList;
    //���ݵ���һ��ѡ��
    Button revert;
    //����
    Button back;

    Area_MainBuff area_MainBuff;

    //�Ƿ��ǵ�һ�δ򿪽���
    bool isFirst=true;

    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        menu = UITool.GetOrAddComponentInChildren<Button>("Btn_Menu");
        control = UITool.GetOrAddComponentInChildren<Button>("Btn_Control");
        sound = UITool.GetOrAddComponentInChildren<Button>("Btn_Sound");

        buffList = UITool.GetOrAddComponentInChildren<Button>("Btn_BuffInfo");
        revert = UITool.GetOrAddComponentInChildren<Button>("Btn_Revert");
        back = UITool.GetOrAddComponentInChildren<Button>("Btn_Back");

        //������ť
        menu.onClick.AddListener(ToMenu);
        control.onClick.AddListener(OpenSetting_Control);
        sound.onClick.AddListener(OpenSetting_Sound);
        buffList.onClick.AddListener(OpenBuffList);
        revert.onClick.AddListener(RevertToLast);
        back.onClick.AddListener(Close);
        
        MonoHelper.Instance.StartCoroutine(ResetAction());

        Time.timeScale = 0f;

        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/panelOpen");

        //stop timeline
        TimelineManager.Instance.PauseCurrentPlayableDirector();
    }

    IEnumerator ResetAction()
    {
        float firstTime = Time.realtimeSinceStartup;
        while(Time.realtimeSinceStartup - firstTime < 0.2f)
        {
            yield return null;
        }
       PanelManager.Instance.KeyBoardUpdateAction += EscClose;
       yield break;
    }

    public override void OnClose()
    {
        base.OnClose();
        
        Time.timeScale = 1f;

        PanelManager.Instance.KeyBoardUpdateAction -= EscClose;

        area_MainBuff = PanelManager.Instance.GetPanel("Area_MainBuff") as Area_MainBuff;
        //area_MainBuff.isOpenPause = false;

        coroutines.Add( MonoHelper.Instance.StartCoroutine(ReListen()));
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/panelClose");

        TimelineManager.Instance.ResumeCurrentPlayableDirector();
    }
    
    //area���¼�������
    IEnumerator ReListen()
    {
        float firstTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - firstTime < 0.1f)
        {
            yield return null;
        }
        PanelManager.Instance.KeyBoardUpdateAction += area_MainBuff.OpenBuffInfo;
    }

    //�������˵�
    public void ToMenu()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/click");
        Close();
        PanelManager.Instance.Close(PanelManager.Instance.GetPanel("BattleMainPanel").UIType);
        SceneLoadManager.Instance.LoadMainScene(0);
    }
    //������-����ҳ
    public void OpenSetting_Control()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/click");
        Close();
        PanelManager.Instance.Open(new SettingPanel(),null,1);
    }
    //������-����ҳ
    public void OpenSetting_Sound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/click");
        Close();
        PanelManager.Instance.Open(new SettingPanel(), null, 2);
    }

    //�򿪵�ǰ��������
    public void OpenBuffList()
    {
        //Close();
        PanelManager.Instance.KeyBoardUpdateAction -= EscClose;

        PanelManager.Instance.Open(new BuffInfoPanel());
        PanelManager.Instance.KeyBoardUpdateAction -= area_MainBuff.OpenBuffInfo;

    }
    //���ݵ���һ����������
    public void RevertToLast()
    {

    }

    //ESC�ر����
    public void EscClose(KeyCode keyCode)
    {
        if (keyCode == KeyCode.Escape&&!isFirst)
        {
            Close();
        }
        isFirst = false;
    }

}
