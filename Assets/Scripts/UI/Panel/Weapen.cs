using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 环形UI，挂载在BattleMainPanel上用来显示并切换当前的武器
/// xushi
/// </summary>
public class Weapen : BasePanel
{
    static readonly string path = "UI/Panel/Weapen(RollingWindow";

    public Weapen() : base(new UIType(path)) { }

    //选项预制体
    private GameObject optionPrefab;
    //选项组的父对象
    private GameObject optionGroup;
    //选项组
    private Transform[] options;
    //选项总数
    [Range(0, 20)]
    private int optionNUm;
    //选项总数的一半
    private float halfNum;
    //选项和选项位置
    private Dictionary<Transform, Vector3> OptionP = new Dictionary<Transform, Vector3>();
    //调整层级顺序的字典
    private Dictionary<Transform, int> OptionS = new Dictionary<Transform, int>();
    //旋转中心
    private Vector3 center = Vector3.zero;
    //旋转半径
    private float R = 50f;
    //旋转速度
    [Range(1f, 100f)]
    private float speed;
    //y轴的偏移量，让每一个元素都能被看到
    private float yOffet;
    //最小透明度
    [Range(0, 1)]
    public float minAlpha;

    //选中项的缩放程度
    [Range(1, 5)]
    public float firstS;
    [Range(0, 1)]
    public float minS;//最小缩放程度
    [Range(0, 1)]
    public float tempS;//旋转过程中的缩放程度
    [Range(0, 0.5f)]
    public float smoothSTime;//缩放的平滑时间

    Coroutine currentPIE;//当前正在移动的协程
    Coroutine[] SIE2;//所有的缩放协程

    //边框颜色部分
    Image[] border;//选项边框
    [ColorUsage(true, false)]//第一个参数为是否显示Alpha通道，第二个参数为是否开启HDR
    public Color originColor;//初始边框颜色
    [ColorUsage(true, false)]
    public Color firstColor;//选中边框的颜色

    private Button leftButton;//左按钮
    private Button rightButton;//右按钮

    public override void OnInit()
    {
        //skinPath = "UI/Panel/RollingWindow";
        //layer = PanelManager.Layer.Panel;
    }


    //重写函数部分
    public override void OnShow(params object[] para)
    {
        ////组件绑定
        //optionGroup = skin.transform.Find("Option").gameObject;
        //optionPrefab = optionGroup.transform.Find("Item").gameObject;
        //leftButton = skin.transform.Find("LeftButton").gameObject.GetComponent<Button>();
        //rightButton = skin.transform.Find("RightButton").gameObject.GetComponent<Button>();

        //组件绑定
        optionGroup = UITool.FindChildGameObject("Option");
        optionPrefab = UITool.FindChildGameObject("Item");
        leftButton = UITool.GetOrAddComponentInChildren<Button>("LeftButton");
        rightButton = UITool.GetOrAddComponentInChildren<Button>("RightButton");

        //添加监听
        leftButton.onClick.AddListener(TurnLeft);
        rightButton.onClick.AddListener(TurnRight);

        //数据初始化
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

        //初始化各种数组
        SIE2 = new Coroutine[optionNUm];
        options = new Transform[optionNUm];
        border = new Image[optionNUm];

        //生成逻辑
        Generate();
    }

    public void Generate()
    {
        Debug.Log("生成了循环UI");
        for (int i = 0; i < optionNUm - 1; i++)//-1是因为第一个已经生成了，用来当作预制体模板
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

        InitPos();//初始化位置
        InitSibling();//对位置进行排列
        SetAlpha();//设置元素透明度
        SetFristColor();//设置元素颜色
        // MonoHelper.Instance.StartCoroutine(SetScale());//设置缩放比例大小，暂时不需要这个功能
    }

    public void InitPos()
    {
        float angle = 0;

        for (int i = 0; i < optionNUm; i++)
        {
            //设置对应的角度
            angle = (360.0f / (float)optionNUm) * i * Mathf.Deg2Rad;

            float x = Mathf.Sin(angle) * R;
            float z = -Mathf.Cos(angle) * R;

            //设置对应的偏移量
            float y = 0;
            if (i != 0)
            {
                y = i * yOffet;
                if (i > halfNum)
                {
                    y = (optionNUm - i) * yOffet;
                }
            }

            //初始化位置和字典
            Vector3 temp = options[i].localPosition = new Vector3(x, y, z);
            OptionP.Add(options[i], temp);
        }
    }

