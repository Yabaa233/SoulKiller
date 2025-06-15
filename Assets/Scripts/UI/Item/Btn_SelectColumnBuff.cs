using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
///<summary>

////// 小さなバフアイコンを表示する

///</summary>
public class Btn_SelectColumnBuff : BasePanel
{
    static readonly string path = "UI/Item/Btn_SelectColumnBuff";

    public Btn_SelectColumnBuff() : base(new UIType(path)) { }

    //バフ画像
    Image image;
    //現在のバフタイプ名
    //E_BuffKind buffKind;
    //現在のバフ情報
    BuffItemData itemData;
    //バフのレベル
    int buffLevel;
    //バフのレベル表示
    GameObject buffLevelList;
    //バフのレベルリスト
    GameObject buffLevelListContent;

    //紹介パネルを開くためのコルーチンをホバーします
    Coroutine openBuffInfo;
    //ホバーで紹介パネルを開くダークプレス
    GameObject leftFocusGO;

    //敵かどうか
    bool isEnemy = false;

    Material material;

    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        image = UITool.GetOrAddComponentInChildren<Image>("Image");

        buffLevelList = UITool.FindChildGameObject("BuffChild(Scroll View");
        buffLevelListContent = UITool.FindChildGameObject("Content");

        UITool.addTriggersListener(UnityEngine.EventSystems.EventTriggerType.PointerEnter, PointerEnter);
        UITool.addTriggersListener(UnityEngine.EventSystems.EventTriggerType.PointerExit, PointerExit);

        material = image.material;

        material = Object.Instantiate(material);

        //ハイライト表示するかどうかを渡します
        if (para.Length != 0)
        {
            if (para.Length == 1)
            {
                image.color = new Color(1, 1, 1, (float)para[0]);
            }
            else
            {
                itemData = (BuffItemData)para[3];
                image.color = new Color(1, 1, 1, (float)para[0]);
                buffLevel = (int)para[1];
                leftFocusGO = (GameObject)para[2];
            }


        }
        if((float)para[0] == 0.4f)
            isEnemy= true;

        material.SetFloat("_TintColorIntensity", 2*(float)para[0]);
        image.material = material;

        image.sprite = itemData.buffSprite;

        buffLevelList.SetActive(false);
        InitBuffLevelIcon();
    }

    public override void OnClose()
    {
        base.OnClose();
        foreach (var item in PanelManager.Instance.GetAllPanel("Btn_Buff"))
        {
            item.Close();
        }
    }

    //現在のバフのアップグレードをロードします
    public void InitBuffLevelIcon()
    {
        if (isEnemy)
            return;
        for (int i = 0; i < buffLevel; i++)
        {
            PanelManager.Instance.Open(new Btn_LevelColumn(), buffLevelListContent.transform,itemData.buffLevelDatas[i]);
        }
    }

    public override void PointerEnter(BaseEventData data)
    {
        if (isEnemy)
            return;

        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/smallItemSelect");

        base.PointerEnter(data);

        material.SetFloat("_TintColorIntensity", 2.5f);
        image.material = material;

        openBuffInfo = MonoHelper.Instance.StartCoroutine(IE_OpenLevelUp());
        coroutines.Add(openBuffInfo);
    }
    public override void PointerExit(BaseEventData data)
    {
        if (isEnemy)
            return;
        base.PointerExit(data);
        material.SetFloat("_TintColorIntensity", 1f);
        if (openBuffInfo != null)
            MonoHelper.Instance.StopCoroutine(openBuffInfo);
        CloseLevelUp();
    }

    IEnumerator IE_OpenLevelUp()
    {
        leftFocusGO.SetActive(true);
        float firstTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - firstTime < 0.5f)
        {
            yield return null;
        }
        OpenLevelUp();
    }

    //現在のバフのアップグレードを表示します
    public void OpenLevelUp()
    {
        buffLevelList.SetActive(true);
        buffLevelList.transform.SetParent(UITool.GetUI().transform.parent.parent.parent);
    }
    //現在のバフのアップグレードを閉じる
    public void CloseLevelUp()
    {
        leftFocusGO.SetActive(false);
        buffLevelList.SetActive(false);
        buffLevelList.transform.SetParent(UITool.GetUI().transform);
    }
}
