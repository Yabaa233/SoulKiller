// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;


//暂时不使用
// public class WindowManager : singleton<WindowManager>
// {
//     //维护所有窗口的字典类
//     Dictionary<WindowType,BaseWindow> windowDIc = new Dictionary<WindowType, BaseWindow>();

//     //构造函数 做初始化
//     public WindowManager()
//     {
//         //往字典中填充注册相关窗口
//         windowDIc.Add(WindowType.TestWindow,new TestWindow());
//         windowDIc.Add(WindowType.RollingWindow,new RollingWindow());
//     }


    
//     //提供给面板的自动注册方法
//     public void RegisterUI(WindowType windowType,BaseWindow baseWindow)
//     {
//         if(windowDIc.ContainsKey(windowType))//如果已经注册过了
//         {
//             return;
//         }
//         else
//         {
//             windowDIc.Add(windowType,baseWindow);//注册到字典中
//         }
//     }

//     //更新方法
//     public void Update()
//     {
//         foreach(var window in windowDIc.Values)
//         {
//             if(window.IsVisible())
//             {
//                 window.Update(Time.deltaTime);
//             }
//         }
//     }

//     //打开窗口方法
//     public BaseWindow OpenWindow(WindowType type)
//     {
//         BaseWindow window;
//         if(windowDIc.TryGetValue(type,out window))
//         {
//             window.Open();
//             return window;
//         }
//         else
//         {
//             Debug.LogError($"Open Error{type}");
//             return null; 
//         }
//     }

//     //关闭窗口方法
//     public void CloseWindow(WindowType type)
//     {
//         BaseWindow window;
//         if(windowDIc.TryGetValue(type,out window))
//         {
//             window.Close();
//         }
//         else
//         {
//             Debug.LogError($"Close Error{type}");
//         }
//     }
//     //预加载方法
//     public void PreLoadWindow(ScenesType type)
//     {
//         foreach(var item in windowDIc.Values)
//         {
//             if(item.GetScenesType() == type)
//             {
//                 item.PreLoad(); 
//             }
//         }
//     }

//     //隐藏掉某个类型的所有窗口
//     public void HideALLWindow(ScenesType type,bool isDestroy = false)
//     {
//         foreach(var item in windowDIc.Values)
//         {
//             if(item.GetScenesType() == type)
//             {
//                 if(isDestroy == false)
//                 {
//                     item.Close();
//                 }
//                 else
//                 {
//                     item.Close(true);
//                 }
//             }
//         }
//     }

//     //强制卸载所有的窗口
//     public void ForceCloseWindow(ScenesType type)
//     {
//         foreach(var item in windowDIc.Values)
//         {
//             if(item.GetScenesType() == type)
//             {
//                 item.Close(true);
//             }
//         }
//     }


// }
