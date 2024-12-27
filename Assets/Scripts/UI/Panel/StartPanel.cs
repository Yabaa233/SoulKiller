using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ???????
/// xushi
/// </summary>
public class StartPanel : BasePanel
{
    static readonly string path = "UI/Panel/StartPanel";

    public StartPanel() : base(new UIType(path)) { }

    //??????
    Button start;
    //????
    Button setting;
    //???
    Button quit;


    public override void OnInit()
    {
        base.OnInit();
        //PanelManager.Instance.Open(new GM());
    }
    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        start = UITool.GetOrAddComponentInChildren<Button>("Btn_Start");
        setting = UITool.GetOrAddComponentInChildren<Button>("Btn_Setting");
        quit = UITool.GetOrAddComponentInChildren<Button>("Btn_Quit");

        //????
        start.onClick.AddListener(GameStart);
        setting.onClick.AddListener(Setting);
        quit.onClick.AddListener(QuitGame);

        //start.gameObject.AddComponent<ButtonHighLight>();
        //setting.gameObject.AddComponent<ButtonHighLight>();
        //quit.gameObject.AddComponent<ButtonHighLight>();

        //��ʼ��
        //start.Select();
        PanelManager.Instance.SetSkipButton(false);
    }
    //??????
    public void GameStart()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/click");
        PanelManager.Instance.SetSkipButton(true);
        Close();
        SceneLoadManager.Instance.LoadScene(1);
    }
    //?????y???
    public void Setting()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/click");
        //Close();
        PanelManager.Instance.Open(new SettingPanel());
    }
    //??????
    public void QuitGame()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/click");
        PanelManager.Instance.SetSkipButton(true);
        Close();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


}

