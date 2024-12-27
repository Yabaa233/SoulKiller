// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;

//暂时不使用
// public class BaseWindow
// {
//     //窗体本体
//     protected Transform transform;
//     //资源名称
//     protected string resName;
//     //是否常驻
//     protected bool resident;
//     //是否可见
//     protected bool visble = false;
//     //窗体类型
//     protected WindowType selfType;
//     //场景类型
//     protected ScenesType scenesType;

//     //UI控件 主要是按钮的反馈事件
//     //按钮列表
//     protected Button[] buttonList;


//     //////////给子类提供的接口：
//     //初始化
//     protected virtual void Awake()
//     {
//         //bool类型为true表示隐藏的物体也会去查找
//         buttonList = transform.GetComponentsInChildren<Button>(true);
//         RegisterUIEvent(); 
//     }

//     //UI事件注册
//     protected virtual void RegisterUIEvent()
//     {
        
//     }

//     //添加监听游戏事件（比如网络事件和游戏逻辑事件）
//     protected virtual void OnAddListener()
//     {
        
//     }

//     //移除游戏事件
//     protected virtual void OnRemoveListener()
//     {

//     }

//     //每次打开
//     protected virtual void OnEnable()
//     {

//     }

//     //每次关闭
//     protected virtual void OnDisable()
//     {

//     }

//     //每一帧的更新
//     public virtual void Update(float deltaTime)
//     {

//     }

//     //--------------针对WindowManager的接口------------------
//     //打开窗口
//     public void Open()
//     {
//         //如果当前物体为空，则去调用初始化方法
//         if(transform == null)
//         {
//             if(Create())
//             {
//                 Awake();//初始化
//             }
//         }

//         //如果处于没有激活的状态（在Create方法中其实被显式隐藏了）
//         if(transform.gameObject.activeSelf == false)
//         {
//             UIRoot.SetParent(transform,true,selfType == WindowType.TipsWindow);
//             transform.gameObject.SetActive(true);
//             visble = true;
//             OnEnable();//调用激活时应该触发的事件
//             OnAddListener();//添加激活时应该监听的事件
//         }
//     }

//     public void Close(bool isDestroy=false)
//     {
//         if(transform.gameObject.activeSelf == true)
//         {
//             OnRemoveListener();//首先移除游戏事件监听
//             OnDisable();//调用隐藏关闭事件

//             if(isDestroy == false)//判断是否是强制关闭（比如重新加载游戏的时候可能要全部卸载）
//             {
//                 if(resident)//判断是不是常驻内存的面板，如果是频繁调用的面板让它常驻内存可以降低开销
//                 {
//                     transform.gameObject.SetActive(false);
//                     UIRoot.SetParent(transform,false,false);
//                 }
//                 else
//                 {
//                     GameObject.Destroy(transform.gameObject);
//                     transform = null;
//                 }
//             }
//             else
//             {
//                 GameObject.Destroy(transform.gameObject);
//                 transform = null;
//             }
//         }
//         //不可见的状态 
//         visble = false;
//     }

//     //预加载方法只需要简单的初始化面板即可
//     public void PreLoad()
//     {
//         if(transform == null)
//         {
//             if(Create())
//             {
//                 Awake();
//             }
//         }
//     }

//     public ScenesType GetScenesType()//得到属于场景
//     {
//         return scenesType;
//     }

//     public WindowType GetWindowType()//得到窗体类型
//     {
//         return selfType;
//     }

//     public Transform GetRoot()//得到窗体本体
//     {
//         return transform;
//     }

//     public bool IsVisible()//得到窗体是否可见
//     {
//         return visble;
//     }

//     public bool isResident()//窗体是否要常驻内存
//     {
//         return resident;
//     }

//     //-----------------内部调用的接口--------------------
//     //创建面板的接口
//     private bool Create()
//     {
//         if(string.IsNullOrEmpty(resName))
//         {
//             return false;
//         }

//         if(transform == null)
//         {
//             //创建出面板
//             GameObject obj = Resources.Load<GameObject>(resName);
//             if(obj == null)
//             {
//                 Debug.LogError($"没有找到UI预制体{selfType}");
//                 return false;
//             }
//             transform = GameObject.Instantiate(obj).transform;

//             transform.gameObject.SetActive(false);//先将面板隐藏，这样在之后打开就可以调用OnEnable方法，比较方便

//             UIRoot.SetParent(transform,false,selfType == WindowType.TipsWindow);
//             return true;
//         }

//         return true;
//     }
    
// }


// //窗口的类型
// public enum WindowType
// {
//     MainWindow,
//     TipsWindow,//提示窗口
//     TestWindow,//用于测试的窗口
//     RollingWindow,//旋转窗口
// }

// //属于哪个场景,提供根据场景类型进行的预加载功能
// public enum ScenesType
// {
//     Main,
//     KB,
// }
