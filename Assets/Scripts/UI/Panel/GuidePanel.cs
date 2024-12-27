using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ���ְ�������
/// xushi
/// </summary>
public class GuidePanel : BasePanel
{
    static readonly string path = "UI/Panel/GuidePanel";
    public GuidePanel() : base(new UIType(path)){}

    GameObject move,dush,attack1,attack2,attack3,attack4,function1,function2;
    Animator anim;
    Coroutine closePanel;

    public override void OnInit()
    {
        base.OnInit();
    }

    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        //move = UITool.FindChildGameObject("Move");
        //dush = UITool.FindChildGameObject("Dush");
        //attack1 = UITool.FindChildGameObject("Attack1");
        //attack2 = UITool.FindChildGameObject("Attack2");
        //attack3 = UITool.FindChildGameObject("Attack3");
        //attack4 = UITool.FindChildGameObject("Attack4");
        //function1 = UITool.FindChildGameObject("Function");
        //function2 = UITool.FindChildGameObject("Function2");
        anim=UITool.GetOrAddComponent<Animator>();

        //move.SetActive(true);
        //dush.SetActive(false); attack1.SetActive(false); attack2.SetActive(false);
        //attack3.SetActive(false); attack4.SetActive(false); function1.SetActive(false); function2.SetActive(false);

        PanelManager.Instance.KeyBoardUpdateAction += PanelSwitch;

        closePanel = MonoHelper.Instance.StartCoroutine(IE_Close());
        coroutines.Add(closePanel);
    }


    public override void OnClose()
    {
        base.OnClose();
        PanelManager.Instance.KeyBoardUpdateAction -= PanelSwitch;
    }


    //������ⷽ��
    public void PanelSwitch(KeyCode keyCode)
    {
        
        switch (keyCode)
        {
            case KeyCode.A:
                //dush.SetActive(true);
                anim.SetBool("1",true);
                break;
            case KeyCode.W:
                //dush.SetActive(true);
                anim.SetBool("1", true);
                break;
            case KeyCode.S:
                //dush.SetActive(true);
                anim.SetBool("1", true);
                break;
            case KeyCode.D:
                //dush.SetActive(true);
                anim.SetBool("1", true);
                break;
            case KeyCode.Space:
                //attack1.SetActive(true);
                anim.SetBool("2", true);
                break;
            case KeyCode.Mouse0:
                //attack2.SetActive(true);
                anim.SetBool("3", true);
                break;
            case KeyCode.Mouse1:
                //attack3.SetActive(true);
                anim.SetBool("4", true);
                break;
            case KeyCode.Q:
                //attack4.SetActive(true);
                anim.SetBool("5", true);
                break;
            case KeyCode.E:
                //function1.SetActive(true);
                //function2.SetActive(true);
                anim.SetBool("6", true);

                break;

        }

    }
    IEnumerator IE_Close()
    {
        float firstTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - firstTime < 30f)
        {
            yield return null;
        }
        Close();
    }
}
