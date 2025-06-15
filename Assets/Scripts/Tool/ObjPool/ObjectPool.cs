using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//池内オブジェクトの初期化情報
[System.Serializable]
public struct PoolObjectItem
{
    [Header("プールインデックス")]
    public string key;
    [Header("プレファブ体")]
    public GameObject prefab;
    [Header("最大サイズ")]
    public int maxSize;
    [Header("プレファブの親オブジェクトを管理する")]
    public Transform parent;
}
//池内のオブジェクトストレージ情報

public class ObjectPool : singleton<ObjectPool>
{
    #region 对象池信息
    //オブジェクトプールは、さまざまなタイプのオブジェクトを格納でき、名前でインデックスを作成します。対応するタイプのキューがあれば操作できますが、なければ一時的に作成する必要があります。
    private Dictionary<string, List<GameObject>> pool = new Dictionary<string, List<GameObject>>();
    //オブジェクトプールのサイズ、各プールのサイズを保存します。
    private Dictionary<string, int> poolSize = new Dictionary<string, int>();
    //オブジェクトプールの親クラス、各プールの親クラスの位置を保存します。
    private Dictionary<string, Transform> poolParent = new Dictionary<string, Transform>();
    //オブジェクトプールのプレハブ、各プールのプレハブを保存し、プールの動的な拡張に使用します。
    private Dictionary<string, GameObject> poolPrefab = new Dictionary<string, GameObject>();
    #endregion

    [Header("ここに池に必要な物体を追加してください。")]
    [SerializeField] public List<PoolObjectItem> poolObjectItems = new List<PoolObjectItem>();    //インスペクターでプールを作成するためのオブジェクト情報を追加します。

    ///<summary>


    ////// 初期化し、既存の情報に基づいてオブジェクトプールを作成します。


    ///</summary>
    protected override void Awake()
    {
        base.Awake();
        foreach (PoolObjectItem item in poolObjectItems)
        {
            CreateNewPool(item);
        }
        poolObjectItems.Clear();    //使用後のクリアでスペースを節約します
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// すべてのプールを削除します
    /// </summary>
    override protected void OnDestroy()
    {
        base.OnDestroy();
        DestroyPool();
    }

    /// <summary>
    /// すべてのプールを削除します
    /// </summary>
    public void DestroyPool()
    {
        pool.Clear();
        poolSize.Clear();
        poolParent.Clear();
        poolPrefab.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform curParent = transform.GetChild(i);
            for (int j = 0; j < curParent.childCount; j++)
            {
                Destroy(curParent.GetChild(j).gameObject);
            }
        }
    }

    //関数：CreateNewPool(PoolObjectItem item)【新しいプールを作成する】
    //パラメータ：item【新しいプールを作成するために必要な情報の構造体】
    public void CreateNewPool(PoolObjectItem item)
    {
        poolSize.Add(item.key, item.maxSize);
        poolParent.Add(item.key, item.parent);
        poolPrefab.Add(item.key, item.prefab);
        List<GameObject> newPool = new List<GameObject>();
        GameObject newObject = null;
        for (int i = 0; i < item.maxSize; i++)
        {
            newObject = Instantiate(item.prefab, item.parent);
            newObject.name = item.key + i;
            newObject.SetActive(false);
            newPool.Add(newObject);
        }
        pool.Add(item.key, newPool);
    }


    //関数：void RecycleObj(string poolKey, GameObject obj)【単一オブジェクトのリサイクル】
    //パラメータ：poolKey【プール内のインデックス。見つからない場合は新しいプールを作成します】
    //パラメータ：obj【回収対象物体】
    public void RecycleObj(string poolKey, GameObject obj)
    {
        var parent = poolParent[poolKey];
        obj.transform.SetParent(parent);    //特定の親オブジェクトにマウントする
        obj.transform.position = parent.transform.position;
        obj.SetActive(false);

        if (pool.ContainsKey(poolKey))
        {
            //最大サイズ内のみがプールに回収されます。
            if (pool[poolKey].Count < poolSize[poolKey])
            {
                pool[poolKey].Add(obj);
            }
        }
        else
        {
            //新しいオブジェクトプール情報を作成する
            PoolObjectItem newPoolItem = new PoolObjectItem();
            //新しいオブジェクトプールのデフォルト親オブジェクトを作成します
            Transform newParent = new GameObject(poolKey + "Parent").transform;
            newParent.SetParent(transform);
            newPoolItem.key = poolKey;
            newPoolItem.maxSize = 50;   //デフォルトのサイズは50で、変更可能です。
            newPoolItem.prefab = obj;
            newPoolItem.parent = newParent;
            CreateNewPool(newPoolItem);
        }
    }

