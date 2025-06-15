using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// リング型UI、BattleMainPanelにマウントして現在の武器を表示し切り替えるために使用します。
/// xushi
/// </summary>
public class Weapen : BasePanel
{
    static readonly string path = "UI/Panel/Weapen(RollingWindow";

    public Weapen() : base(new UIType(path)) { }

    //選択肢プレファブ体
    private GameObject optionPrefab;
    //オプショングループの親オブジェクト
    private GameObject optionGroup;
    //オプショングループ
    private Transform[] options;
    //選択肢の総数
    [Range(0, 20)]
    private int optionNUm;
    //選択肢の総数の半分
    private float halfNum;
    //選択肢と選択肢の位置
    private Dictionary<Transform, Vector3> OptionP = new Dictionary<Transform, Vector3>();
    //階層順序を調整する辞書
    private Dictionary<Transform, int> OptionS = new Dictionary<Transform, int>();
    //回転中心
    private Vector3 center = Vector3.zero;
    //回転半径
    private float R = 50f;
    //回転速度
    [Range(1f, 100f)]
    private float speed;
    //y軸のオフセット量、すべての要素が見えるようにします。
    private float yOffet;
    //最小透明度
    [Range(0, 1)]
    public float minAlpha;

    //選択項目のズームレベル
    [Range(1, 5)]
    public float firstS;
    [Range(0, 1)]
    public float minS;//最小スケール度
    [Range(0, 1)]
    public float tempS;//回転過程におけるスケーリングの程度
    [Range(0, 0.5f)]
    public float smoothSTime;//ズームのスムーズな時間

    Coroutine currentPIE;//現在移動中のコルーチン
    Coroutine[] SIE2;//すべてのスケーリングコルーチン

    //枠線の色部分
    Image[] border;//選択肢の枠線
    [ColorUsage(true, false)]//最初のパラメータはAlphaチャンネルを表示するかどうか、二番目のパラメータはHDRを有効にするかどうかです。
    public Color originColor;//初期の枠線の色
    [ColorUsage(true, false)]
    public Color firstColor;//選択した枠線の色

    private Button leftButton;//左ボタン
    private Button rightButton;//右ボタン

    public override void OnInit()
    {
        //skinPath = "UI/Panel/RollingWindow";
        //layer = PanelManager.Layer.Panel;
    }


    //関数部分の書き換え
    public override void OnShow(params object[] para)
    {
        ////コンポーネントのバインディング
        //optionGroup = skin.transform.Find("Option").gameObject;
        //optionPrefab = optionGroup.transform.Find("Item").gameObject;
        //leftButton = skin.transform.Find("LeftButton").gameObject.GetComponent<Button>();
        //rightButton = skin.transform.Find("RightButton").gameObject.GetComponent<Button>();

        //コンポーネントのバインディング
        optionGroup = UITool.FindChildGameObject("Option");
        optionPrefab = UITool.FindChildGameObject("Item");
        leftButton = UITool.GetOrAddComponentInChildren<Button>("LeftButton");
        rightButton = UITool.GetOrAddComponentInChildren<Button>("RightButton");

        //リスナーを追加する
        leftButton.onClick.AddListener(TurnLeft);
        rightButton.onClick.AddListener(TurnRight);

        //データの初期化
        optionNUm = 3;
        yOffet = 20;
        speed = 8;
        minAlpha = 0.1f;
        minS = 0.5f;
        tempS = 0.5f;
        smoothSTime = 0.3f;
        firstS = 1.0f;
        originColor = Color.white;
        firstColor = Color.black;

        //各種の配列を初期化する
        SIE2 = new Coroutine[optionNUm];
        options = new Transform[optionNUm];
        border = new Image[optionNUm];

        //生成ロジック
        Generate();
    }

    public void Generate()
    {
        Debug.Log("サイクリックUIが生成されました");
        for (int i = 0; i < optionNUm - 1; i++)//-1は、最初のものがすでに生成され、プレファブテンプレートとして使用するためです。
        {
            GameObject go = GameObject.Instantiate(optionPrefab, Vector3.zero, Quaternion.identity, optionGroup.transform);
            go.name = i.ToString();
        }

        halfNum = optionNUm / 2;

        for (int i = 0; i < optionNUm; i++)
        {
            options[i] = optionGroup.transform.GetChild(i);
            border[i] = options[i].GetComponent<Image>();
            SetBorderColor(i, originColor);
        }

        InitPos();//初期位置
        InitSibling();//位置を整列する
        SetAlpha();//要素の透明度を設定する
        SetFristColor();//要素の色を設定する
        // MonoHelper.Instance.StartCoroutine(SetScale());//スケールを設定する、現在はこの機能は不要
    }

