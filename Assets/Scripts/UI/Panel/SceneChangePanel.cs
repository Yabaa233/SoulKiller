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
        ifNeedUpdate = true;//需要Update方法来更新
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
        if(asyncPercent>89f)//因为传值最大就是90
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

        loadText.text = "加载中..." + currentPercent.ToString() + "%";

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

    /// <summary>
    /// 设置目标百分比
    /// </summary>
    /// <param name="asyncPercent"></param>
    public void SetPercent(float _asyncPercent)
    {
        asyncPercent = _asyncPercent;
    }


    /// <summary>
    /// 返回是否可以跳转了
    /// </summary>
    /// <returns></returns>
    public bool IFCanJump()
    {
        return ifCanJump;
    }

}
