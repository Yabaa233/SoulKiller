using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FullPanel : BasePanel
{
    static readonly string path = "UI/Panel/FullPanel";

    public FullPanel() : base(new UIType(path)) { }

    private Text fullText;
    private Text teamText;
    private Text thanksText;
    private Text gameTime;
    private Button returnButton;
    private float timer = 0;
    private float speed = 55f;

    private bool move;

    public override void OnInit()
    {
        base.OnInit();
        ifNeedUpdate = true;
    }

    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        fullText = UITool.GetOrAddComponentInChildren<Text>("FullText");
        teamText = UITool.GetOrAddComponentInChildren<Text>("TeamText");
        thanksText = UITool.GetOrAddComponentInChildren<Text>("ThanksText");
        gameTime = UITool.GetOrAddComponentInChildren<Text>("GameTime");
        returnButton = UITool.GetOrAddComponentInChildren<Button>("ReturnButton");

        //添加事件监听
        returnButton.onClick.AddListener(Skip);
        move = true;
        
        //播放Doteen
        //fullText.DOText(fullText.text.ToString(),2f);
    }

    public override void Update()
    {
        base.Update();
        // timer += Time.deltaTime;
        // if(timer > 4f && move)
        // {
        //     fullText.transform.localPosition += new Vector3(0f,speed * Time.deltaTime,0f);
        //     teamText.transform.localPosition += new Vector3(0f,speed * Time.deltaTime,0f);
        //     thanksText.transform.localPosition += new Vector3(0f,speed * Time.deltaTime,0f);
        // }

        // if(thanksText.transform.localPosition.y > -374f)
        // {
        //     move = false;
        // }
    }


    /// <summary>
    /// 跳转回主场景的方法,记得调用
    /// </summary>
    public void Skip()
    {
        SceneLoadManager.Instance.LoadMainScene(0);
        Close();
    }



}