    public void InitPos()
    {
        float angle = 0;

        for (int i = 0; i < optionNUm; i++)
        {
            //対応する角度を設定する
            angle = (360.0f / (float)optionNUm) * i * Mathf.Deg2Rad;

            float x = Mathf.Sin(angle) * R;
            float z = -Mathf.Cos(angle) * R;

            //対応するオフセットを設定する
            float y = 0;
            if (i != 0)
            {
                y = i * yOffet;
                if (i > halfNum)
                {
                    y = (optionNUm - i) * yOffet;
                }
            }

            //位置と辞書の初期化
            Vector3 temp = options[i].localPosition = new Vector3(x, y, z);
            OptionP.Add(options[i], temp);
        }
    }

    public void InitSibling()
    {
        //順序を設定する
        for (int i = 0; i < optionNUm; i++)
        {
            //まだ半分も経っていません
            if (i <= halfNum)
            {
                //偶数
                if (optionNUm % 2 == 0)
                {
                    options[i].SetSiblingIndex((int)halfNum - i);
                }
                //奇数
                else
                {
                    options[i].SetSiblingIndex((int)((optionNUm - 1) / 2) - i);
                }
            }
            else//半分以上
            {
                options[i].SetSiblingIndex(options[optionNUm - i].GetSiblingIndex());
            }
        }
        //辞書に追加する
        for (int i = 0; i < optionNUm; i++)
        {
            OptionS.Add(options[i], options[i].GetSiblingIndex());
        }
    }

    //現在の選択肢のインデックスを取得する
    public int GetFirst()
    {
        for (int i = 0; i < optionNUm; i++)
        {
            if (options[i].GetSiblingIndex() == optionNUm - 1)
            {
                return i;
            }
        }
        //見つからない識別子
        return 233;
    }

    public void TurnLeft()
    {
        MonoHelper.Instance.StartCoroutine(MoveLeft());
    }

    public void TurnRight()
    {
        MonoHelper.Instance.StartCoroutine(MoveRight());

    }

    IEnumerator MoveToTarget(Transform tf, Vector3 target)
    {

        float tempspeed = (tf.localPosition - target).magnitude * speed;
        while (tf.localPosition != target)
        {
            tf.localPosition = Vector3.MoveTowards(tf.localPosition, target, tempspeed * Time.deltaTime);
            yield return null;
        }
        //辞書の位置を更新する
        OptionP[tf] = target;

        //透明度を設定する
        SetAlpha();

        yield return null;
    }

    IEnumerator MoveLeft()
    {
        //コルーチンの競合を避ける
        if (currentPIE != null)
        {
            yield return currentPIE;
        }

        //一時的にズーム機能をオフにします。
        // for(int i = 0;i<optionNUm;i++)
        // {
        //     if(SIE2[i] != null)
        //     {
        //         yield return SIE2[i];
        //     }
        // }

        //属性領域をリセット
        int first = GetFirst();
        SetBorderColor(first, originColor);
        // ReSetScale();

        Vector3 p = OptionP[options[0]];
        int s = OptionS[options[0]];

        Vector3 targetP;

        for (int i = 0; i < optionNUm; i++)
        {
            if (i == optionNUm - 1)
            {
                targetP = p;
                OptionS[options[i]] = s;
            }
            else
            {
                targetP = options[(i + 1) % optionNUm].localPosition;
                OptionS[options[i]] = OptionS[options[(i + 1) % optionNUm]];
            }

            options[i].SetSiblingIndex(OptionS[options[i]]);
            currentPIE = MonoHelper.Instance.StartCoroutine(MoveToTarget(options[i], targetP));
        }

        if (currentPIE != null)
        {
            yield return currentPIE;
        }

        // MonoHelper.Instance.StartCoroutine(SetScale());

        SetFristColor();//要素の色を設定する


        yield return null;

    }

