using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//ある大きなカテゴリのBuffのすべてのサブカテゴリ
public enum BuffType
{
    Buff1,
    Buff2,
    Buff3,
}

//デリゲートタイプを作成し、各差分関数を設定します。関数をBuffBaseに形式パラメータとして渡します。BuffBaseでは、この関数を受け取るためのデリゲート変数を設定します。
public delegate void BuffFunction();

//BallBuffの保存方法 タイプ - 確率
[System.Serializable]
public struct BuffItem
{
    public BuffType type;   //タイプ
    public float probability;   //確率
    public Sprite sprite;   //画像を貼り付ける
}

[System.Serializable]
public class BuffManager : singleton<BuffManager>
{
    [Header("ある種類のバフの確率")]
    public float isxxxBuff = 0.4f;
    [Header("もう一つの種類のBuffの確率")]
    public float isxxxxBuff = 0.3f;
    [Header("もう一つの種類のBuffの確率")]
    public float isxxxxxBuff = 0.3f;

    [Header("各種バフの対応確率")]
    [SerializeField] public List<BuffItem> ballBuffs;    //各種Buffのインターフェースを設定するユーザー
    [Header("Buffプレハブ体")]
    public GameObject BuffPrefab;       //ユーザーがBuffプレファブインターフェースを設定します
    public Dictionary<BuffType, BuffFunction> Dic_BuffFunction; //各種のバフを保存し、照会する機能関数
    private List<float> BuffProbabilitys;    //各Buffの確率、動的にBuffを作成するために使用されます。
    private Queue<BuffFunction> Que_BuffFunction = new Queue<BuffFunction>();   //委託キュー、順次処理

    private bool Processing;   //Buffは処理中ですか？

    protected override void Awake()
    {
        base.Awake();
        BuffProbabilitys = new List<float>();
        //Buffキューの初期化
        Dic_BuffFunction = new Dictionary<BuffType, BuffFunction>();

        #region 初始化分配各Buff出现概率
        // isPaddleBuff += isBallBuff;
        // isBrickBuff += isPaddleBuff;
        #endregion

        #region 初始化Buff函数字典
        //BuffFunctionの初期化
        Dic_BuffFunction.Add(BuffType.Buff1, BuffFunction_BuffType_Buff1);
        Dic_BuffFunction.Add(BuffType.Buff2, BuffFunction_BuffType_Buff2);
        Dic_BuffFunction.Add(BuffType.Buff3, BuffFunction_BuffType_Buff3);
        #endregion

        #region 初始化第1大类Buff
        float curProbability = 0;
        foreach (var i in ballBuffs)
        {
            curProbability += i.probability;
            BuffProbabilitys.Add(curProbability);   //対応するインデックスに対応する確率
        }
        #endregion

        #region 初始化第2大类Buff
       
        #endregion

        #region 初始化第3大类Buff
        
        #endregion
    }
    //キューをクリアする
    private void Update()
    {
        if (Processing) return; //Buffを処理中の場合、直接に戻ってください。
        if (Que_BuffFunction.Count != 0)
        {
            Que_BuffFunction.Dequeue()();
            Debug.Log("BallBuffを実行する");
        }
    }
    //設定された各種Buffの確率に基づいて、対応するBuff取得関数に選択的に入る
    public void GetBuff(Vector3 position, Transform parent)
    {
        float rand = Random.Range(0f, isxxxxxBuff);
        if (rand < isxxxBuff)
        {
            GetRandomBuffType(position, parent);
        }
        else if (rand < isxxxxBuff)
        {
            //もう一つのBuff
            // GetRandomBuffType(position, parent);
        }
        else
        {
            //もう一つのBuff
            // GetRandomBuffType(position, parent);
        }
    }

    //ランダムにバフを生成する
    //ランダムに生成された値がBuffProbabilitysの最後の値よりも大きい場合、Buffがランダムに選ばれていないことを意味します。
    public void GetRandomBuffType(Vector3 Pos, Transform parent)
    {
        float rand = Random.Range(0f, BuffProbabilitys.Last());
        if (rand > BuffProbabilitys.Last()) //最後の値よりも大きければ、buffが出現していないことを示します。
        {
            return;
        }
        for (int i = 0; i < BuffProbabilitys.Count; i++)    //確率リストを走査し、対応するBuffを見つけてドロップします。
        {
            if (rand < BuffProbabilitys[i])
            {
                GameObject newBallBuff = Instantiate(BuffPrefab, Pos, Quaternion.identity, parent);
                //辞書を参照してイベントの実行内容を取得し、対応するBuffを初期化します。
                // newBallBuff.GetComponent<BuffBase>().Init();
                break;
            }
        }
        return;
    }

    //現在のシーンのすべてのBUFFをクリアします。
    public void ClearBuffs()
    {
        int i = 0;
        //Buffキューをクリアする
        Que_BuffFunction.Clear();
        while (i < transform.childCount)
        {
            Destroy(transform.GetChild(i++).gameObject);
        }
    }

    //***********************************
    //
    // BuffTypeの具体的な実装
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
