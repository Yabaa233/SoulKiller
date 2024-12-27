using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRoot
{
    private static Transform rootTransform;
    private static Transform recyclePool;//回收的窗体 隐藏
    private static Transform workStation;//前台显示，工作中的窗体
    private static Transform noticesStation;//提示类型的窗体
    private static bool isInit = false;//是否完成了初始化,避免空引用

    //不常驻场景
    // public static void Init()
    // {
    //     //每次都重新创建一下
    //     if(transform!=null)
    //     {
    //         GameObject.Destroy(transform.gameObject);
    //     }

    //     //初始化创建整个UI层级布局
    //     if(transform == null)
    //     {
    //         GameObject obj = Resources.Load<GameObject>("UI/UIRoot");
    //         transform = GameObject.Instantiate(obj).transform;
    //     }

    //     if(recyclePool == null)
    //     {
    //         recyclePool = transform.Find("recyclePool");
    //     }

    //     if(workStation == null)
    //     {
    //         workStation = transform.Find("workStation");
    //     }

    //     if(noticesStation == null)
    //     {
    //         noticesStation = transform.Find("noticesStation");
    //     }
    //     isInit = true;
    // }

    //让元素常驻场景
    public static void Init()
    {
        rootTransform = GameObject.Find("UIRoot").transform;
        recyclePool = rootTransform.Find("recyclePool");
        workStation = rootTransform.Find("workStation");
        noticesStation = rootTransform.Find("noticesStation");
        isInit = true;
    }

    //对外接口部分
    public static void SetParent(Transform window,bool isOpen,bool isTipsWindow =false)
    {
        if(isInit == false)
        {
            Init();//如果还没有初始化则直接初始化
        }

        if(isOpen == true)//如果面板窗口处于工作状态，则放入下列两个的其中一种窗口
        {
            if(isTipsWindow)
            {
                window.SetParent(noticesStation,false);
            }
            else
            {
                window.SetParent(workStation,false);
            }
        }
        else//非工作状态下的窗口进行回收
        {
            window.SetParent(recyclePool,false);
        }
    }

}
