using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//池中对象初始化信息
[System.Serializable]
public struct PoolObjectItem
{
    [Header("池子索引")]
    public string key;
    [Header("预制体")]
    public GameObject prefab;
    [Header("最大尺寸")]
    public int maxSize;
    [Header("管理预制体的父物体")]
    public Transform parent;
}
//池中对象存储信息

public class ObjectPool : singleton<ObjectPool>
{
    #region 对象池信息
    //对象池，可以存放多种类型物体，根据name进行索引，如果有对应类型的队列则可以操作，没有则需要临时创建
    private Dictionary<string, List<GameObject>> pool = new Dictionary<string, List<GameObject>>();
    //对象池大小，存储每一个池子的大小
    private Dictionary<string, int> poolSize = new Dictionary<string, int>();
    //对象池父类，存储每一个池子的父类位置
    private Dictionary<string, Transform> poolParent = new Dictionary<string, Transform>();
    //对象池预制体，存储每一个池子的预制体，用于动态扩充池子
    private Dictionary<string, GameObject> poolPrefab = new Dictionary<string, GameObject>();
    #endregion

    [Header("在此处添加池中需要的物体")]
    [SerializeField] public List<PoolObjectItem> poolObjectItems = new List<PoolObjectItem>();    //添加对象的信息，用于Inspector中创建池子

    /// <summary>
    /// 初始化，根据已有信息创建对象池
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        foreach (PoolObjectItem item in poolObjectItems)
        {
            CreateNewPool(item);
        }
        poolObjectItems.Clear();    //用完清除，节约空间
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// 删除所有池子
    /// </summary>
    override protected void OnDestroy()
    {
        base.OnDestroy();
        DestroyPool();
    }

    /// <summary>
    /// 删除全部池子
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

    //函数：CreateNewPool(PoolObjectItem item)【创建新池子】
    //参数：item【创建新池子需要的信息结构体】
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


    //函数：void RecycleObj(string poolKey, GameObject obj)【回收单个物体】
    //参数：poolKey【在池中的索引，没有查找到则创建新池子】
    //参数：obj【待回收物体】
    public void RecycleObj(string poolKey, GameObject obj)
    {
        var parent = poolParent[poolKey];
        obj.transform.SetParent(parent);    //挂载到特定父物体
        obj.transform.position = parent.transform.position;
        obj.SetActive(false);

        if (pool.ContainsKey(poolKey))
        {
            //只有在最大尺寸内才会回收进池子
            if (pool[poolKey].Count < poolSize[poolKey])
            {
                pool[poolKey].Add(obj);
            }
        }
        else
        {
            //创建新对象池信息
            PoolObjectItem newPoolItem = new PoolObjectItem();
            //创建新对象池默认父物体
            Transform newParent = new GameObject(poolKey + "Parent").transform;
            newParent.SetParent(transform);
            newPoolItem.key = poolKey;
            newPoolItem.maxSize = 50;   //默认大小为50，可改
            newPoolItem.prefab = obj;
            newPoolItem.parent = newParent;
            CreateNewPool(newPoolItem);
        }
    }

    //函数：void RecycleAllChildren(Transform parent, string poolKey)【回收全部物体】
    //参数：parent【需要回收其下子物体的父物体】
    //参数：poolKey【在池中的索引，没有查找到则创建新池子】
    public void RecycleAllChildren(Transform parent, string poolKey)
    {
        for (; parent.childCount > 0;)
        {
            var temp = parent.GetChild(0).gameObject;
            RecycleObj(poolKey, temp);
        }
    }

    /// <summary>
    /// 从对象池中获取物体
    /// </summary>
    /// <param name="poolKey">在池中的索引，没有查找到则警告</param>
    /// <param name="create">如果对象池为空，是否需要创建新物体</param>
    /// <param name="active">初始化时是否显示</param>
    /// <returns> 返回从池中取出的物体引用 </returns>
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
                    GetObject(poolKey, create, active); //需要再次调用，保证物体能出队列
                    Debug.LogWarning("没有对象池已满，需要创建新物体！");
                    return result;
                }
                else
                {
                    Debug.LogWarning("没有对象池已满，并且不再创建新物体！");
                    return result;
                }
            }
        }
        else
        {
            Debug.LogWarning("没有此对象池信息");
            return result;
        }
    }

    /// <summary>
    /// 从对象池中获取物体
    /// </summary>
    /// <param name="poolKey">在池中的索引，没有查找到则警告</param>
    /// <param name="parent">希望挂载到的父物体</param>
    /// <param name="create">如果对象池为空，是否需要创建新物体</param>
    /// <param name="active">初始化时是否显示</param>
    /// <returns> 返回从池中取出的物体引用 </returns>
    public GameObject GetObject(string poolKey, Transform parent, bool create = false, bool active = false)
    {
        GameObject result = null;
        if (pool.ContainsKey(poolKey))
        {
            if (pool[poolKey].Count > 0)
            {
                result = pool[poolKey][0];
                result.transform.parent = parent;   ///挂载父物体
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
                    GetObject(poolKey, parent, create, active); //需要再次调用，保证物体能出队列
                    Debug.LogWarning("没有对象池已满，创建了新物体！");
                    return result;
                }
                else
                {
                    Debug.LogWarning("没有对象池已满，并且不再创建新物体！");
                    return result;
                }
            }
        }
        else
        {
            Debug.LogWarning("没有此对象池信息");
            return result;
        }
    }
}
