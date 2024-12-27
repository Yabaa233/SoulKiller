using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//某一大类Buff的所有小类
public enum BuffType
{
    Buff1,
    Buff2,
    Buff3,
}

//创建一个委托类型，同时配置好各差异函数，将函数作为形参传入BuffBase中，BuffBase中设置委托变量接收此函数
public delegate void BuffFunction();

//BallBuff的存储方式 类型——概率
[System.Serializable]
public struct BuffItem
{
    public BuffType type;   //类型
    public float probability;   //概率
    public Sprite sprite;   //贴图
}

[System.Serializable]
public class BuffManager : singleton<BuffManager>
{
    [Header("某类Buff的概率")]
    public float isxxxBuff = 0.4f;
    [Header("另一类Buff的概率")]
    public float isxxxxBuff = 0.3f;
    [Header("另一类Buff的概率")]
    public float isxxxxxBuff = 0.3f;

    [Header("各种Buff对应概率")]
    [SerializeField] public List<BuffItem> ballBuffs;    //用户配置每种Buff的接口
    [Header("Buff预制体")]
    public GameObject BuffPrefab;       //用户配置Buff预制体接口
    public Dictionary<BuffType, BuffFunction> Dic_BuffFunction; //存储和查询各buff的功能函数
    private List<float> BuffProbabilitys;    //每个Buff的概率，用于动态创建Buff
    private Queue<BuffFunction> Que_BuffFunction = new Queue<BuffFunction>();   //委托队列，依次处理

    private bool Processing;   //是否正在处理Buff

    protected override void Awake()
    {
        base.Awake();
        BuffProbabilitys = new List<float>();
        //初始化Buff队列
        Dic_BuffFunction = new Dictionary<BuffType, BuffFunction>();

        #region 初始化分配各Buff出现概率
        // isPaddleBuff += isBallBuff;
        // isBrickBuff += isPaddleBuff;
        #endregion

        #region 初始化Buff函数字典
        //BuffFunction初始化
        Dic_BuffFunction.Add(BuffType.Buff1, BuffFunction_BuffType_Buff1);
        Dic_BuffFunction.Add(BuffType.Buff2, BuffFunction_BuffType_Buff2);
        Dic_BuffFunction.Add(BuffType.Buff3, BuffFunction_BuffType_Buff3);
        #endregion

        #region 初始化第1大类Buff
        float curProbability = 0;
        foreach (var i in ballBuffs)
        {
            curProbability += i.probability;
            BuffProbabilitys.Add(curProbability);   //对应索引对应概率
        }
        #endregion

        #region 初始化第2大类Buff
       
        #endregion

        #region 初始化第3大类Buff
        
        #endregion
    }
    //清空队列
    private void Update()
    {
        if (Processing) return; //如果正在处理Buff，直接返回
        if (Que_BuffFunction.Count != 0)
        {
            Que_BuffFunction.Dequeue()();
            Debug.Log("执行一个BallBuff");
        }
    }
    //根据配置的各种Buff的概率选择性进入对应的Buff获取函数
    public void GetBuff(Vector3 position, Transform parent)
    {
        float rand = Random.Range(0f, isxxxxxBuff);
        if (rand < isxxxBuff)
        {
            GetRandomBuffType(position, parent);
        }
        else if (rand < isxxxxBuff)
        {
            //另一种Buff
            // GetRandomBuffType(position, parent);
        }
        else
        {
            //另一种Buff
            // GetRandomBuffType(position, parent);
        }
    }

    //随机生成一个Buff
    //如果随机出来的值比BuffProbabilitys中最后一个值还大说明没有随机到Buff
    public void GetRandomBuffType(Vector3 Pos, Transform parent)
    {
        float rand = Random.Range(0f, BuffProbabilitys.Last());
        if (rand > BuffProbabilitys.Last()) //如果比最后一个值还大，说明没有buff出现
        {
            return;
        }
        for (int i = 0; i < BuffProbabilitys.Count; i++)    //遍历概率列表，找到并掉落对应Buff
        {
            if (rand < BuffProbabilitys[i])
            {
                GameObject newBallBuff = Instantiate(BuffPrefab, Pos, Quaternion.identity, parent);
                //查询字典获取事件执行的内容并初始化对应Buff
                // newBallBuff.GetComponent<BuffBase>().Init();
                break;
            }
        }
        return;
    }

    //清空当前场景中的所有BUFF
    public void ClearBuffs()
    {
        int i = 0;
        //清空Buff队列
        Que_BuffFunction.Clear();
        while (i < transform.childCount)
        {
            Destroy(transform.GetChild(i++).gameObject);
        }
    }

    //***********************************
    //
    //            BuffType具体实现
    //
    //***********************************
    public void BuffFunction_BuffType_Buff1()
    {
        Debug.Log("Buff1");
    }
    public void BuffFunction_BuffType_Buff2()
    {
        Debug.Log("Buff2");
    }
    public void BuffFunction_BuffType_Buff3()
    {
        Debug.Log("Buff3");
    }
}
