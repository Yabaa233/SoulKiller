// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;


//一時的に使用しない
// public class WindowManager : singleton<WindowManager>
// {
//     //すべてのウィンドウを維持する辞書クラス
//     Dictionary<WindowType,BaseWindow> windowDIc = new Dictionary<WindowType, BaseWindow>();

//     // コンストラクタ 初期化を行う
//     public WindowManager()
//     {
//         //辞書に登録関連ウィンドウを埋め込む
//         windowDIc.Add(WindowType.TestWindow,new TestWindow());
//         windowDIc.Add(WindowType.RollingWindow,new RollingWindow());
//     }


    
//     //パネルに提供される自動登録方法
//     public void RegisterUI(WindowType windowType,BaseWindow baseWindow)
//     {
//         if(windowDIc.ContainsKey(windowType))//すでに登録されている場合
//         {
//             return;
//         }
//         else
//         {
//             windowDIc.Add(windowType,baseWindow);//辞書に登録する
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

//     //ウィンドウを開く方法
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

//     //ウィンドウを閉じる方法
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
//     //プリロード方法
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

//     //特定のタイプのすべてのウィンドウを非表示にする
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

//     //すべてのウィンドウを強制的にアンインストールします
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
