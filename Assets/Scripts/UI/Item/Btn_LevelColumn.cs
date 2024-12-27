using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// �ڽ�ɫbuffչʾ��壬��ʾbuff���������
/// xushi
/// </summary>
public class Btn_LevelColumn : BasePanel
{
    static readonly string path = "UI/Item/Btn_LevelColumn";

    public Btn_LevelColumn() : base(new UIType(path)) { }

    //buffͼƬ
    Image img;
    //buff����
    Text info;

    //��ǰbuff�ȼ���Ϣ
    BuffLevelData levelData;


    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        img = UITool.GetOrAddComponentInChildren<Image>("Image");
        info = UITool.GetOrAddComponentInChildren<Text>("Text");

        //���ݸ�ui����ʾ���

        levelData = (BuffLevelData)para[0];

        img.sprite = levelData.levelSprite;
        info.text = levelData.levelDescribe;
    }


}
