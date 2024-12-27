using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ��Ϸ�������ʱ�Ľ���
/// xushi
/// </summary>
public class LaunchPanel : BasePanel
{
    static readonly string path = "UI/Panel/LaunchPanel";
    public LaunchPanel() : base(new UIType(path)) { }

    Button start;

    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        start = UITool.GetOrAddComponentInChildren<Button>("Btn_Start");

        start.onClick.AddListener(StartBegin);

        //start.gameObject.SetActive(false);
    }

    //�򿪿�ʼ����
    public void StartBegin()
    {
        Debug.Log("点击了一次");
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/click");
        Close();
        PanelManager.Instance.Open(new StartPanel());
    }
}
