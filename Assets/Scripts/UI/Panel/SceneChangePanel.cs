using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneChangePanel : BasePanel
{
    static readonly string path = "UI/Panel/SceneChangePanel";
    public SceneChangePanel() : base(new UIType(path)){}

    public Slider loadSlider;
    public Text loadText;
    public Image mainImage;
    public Text mainText;


    private float asyncPercent = 0;
    private float currentPercent = 0;
    private bool ifCanJump;

    private float time = 0f;

    public override void OnInit()
    {
        base.OnInit();
        ifNeedUpdate = true;//更新するためのUpdateメソッドが必要です。
    }

    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        loadSlider = UITool.GetOrAddComponentInChildren<Slider>("LoadSlider");
        loadText = UITool.GetOrAddComponentInChildren<Text>("LoadText");
        mainImage = UITool.GetOrAddComponentInChildren<Image>("MainImage");
        mainText = UITool.GetOrAddComponentInChildren<Text>("MainText");

        loadSlider.maxValue = 100;
        loadSlider.minValue = 0;
        ifCanJump = false;

        int count = PanelManager.Instance.buffInfoListSO.buffItems.Count;
        int randomIndex = Random.Range(0,count);
        var buffItem = PanelManager.Instance.buffInfoListSO.buffItems[randomIndex];
        mainImage.sprite = buffItem.buffSpriteText;
        mainText.text = buffItem.buffStory;
    }

    public override void Update()
    {
        base.Update();
        if(asyncPercent>89f)//値を渡す最大値は90です。
        {
            asyncPercent = 100f;
        }

        if(currentPercent < asyncPercent)
        {
            currentPercent += 1f;
            loadSlider.value = currentPercent;
        }
        else
        {
            currentPercent = asyncPercent;
        }

        loadText.text = "読み込み中..." + currentPercent.ToString() + "%";

        if(currentPercent >= 99f)
        {
            if(time < 1.0f)
            {
                time += Time.deltaTime;
            }
            else
            {
                ifCanJump = true;
            }
        }
    }

    public override void OnClose()
    {
        
    }

    ///<summary>


    ////// 目標パーセンテージを設定する


    ///</summary>
    /// <param name="asyncPercent"></param>
    public void SetPercent(float _asyncPercent)
    {
        asyncPercent = _asyncPercent;
    }


    /// <summary>
    /// ジャンプできるかどうかを返します。
    /// </summary>
    /// <returns></returns>
    public bool IFCanJump()
    {
        return ifCanJump;
    }

}
