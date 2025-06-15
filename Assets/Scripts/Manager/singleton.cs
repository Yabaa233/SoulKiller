using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class singleton<T> : MonoBehaviour where T : singleton<T>
{
    private static T instance;  //シングルトンを作成する
    public static T Instance
    {
        get { return instance; }
    }

    protected virtual void Awake()  //サブクラスが継承および修正を許可する
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = (T)this;
        }
    }

    public static bool IsInitialized    //シングルトンがすでに生成されているかどうかを判断する
    {
        get { return instance != null; }
    }

    protected virtual void OnDestroy()  //破壊される時に空に設定します
    {
        if(instance == this)    instance = null;
    }
}
