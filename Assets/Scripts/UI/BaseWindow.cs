// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;

//一時的に使用しない
// public class BaseWindow
// {
//     //ウィンドウ本体
//     protected Transform transform;
//     //リソース名
//     protected string resName;
//     //常駐していますか
//     protected bool resident;
//     //見えますか
//     protected bool visble = false;
//     //ウィンドウタイプ
//     protected WindowType selfType;
//     //シーンタイプ
//     protected ScenesType scenesType;

//     //UIコントロール 主にボタンのフィードバックイベント
//     //ボタンリスト
//     protected Button[] buttonList;


//     サブクラスに提供されるインターフェース：
//     //初期化
//     protected virtual void Awake()
//     {
//         //bool型がtrueであると、隠されたオブジェクトも検索されます
//         buttonList = transform.GetComponentsInChildren<Button>(true);
//         RegisterUIEvent(); 
//     }

//     //UIイベント登録
//     protected virtual void RegisterUIEvent()
//     {
        
//     }

//     //ゲームイベント（ネットワークイベントやゲームロジックイベントなど）のリスナーを追加する
//     protected virtual void OnAddListener()
//     {
        
//     }

//     //ゲームイベントを削除
//     protected virtual void OnRemoveListener()
//     {

//     }

//     //毎回開く
//     protected virtual void OnEnable()
//     {

//     }

//     //毎回閉じる
//     protected virtual void OnDisable()
//     {

//     }

//     //各フレームの更新
//     public virtual void Update(float deltaTime)
//     {

//     }

//     //--------------WindowManagerに対するインターフェース------------------
//     //ウィンドウを開く
//     public void Open()
//     {
//         //現在のオブジェクトが空の場合、初期化メソッドを呼び出します。
//         if(transform == null)
//         {
//             if(Create())
//             {
//                 Awake(); //初期化
//             }
//         }

//         //アクティブでない状態（Createメソッドで明示的に隠されている）の場合
//         if(transform.gameObject.activeSelf == false)
//         {
//             UIRoot.SetParent(transform,true,selfType == WindowType.TipsWindow);
//             transform.gameObject.SetActive(true);
//             visble = true;
//             OnEnable();//アクティブ化時にトリガーするべきイベントを呼び出す
//             OnAddListener();//アクティブ化時にリスナーが監視すべきイベントを追加します
//         }
//     }

//     public void Close(bool isDestroy=false)
//     {
//         if(transform.gameObject.activeSelf == true)
//         {
//             OnRemoveListener();//まず、ゲームイベントのリスナーを削除します
//             OnDisable();//非表示クローズイベントを呼び出す

//             if(isDestroy == false)//強制的に閉じるかどうかを判断する（例えば、ゲームを再読み込みする時には全てをアンインストールする必要があるかもしれません）
//             {
//                 if(resident)//パネルが常駐メモリかどうかを判断し、頻繁に呼び出されるパネルを常駐メモリにすることでコストを削減できます。
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
//         //見えない状態
//         visble = false;
//     }

//     //プリロード方法は、パネルを簡単に初期化するだけで十分です。
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

//     public ScenesType GetScenesType()//シーンタイプを取得する
//     {
//         return scenesType;
//     }

//     public WindowType GetWindowType()//ウィンドウタイプを取得する
//     {
//         return selfType;
//     }

//     public Transform GetRoot()//ウィンドウ本体を取得
//     {
//         return transform;
//     }

//     public bool IsVisible()//ウィンドウが可視かどうかを取得します
//     {
//         return visble;
//     }

//     public bool isResident() //フォームがメモリに常駐するかどうか
//     {
//         return resident;
//     }

//     //-----------------内部呼び出しのインターフェース--------------------
//     //パネル作成のインターフェース
//     private bool Create()
//     {
//         if(string.IsNullOrEmpty(resName))
//         {
//             return false;
//         }

//         if(transform == null)
//         {
//             //パネルを作成する
//             GameObject obj = Resources.Load<GameObject>(resName);
//             if(obj == null)
//             {
//                 Debug.LogError($"UIプレハブ{selfType}が見つかりませんでした");
//                 return false;
//             }
//             transform = GameObject.Instantiate(obj).transform;

//             transform.gameObject.SetActive(false);//パネルを先に非表示にし、その後でOnEnableメソッドを呼び出すことができます。これは非常に便利です。

//             UIRoot.SetParent(transform,false,selfType == WindowType.TipsWindow);
//             return true;
//         }

//         return true;
//     }
    
// }


// //ウィンドウのタイプ
// public enum WindowType
// {
//     MainWindow,
//     TipsWindow,//ヒントウィンドウ
//     TestWindow, //テスト用のウィンドウ
//     ローリングウィンドウ
// }

// //どのシーンに属しているか、シーンタイプに基づいて行われるプリロード機能を提供します
// public enum ScenesType
// {
//     Main,
//     KB,
// }