    //関数：void RecycleAllChildren(Transform parent, string poolKey)【すべてのオブジェクトをリサイクル】
    //パラメータ：parent【その下の子オブジェクトを回収する必要がある親オブジェクト】
    //パラメータ：poolKey【プール内のインデックス。見つからない場合は新しいプールを作成します】
    public void RecycleAllChildren(Transform parent, string poolKey)
    {
        for (; parent.childCount > 0;)
        {
            var temp = parent.GetChild(0).gameObject;
            RecycleObj(poolKey, temp);
        }
    }

    /// <summary>
    /// オブジェクトプールからオブジェクトを取得する
    /// </summary>
    /// <param name="poolKey">プール内のインデックス、見つからない場合は警告</param>
    /// <param name="create">オブジェクトプールが空の場合、新しいオブジェクトを作成する必要があるかどうか</param>
    /// <param name="active">初期化時に表示するかどうか</param>
    /// <returns> プールから取り出されたオブジェクトの参照を返します </returns>
    public GameObject GetObject(string poolKey, bool create = false, bool active = false)
    {
        GameObject result = null;
        if (pool.ContainsKey(poolKey))
        {
            if (pool[poolKey].Count > 0)
            {
                result = pool[poolKey][0];
                result.SetActive(active);
                pool[poolKey].Remove(result);
                return result;
            }
            else
            {
                if (create)
                {
                    result = Instantiate(poolPrefab[poolKey], poolParent[poolKey]);
                    RecycleObj(poolKey, result);
                    GetObject(poolKey, create, active); //再度呼び出す必要があり、オブジェクトがキューから出ることを保証します。
                    Debug.LogWarning("オブジェクトプールがいっぱいで、新しいオブジェクトを作成する必要があります！");
                    return result;
                }
                else
                {
                    Debug.LogWarning("オブジェクトプールが満杯で、新しいオブジェクトはこれ以上作成されません！");
                    return result;
                }
            }
        }
        else
        {
            Debug.LogWarning("このオブジェクトプール情報はありません");
            return result;
        }
    }

    /// <summary>
    /// オブジェクトプールからオブジェクトを取得する
    /// </summary>
    /// <param name="poolKey">プール内のインデックス、見つからない場合は警告</param>
    /// <param name="parent">親オブジェクトにマウントしたい</param>
    /// <param name="create">オブジェクトプールが空の場合、新しいオブジェクトを作成する必要があるかどうか</param>
    /// <param name="active">初期化時に表示するかどうか</param>
    /// <returns> プールから取り出されたオブジェクトの参照を返します </returns>
    public GameObject GetObject(string poolKey, Transform parent, bool create = false, bool active = false)
    {
        GameObject result = null;
        if (pool.ContainsKey(poolKey))
        {
            if (pool[poolKey].Count > 0)
            {
                result = pool[poolKey][0];
                result.transform.parent = parent;   ///親オブジェクトをマウントします
                result.SetActive(active);
                pool[poolKey].Remove(result);
                return result;
            }
            else
            {
                if (create)
                {
                    result = Instantiate(poolPrefab[poolKey], poolParent[poolKey]);
                    RecycleObj(poolKey, result);
                    GetObject(poolKey, parent, create, active); //再度呼び出す必要があり、オブジェクトがキューから出ることを保証します。
                    Debug.LogWarning("オブジェクトプールがいっぱいで、新しいオブジェクトが作成されました！");
                    return result;
                }
                else
                {
                    Debug.LogWarning("オブジェクトプールが満杯で、新しいオブジェクトはこれ以上作成されません！");
                    return result;
                }
            }
        }
        else
        {
            Debug.LogWarning("このオブジェクトプール情報はありません");
            return result;
        }
    }
}
