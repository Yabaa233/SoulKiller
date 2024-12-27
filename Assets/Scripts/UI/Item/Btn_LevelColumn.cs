using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 在角色buff展示面板，显示buff的升级情况
/// xushi
/// </summary>
public class Btn_LevelColumn : BasePanel
{
    static readonly string path = "UI/Item/Btn_LevelColumn";

    public Btn_LevelColumn() : base(new UIType(path)) { }

    //buff图片
    Image img;
    //buff描述
    Text info;

    //当前buff等级信息
    BuffLevelData levelData;


    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        img = UITool.GetOrAddComponentInChildren<Image>("Image");
        info = UITool.GetOrAddComponentInChildren<Text>("Text");

        //传递父ui的显示组件

        levelData = (BuffLevelData)para[0];

        img.sprite = levelData.levelSprite;
        info.text = levelData.levelDescribe;
    }


}