    public void InitSibling()
    {
        //设置顺序
        for (int i = 0; i < optionNUm; i++)
        {
            //没有过半
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
            else//过半
            {
                options[i].SetSiblingIndex(options[optionNUm - i].GetSiblingIndex());
            }
        }
        //添加到字典
        for (int i = 0; i < optionNUm; i++)
        {
            OptionS.Add(options[i], options[i].GetSiblingIndex());
        }
    }

    //获取当前选项Index
    public int GetFirst()
    {
        for (int i = 0; i < optionNUm; i++)
        {
            if (options[i].GetSiblingIndex() == optionNUm - 1)
            {
                return i;
            }
        }
        //没有找到的标识
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
        //更新字典里的位置
        OptionP[tf] = target;

        //设置透明度
        SetAlpha();

        yield return null;
    }

    IEnumerator MoveLeft()
    {
        //避免协程冲突
        if (currentPIE != null)
        {
            yield return currentPIE;
        }

        //暂时不开启缩放功能
        // for(int i = 0;i<optionNUm;i++)
        // {
        //     if(SIE2[i] != null)
        //     {
        //         yield return SIE2[i];
        //     }
        // }

        //重置属性区域
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

        SetFristColor();//设置元素颜色


        yield return null;

    }

    IEnumerator MoveRight()
    {
        if (currentPIE != null)
        {
            yield return currentPIE;
        }

        //暂时不开启缩放功能
        // for(int i = 0;i<optionNUm;i++)
        // {
        //     if(SIE2[i] != null)
        //     {
        //         yield return SIE2[i];
        //     }
        // }


        //重置属性区域
        int first = GetFirst();
        SetBorderColor(first, originColor);
        // ReSetScale();

        //存储信息：最后
        Vector3 p = OptionP[options[optionNUm - 1]];
        int s = OptionS[options[optionNUm - 1]];
        Vector3 targetP;

        //从最后一个开始循环
        for (int i = optionNUm - 1; i >= 0; i--)
        {
            if (i == 0)
            {
                //确定目标的移动位置
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

        SetFristColor();//设置元素颜色


        yield return null;
    }

    private void SetAlpha()//根据Z值动态生成透明度
    {
        //计算Z值的七点，即当前选项，透明度最大
        float startz = center.z - R;
        foreach (var option in OptionP)
        {
            //计算透明度
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
            //当前选项的放大率是单独设置的
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

    // 缩放处理的协程
    IEnumerator ChangeScale(Transform tf, float targetS)
    {
        float temp = 0;
        while (Mathf.Abs(tf.localScale.x - targetS) > 0.001)//由于有可能永远都无法达到相同的缩放程度，所以这里主要是小于某个范围就行
        {
            float s = Mathf.SmoothDamp(tf.localScale.x, targetS, ref temp, smoothSTime);
            tf.localScale = Vector3.one * s;
            yield return null;
        }
        yield return null;
    }


    //让整体居中显示，避免上下空间差距过大
    IEnumerator AlignCenter()
    {
        //确保缩放已经完成了
        //暂时不开启缩放功能
        // for(int i = 0;i<optionNUm;i++)
        // {
        //     if(SIE2[i] != null)
        //     {
        //         yield return SIE2[i];
        //     }
        // }

        float a = options[0].GetComponent<RectTransform>().rect.height * options[0].localScale.x / 2f;
        //如果为偶数的话
        if (optionNUm % 2 == 0)
        {
            float b = options[(int)halfNum].GetComponent<RectTransform>().rect.height * options[(int)halfNum].localScale.x / 2f;
            optionGroup.transform.localPosition = new Vector3(0, (-halfNum * yOffet + a - b) / 2f, 0);
        }
        //如果是奇数
        else
        {
            int temp = (optionNUm - 1) / 2;
            float b = options[temp].GetComponent<RectTransform>().rect.height * options[temp].localScale.x / 2f;
            optionGroup.transform.localPosition = new Vector3(0, (-temp * yOffet + a - b) / 2f, 0);
        }

        yield return null;
    }

    private void SetBorderColor(int i, Color c)//设置颜色的方法
    {
        border[i].color = c;
    }

    private void SetFristColor()//设置第一个颜色
    {
        //设置边框颜色
        int first = GetFirst();
        SetBorderColor(first, firstColor);
    }
}