    IEnumerator MoveRight()
    {
        if (currentPIE != null)
        {
            yield return currentPIE;
        }

        //一時的にズーム機能をオフにします。
        // for(int i = 0;i<optionNUm;i++)
        // {
        //     if(SIE2[i] != null)
        //     {
        //         yield return SIE2[i];
        //     }
        // }


        //属性領域をリセット
        int first = GetFirst();
        SetBorderColor(first, originColor);
        // ReSetScale();

        //情報の保存：最後
        Vector3 p = OptionP[options[optionNUm - 1]];
        int s = OptionS[options[optionNUm - 1]];
        Vector3 targetP;

        //最後のものからループを開始します
        for (int i = optionNUm - 1; i >= 0; i--)
        {
            if (i == 0)
            {
                //目標の移動位置を確定する
                targetP = p;
                OptionS[options[i]] = s;
            }
            else
            {
                targetP = options[(i - 1) % optionNUm].localPosition;
                OptionS[options[i]] = OptionS[options[(i - 1) % optionNUm]];
            }
            options[i].SetSiblingIndex(OptionS[options[i]]);
            currentPIE = MonoHelper.Instance.StartCoroutine(MoveToTarget(options[i], targetP));
        }


        // MonoHelper.Instance.StartCoroutine(SetScale());

        SetFristColor();//要素の色を設定する


        yield return null;
    }

    private void SetAlpha()//Z値に基づいて透明度を動的に生成する
    {
        //Z値を計算する7つのポイント、つまり現在の選択肢、透明度が最大です。
        float startz = center.z - R;
        foreach (var option in OptionP)
        {
            //透明度を計算する
            float val = 1 - Mathf.Abs(option.Value.z - startz) / (2 * R) * (1 - minAlpha);

            Image[] img = option.Key.GetComponentsInChildren<Image>();
            for (int i = 0; i < img.Length; i++)
            {
                Color c = img[i].color;
                img[i].color = new Color(c.r, c.g, c.b, val);
            }
        }
    }
    IEnumerator SetScale()
    {
        int first = GetFirst();
        float startz = center.z - R;

        for (int i = 0; i < optionNUm; i++)
        {
            //現在のオプションの拡大率は個別に設定されています。
            if (i == first)
            {
                SIE2[i] = MonoHelper.Instance.StartCoroutine(ChangeScale(options[i], firstS));
            }
            else
            {
                float val = 1 - Mathf.Abs(options[i].localPosition.z - startz) / (2 * R) * (1 - minS);
                options[i].localScale = Vector3.one * val;
                SIE2[i] = MonoHelper.Instance.StartCoroutine(ChangeScale(options[i], val));
            }
        }

        yield return null;
    }

    public void ReSetScale()
    {
        foreach (Transform tf in options)
        {
            tf.localScale = Vector3.one * tempS;
        }
    }

    // ズーム処理のコルーチン
    IEnumerator ChangeScale(Transform tf, float targetS)
    {
        float temp = 0;
        while (Mathf.Abs(tf.localScale.x - targetS) > 0.001)//同じスケールに到達できない可能性があるため、ここでは主にある範囲より小さいことが重要です。
        {
            float s = Mathf.SmoothDamp(tf.localScale.x, targetS, ref temp, smoothSTime);
            tf.localScale = Vector3.one * s;
            yield return null;
        }
        yield return null;
    }


    //全体を中央に表示させ、上下の空間の差が大きすぎるのを避けてください。
    IEnumerator AlignCenter()
    {
        //拡大縮小が完了したことを確認してください。
        //一時的にズーム機能をオフにします。
        // for(int i = 0;i<optionNUm;i++)
        // {
        //     if(SIE2[i] != null)
        //     {
        //         yield return SIE2[i];
        //     }
        // }

        float a = options[0].GetComponent<RectTransform>().rect.height * options[0].localScale.x / 2f;
        //偶数であれば
        if (optionNUm % 2 == 0)
        {
            float b = options[(int)halfNum].GetComponent<RectTransform>().rect.height * options[(int)halfNum].localScale.x / 2f;
            optionGroup.transform.localPosition = new Vector3(0, (-halfNum * yOffet + a - b) / 2f, 0);
        }
        //奇数であるなら
        else
        {
            int temp = (optionNUm - 1) / 2;
            float b = options[temp].GetComponent<RectTransform>().rect.height * options[temp].localScale.x / 2f;
            optionGroup.transform.localPosition = new Vector3(0, (-temp * yOffet + a - b) / 2f, 0);
        }

        yield return null;
    }

    private void SetBorderColor(int i, Color c)//色の設定方法
    {
        border[i].color = c;
    }

    private void SetFristColor()//最初の色を設定する
    {
        //枠線の色を設定する
        int first = GetFirst();
        SetBorderColor(first, firstColor);
    }
}
