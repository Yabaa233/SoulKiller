using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// UIの管理ツール、特定の子オブジェクトのコンポーネントを取得
/// </summary>
public class UITool 
{
    //現在のアクティビティパネル
    GameObject activePanel;

    public UITool(GameObject panel)
    {
        activePanel = panel;
    }

    //現在のUIオブジェクトを返す
    public GameObject GetUI()
    {
        return activePanel;
    }

    ///<summary>


    ////// 現在のアクティビティパネルにコンポーネントを取得または追加します


    ///</summary>
    /// <typeparam name="T">コンポーネントタイプ</typeparam>
    /// <returns></returns>
    public T GetOrAddComponent<T>() where T : Component
    {
        if (activePanel.GetComponent<T>() == null)
            activePanel.AddComponent<T>();

        return activePanel.GetComponent<T>();
    }

    ///<summary>


    ////// 名前でサブオブジェクトを検索する


    ///</summary>
    /// <param name="name">子オブジェクト名</param>
    /// <returns></returns>
    public GameObject FindChildGameObject(string name)
    {
        Transform[] trans = activePanel.GetComponentsInChildren<Transform>();

        foreach (Transform item in trans)
        {
            if (item.name == name)
            {
                return item.gameObject;
            }
        }

        Debug.LogWarning($"{activePanel.name}内には{name}という名前のサブオブジェクトが見つかりません");
        return null;
    }

    ///<summary>


    ////// 名前に基づいて子オブジェクトのコンポーネントを取得します。


    ///</summary>
    /// <typeparam name="T">コンポーネントタイプ</typeparam>
    /// <param name="name">子オブジェクトの名前</param>
    /// <returns></returns>
    public T GetOrAddComponentInChildren<T>(string name) where T : Component
    {
        GameObject child = FindChildGameObject(name);
        if (child)
        {
            if (child.GetComponent<T>() == null)
                child.AddComponent<T>();

            return child.GetComponent<T>();
        }
        return null;
    }

    public  delegate void MyMehod(BaseEventData pd);

    /// <summary>
    /// eventTriggerのリスナーメソッドをバインドする
    /// </summary>
    /// <param name="obj">オブジェクト</param>
    /// <param name="eventTriggerType">トリガータイプ</param>
    /// <param name="myMehod">メソッドを呼び出す</param>
    public void addTriggersListener(EventTriggerType eventTriggerType, MyMehod myMehod,GameObject obj=null)
    {
        if (obj == null)
            obj = activePanel;

        EventTrigger trigger;
        if (obj.GetComponent<EventTrigger>() == null)
            trigger = obj.AddComponent<EventTrigger>();

        trigger = obj.GetComponent<EventTrigger>();

        
        //リスナーを追加する
        if (trigger.triggers.Count == 0)
        {
            //エントリーコンテナを初期化する
            trigger.triggers = new List<EventTrigger.Entry>();
        }
        //デリゲートのインスタンス化
        UnityAction<BaseEventData> callBack = new UnityAction<BaseEventData>(myMehod);
        //エントリーオブジェクトのインスタンス化
        EventTrigger.Entry entry = new EventTrigger.Entry();
        //指定イベントトリガータイプ
        entry.eventID = eventTriggerType;
        //リスナーを追加する
        entry.callback.AddListener(callBack) ;

        trigger.triggers.Add(entry);

        //ドラッグアンドドロップのブロックを追加する
        if(obj.GetComponent<ScrollTool>()==null)
            obj.AddComponent<ScrollTool>();
    }



}
