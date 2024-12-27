using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// UI的管理工具，获取某个子对象的组件
/// </summary>
public class UITool 
{
    //当前的活动面板
    GameObject activePanel;

    public UITool(GameObject panel)
    {
        activePanel = panel;
    }

    //返回当前UI对象
    public GameObject GetUI()
    {
        return activePanel;
    }

    /// <summary>
    /// 给当前的活动面板获取或添加一个组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <returns></returns>
    public T GetOrAddComponent<T>() where T : Component
    {
        if (activePanel.GetComponent<T>() == null)
            activePanel.AddComponent<T>();

        return activePanel.GetComponent<T>();
    }

    /// <summary>
    /// 根据名称查找一个子对象
    /// </summary>
    /// <param name="name">子对象名称</param>
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

        Debug.LogWarning($"{activePanel.name}里找不到名为{name}的子对象");
        return null;
    }

    /// <summary>
    /// 根据名称获取一个子对象的组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="name">子对象的名称</param>
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
    /// 绑定eventTrigger监听方法
    /// </summary>
    /// <param name="obj">物体</param>
    /// <param name="eventTriggerType">触发类型</param>
    /// <param name="myMehod">调用方法</param>
    public void addTriggersListener(EventTriggerType eventTriggerType, MyMehod myMehod,GameObject obj=null)
    {
        if (obj == null)
            obj = activePanel;

        EventTrigger trigger;
        if (obj.GetComponent<EventTrigger>() == null)
            trigger = obj.AddComponent<EventTrigger>();

        trigger = obj.GetComponent<EventTrigger>();

        
        //添加监听
        if (trigger.triggers.Count == 0)
        {
            //初始化entry容器
            trigger.triggers = new List<EventTrigger.Entry>();
        }
        //实例化委托
        UnityAction<BaseEventData> callBack = new UnityAction<BaseEventData>(myMehod);
        //实例化entry对象
        EventTrigger.Entry entry = new EventTrigger.Entry();
        //指定事件触发类型
        entry.eventID = eventTriggerType;
        //添加监听
        entry.callback.AddListener(callBack) ;

        trigger.triggers.Add(entry);

        //添加拖拽屏蔽
        if(obj.GetComponent<ScrollTool>()==null)
            obj.AddComponent<ScrollTool>();
    }



}
