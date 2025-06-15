using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// キャラクターのバフ表示パネルで、バフのアップグレード状況を表示します。
/// xushi
/// </summary>
public class Btn_LevelColumn : BasePanel
{
    static readonly string path = "UI/Item/Btn_LevelColumn";

    public Btn_LevelColumn() : base(new UIType(path)) { }

    //バフ画像
    Image img;
    //バフの説明
    Text info;

    //現在のバフレベル情報
    BuffLevelData levelData;


    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        img = UITool.GetOrAddComponentInChildren<Image>("Image");
        info = UITool.GetOrAddComponentInChildren<Text>("Text");

        //親UIの表示コンポーネントを伝達します

        levelData = (BuffLevelData)para[0];

        img.sprite = levelData.levelSprite;
        info.text = levelData.levelDescribe;
    }


}
