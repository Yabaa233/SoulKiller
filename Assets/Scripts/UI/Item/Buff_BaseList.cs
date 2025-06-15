using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

///<summary>


////// 属性や武器に関連するバフ、現在のバフとバフのアップグレード状況を表示します。


///</summary>
public class Buff_BaseList : BasePanel
{
    //static readonly string path = "";

    public Buff_BaseList(UIType uIType) : base(uIType) { }

    //現在のBuffオブジェクト
    GameObject buff;
    //現在のBuff画像
    Image buffImage;
    //現在のバフアップグレード状況リスト
    GameObject buffLevelList;
    //アイコンの詳細情報
    GameObject buffInfo;
    //アイコンの詳細情報テキスト
    Text buffInfoT;

    //渡された情報
    BuffItemData itemData;
    int buffLevel;

    //現在、詳細を表示することは可能ですか？
    bool canShow=true;

    //紹介パネルを開くためのコルーチンをホバーします
    Coroutine openBuffInfo;

    Material material;

    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        buff = UITool.FindChildGameObject("Btn_Weapon");
        buffImage = buff.GetComponent<Image>();
        buffLevelList = UITool.FindChildGameObject("BuffChild(Scroll View").transform.Find("Viewport").Find("Content").gameObject;
        buffInfo = UITool.FindChildGameObject("Img_Info");
        buffInfoT = buffInfo.GetComponentInChildren<Text>();

        //マウスオーバーアニメーションを監視する
        UITool.addTriggersListener(EventTriggerType.PointerEnter, PointerEnter, buff);
        UITool.addTriggersListener(EventTriggerType.PointerExit, PointerExit, buff);

        buffInfo.SetActive(false);

        //現在のbuffの情報を初期化する
        itemData = (BuffItemData)para[0];
        buffLevel = (int)para[1];

        buffImage.sprite = itemData.buffSprite;

        //buffImage.material = new Material("Slash/Blended");
        material=buffImage.material;

        material= Object.Instantiate(material);

        //アップグレードアイコンの初期化
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

    //canShowを変更する
    public void SwitchShow(bool a)
    {
        canShow = a;
    }

    //現在のバフのアップグレードをロードします
    public void InitBuffLevelIcon()
    {

        //読み込み
        for (int i = 0; i < buffLevel; i++)
        {
            PanelManager.Instance.Open(new Btn_Buff(), buffLevelList.transform, buffInfo,itemData.buffLevelDatas[i] ,UITool.GetUI());
        }
        
    }
    //マウスホバーアイコンの詳細情報を表示する
    public override void PointerEnter(BaseEventData data)
    {
        material.SetFloat("_TintColorIntensity", 2.5f);
        buffImage.material = material; 

        if (canShow)
        {
            openBuffInfo= MonoHelper.Instance.StartCoroutine(IE_OpenInfo());
            coroutines.Add(openBuffInfo);
        }

        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/buffItemSelect");
    }
    //マウスホバーアイコンの詳細情報を閉じる
    public override void PointerExit(BaseEventData data)
    {

        material.SetFloat("_TintColorIntensity", 1f);
        buffImage.material = material;
        if (openBuffInfo != null)
            MonoHelper.Instance.StopCoroutine(openBuffInfo);
        CloseInfo();
    }

    IEnumerator IE_OpenInfo()
    {
        float firstTime = Time.realtimeSinceStartup;
        while(Time.realtimeSinceStartup - firstTime < 0.5f)
        {
            yield return null;
        }
        OpenInfo();

    }
    //マウスオーバーアイコンの詳細情報を開く
    public void OpenInfo()
    {

        buffInfoT.text = itemData.buffDescribe;
        //詳細情報を表示する
        buffInfo.transform.SetParent(buffInfo.transform.parent.parent.parent.parent);
        buffInfo.SetActive(true);

    }
    //マウスホバーアイコンの詳細情報を閉じる
    public  void CloseInfo()
    {
        buffInfo.transform.SetParent(UITool.GetUI().transform);
        buffInfo.SetActive(false);
    }

}
