using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRoot
{
    private static Transform rootTransform;
    private static Transform recyclePool;//リサイクルされたウィンドウ 隠す
    private static Transform workStation;//フロントディスプレイ、作業中のウィンドウ
    private static Transform noticesStation;//ヒントタイプのウィンドウ形式
    private static bool isInit = false;//初期化は完了し、null参照を避けましたか？

    //常駐シーンではない
    // public static void Init()
    // {
    //     //毎回新しく作成します
    //     if(transform!=null)
    //     {
    //         GameObject.Destroy(transform.gameObject);
    //     }

    //     // UI階層レイアウト全体の初期化作成
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

    //エレメントをシーンに常駐させる
    public static void Init()
    {
        rootTransform = GameObject.Find("UIRoot").transform;
        recyclePool = rootTransform.Find("recyclePool");
        workStation = rootTransform.Find("workStation");
        noticesStation = rootTransform.Find("noticesStation");
        isInit = true;
    }

    //外部インターフェース部分
    public static void SetParent(Transform window,bool isOpen,bool isTipsWindow =false)
    {
        if(isInit == false)
        {
            Init();//まだ初期化されていなければ、直接初期化してください。
        }

        if(isOpen == true)//パネルウィンドウが作業状態にある場合、以下の二つのウィンドウのうち一つを入れてください。
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
        else//非稼働状態のウィンドウをリサイクルします
        {
            window.SetParent(recyclePool,false);
        }
    }

}
