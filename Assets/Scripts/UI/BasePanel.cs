using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// すべてのインターフェースの基底クラスで、インターフェース情報と共通メソッドを保存します。
/// xushi
/// </summary>
public class BasePanel
{
    //UI基本情報
    public UIType UIType { get; private set; }

    //UIツール
    public UITool UITool { get; private set; }

    public BasePanel(UIType uIType)
    {
        UIType = uIType;
    }

    //UIが起動したすべてのコルーチン情報
    protected List<Coroutine> coroutines;

    //UIは更新が必要ですか？
    public bool ifNeedUpdate = false;

    //初期化読み込み
    public void Init(UITool tool){
        //肌
        // GameObject skinPrefab = ResManager.LoadPrefab(skinPath); //リソースロードモジュールは後で補充する必要があります

        //現在のUIのUIツール（現在のGameObject情報を含む）
        UITool = tool;
        coroutines = new List<Coroutine>();
    }
    //閉じる
    public virtual void Close()
    {
        PanelManager.Instance.Close(UIType);
    }

    //一時停止時に
    public virtual void OnPause()
    {

    }

    //初期化時に
    public virtual void OnInit(){

    }

    //表示するとき
    public virtual void OnShow(params object[] para){
        
    }
    //閉じる時に
    public virtual void OnClose(){
        //すべてのコルーチンを閉じる
        if (coroutines != null)
            foreach (var item in coroutines)
            {
                MonoHelper.Instance.StopCoroutine(item);
            }
    }

    //アップデート配布時間の受信
    public virtual void Update()
    {

    }

    //マウスエンターメソッド
    public virtual void PointerEnter(BaseEventData data)
    {

    }
    //マウスアウトメソッド
    public virtual void PointerExit(BaseEventData data)
    {

    }
    //マウスクリック方法
    public virtual void PointerClick(BaseEventData data)
    {

    }

}
