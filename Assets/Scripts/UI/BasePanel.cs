using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 所有界面的基类，存放界面信息和通用方法
/// xushi
/// </summary>
public class BasePanel
{
    //UI基本信息
    public UIType UIType { get; private set; }

    //UI工具
    public UITool UITool { get; private set; }

    public BasePanel(UIType uIType)
    {
        UIType = uIType;
    }

    //UI启动的所有协程信息
    protected List<Coroutine> coroutines;

    //UI是否需要Update
    public bool ifNeedUpdate = false;

    //初始化加载
    public void Init(UITool tool){
        //皮肤
        // GameObject skinPrefab = ResManager.LoadPrefab(skinPath);//资源加载模块后续要补

        //当前UI的UI工具（包括当前GameObject信息）
        UITool = tool;
        coroutines = new List<Coroutine>();
    }
    //关闭
    public virtual void Close()
    {
        PanelManager.Instance.Close(UIType);
    }

    //暂停时
    public virtual void OnPause()
    {

    }

    //初始化时
    public virtual void OnInit(){

    }

    //显示的时候
    public virtual void OnShow(params object[] para){
        
    }
    //关闭的时候
    public virtual void OnClose(){
        //关闭所有协程
        if (coroutines != null)
            foreach (var item in coroutines)
            {
                MonoHelper.Instance.StopCoroutine(item);
            }
    }

    //接收分发Update时间
    public virtual void Update()
    {

    }

    //鼠标进入方法
    public virtual void PointerEnter(BaseEventData data)
    {

    }
    //鼠标移出方法
    public virtual void PointerExit(BaseEventData data)
    {

    }
    //鼠标点击方法
    public virtual void PointerClick(BaseEventData data)
    {

    }

}
