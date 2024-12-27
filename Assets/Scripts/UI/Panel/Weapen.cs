using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ����UI��������BattleMainPanel��������ʾ���л���ǰ������
/// xushi
/// </summary>
public class Weapen : BasePanel
{
    static readonly string path = "UI/Panel/Weapen(RollingWindow";

    public Weapen() : base(new UIType(path)) { }

    //ѡ��Ԥ����
    private GameObject optionPrefab;
    //ѡ����ĸ�����
    private GameObject optionGroup;
    //ѡ����
    private Transform[] options;
    //ѡ������
    [Range(0, 20)]
    private int optionNUm;
    //ѡ��������һ��
    private float halfNum;
    //ѡ���ѡ��λ��
    private Dictionary<Transform, Vector3> OptionP = new Dictionary<Transform, Vector3>();
    //�����㼶˳����ֵ�
    private Dictionary<Transform, int> OptionS = new Dictionary<Transform, int>();
    //��ת����
    private Vector3 center = Vector3.zero;
    //��ת�뾶
    private float R = 50f;
    //��ת�ٶ�
    [Range(1f, 100f)]
    private float speed;
    //y���ƫ��������ÿһ��Ԫ�ض��ܱ�����
    private float yOffet;
    //��С͸����
    [Range(0, 1)]
    public float minAlpha;

    //ѡ��������ų̶�
    [Range(1, 5)]
    public float firstS;
    [Range(0, 1)]
    public float minS;//��С���ų̶�
    [Range(0, 1)]
    public float tempS;//��ת�����е����ų̶�
    [Range(0, 0.5f)]
    public float smoothSTime;//���ŵ�ƽ��ʱ��

    Coroutine currentPIE;//��ǰ�����ƶ���Э��
    Coroutine[] SIE2;//���е�����Э��

    //�߿���ɫ����
    Image[] border;//ѡ��߿�
    [ColorUsage(true, false)]//��һ������Ϊ�Ƿ���ʾAlphaͨ�����ڶ�������Ϊ�Ƿ���HDR
    public Color originColor;//��ʼ�߿���ɫ
    [ColorUsage(true, false)]
    public Color firstColor;//ѡ�б߿����ɫ

    private Button leftButton;//��ť
    private Button rightButton;//�Ұ�ť

    public override void OnInit()
    {
        //skinPath = "UI/Panel/RollingWindow";
        //layer = PanelManager.Layer.Panel;
    }


    //��д��������
    public override void OnShow(params object[] para)
    {
        ////�����
        //optionGroup = skin.transform.Find("Option").gameObject;
        //optionPrefab = optionGroup.transform.Find("Item").gameObject;
        //leftButton = skin.transform.Find("LeftButton").gameObject.GetComponent<Button>();
        //rightButton = skin.transform.Find("RightButton").gameObject.GetComponent<Button>();

        //�����
        optionGroup = UITool.FindChildGameObject("Option");
        optionPrefab = UITool.FindChildGameObject("Item");
        leftButton = UITool.GetOrAddComponentInChildren<Button>("LeftButton");
        rightButton = UITool.GetOrAddComponentInChildren<Button>("RightButton");

        //��Ӽ���
        leftButton.onClick.AddListener(TurnLeft);
        rightButton.onClick.AddListener(TurnRight);

        //���ݳ�ʼ��
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

        //��ʼ����������
        SIE2 = new Coroutine[optionNUm];
        options = new Transform[optionNUm];
        border = new Image[optionNUm];

        //�����߼�
        Generate();
    }

    public void Generate()
    {
        Debug.Log("������ѭ��UI");
        for (int i = 0; i < optionNUm - 1; i++)//-1����Ϊ��һ���Ѿ������ˣ���������Ԥ����ģ��
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

        InitPos();//��ʼ��λ��
        InitSibling();//��λ�ý�������
        SetAlpha();//����Ԫ��͸����
        SetFristColor();//����Ԫ����ɫ
        // MonoHelper.Instance.StartCoroutine(SetScale());//�������ű�����С����ʱ����Ҫ�������
    }

    public void InitPos()
    {
        float angle = 0;

        for (int i = 0; i < optionNUm; i++)
        {
            //���ö�Ӧ�ĽǶ�
            angle = (360.0f / (float)optionNUm) * i * Mathf.Deg2Rad;

            float x = Mathf.Sin(angle) * R;
            float z = -Mathf.Cos(angle) * R;

            //���ö�Ӧ��ƫ����
            float y = 0;
            if (i != 0)
            {
                y = i * yOffet;
                if (i > halfNum)
                {
                    y = (optionNUm - i) * yOffet;
                }
            }

            //��ʼ��λ�ú��ֵ�
            Vector3 temp = options[i].localPosition = new Vector3(x, y, z);
            OptionP.Add(options[i], temp);
        }
    }

    public void InitSibling()
    {
        //����˳��
        for (int i = 0; i < optionNUm; i++)
        {
            //û�й���
            if (i <= halfNum)
            {
                //ż��
                if (optionNUm % 2 == 0)
                {
                    options[i].SetSiblingIndex((int)halfNum - i);
                }
                //����
                else
                {
                    options[i].SetSiblingIndex((int)((optionNUm - 1) / 2) - i);
                }
            }
            else//����
            {
                options[i].SetSiblingIndex(options[optionNUm - i].GetSiblingIndex());
            }
        }
        //��ӵ��ֵ�
        for (int i = 0; i < optionNUm; i++)
        {
            OptionS.Add(options[i], options[i].GetSiblingIndex());
        }
    }

    //��ȡ��ǰѡ��Index
    public int GetFirst()
    {
        for (int i = 0; i < optionNUm; i++)
        {
            if (options[i].GetSiblingIndex() == optionNUm - 1)
            {
                return i;
            }
        }
        //û���ҵ��ı�ʶ
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
        //�����ֵ����λ��
        OptionP[tf] = target;

        //����͸����
        SetAlpha();

        yield return null;
    }

    IEnumerator MoveLeft()
    {
        //����Э�̳�ͻ
        if (currentPIE != null)
        {
            yield return currentPIE;
        }

        //��ʱ���������Ź���
        // for(int i = 0;i<optionNUm;i++)
        // {
        //     if(SIE2[i] != null)
        //     {
        //         yield return SIE2[i];
        //     }
        // }

        //������������
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

        SetFristColor();//����Ԫ����ɫ


        yield return null;

    }

    IEnumerator MoveRight()
    {
        if (currentPIE != null)
        {
            yield return currentPIE;
        }

        //��ʱ���������Ź���
        // for(int i = 0;i<optionNUm;i++)
        // {
        //     if(SIE2[i] != null)
        //     {
        //         yield return SIE2[i];
        //     }
        // }


        //������������
        int first = GetFirst();
        SetBorderColor(first, originColor);
        // ReSetScale();

        //�洢��Ϣ�����
        Vector3 p = OptionP[options[optionNUm - 1]];
        int s = OptionS[options[optionNUm - 1]];
        Vector3 targetP;

        //�����һ����ʼѭ��
        for (int i = optionNUm - 1; i >= 0; i--)
        {
            if (i == 0)
            {
                //ȷ��Ŀ����ƶ�λ��
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

        SetFristColor();//����Ԫ����ɫ


        yield return null;
    }

    private void SetAlpha()//����Zֵ��̬����͸����
    {
        //����Zֵ���ߵ㣬����ǰѡ�͸�������
        float startz = center.z - R;
        foreach (var option in OptionP)
        {
            //����͸����
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
            //��ǰѡ��ķŴ����ǵ������õ�
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

    // ���Ŵ����Э��
    IEnumerator ChangeScale(Transform tf, float targetS)
    {
        float temp = 0;
        while (Mathf.Abs(tf.localScale.x - targetS) > 0.001)//�����п�����Զ���޷��ﵽ��ͬ�����ų̶ȣ�����������Ҫ��С��ĳ����Χ����
        {
            float s = Mathf.SmoothDamp(tf.localScale.x, targetS, ref temp, smoothSTime);
            tf.localScale = Vector3.one * s;
            yield return null;
        }
        yield return null;
    }


    //�����������ʾ���������¿ռ������
    IEnumerator AlignCenter()
    {
        //ȷ�������Ѿ������
        //��ʱ���������Ź���
        // for(int i = 0;i<optionNUm;i++)
        // {
        //     if(SIE2[i] != null)
        //     {
        //         yield return SIE2[i];
        //     }
        // }

        float a = options[0].GetComponent<RectTransform>().rect.height * options[0].localScale.x / 2f;
        //���Ϊż���Ļ�
        if (optionNUm % 2 == 0)
        {
            float b = options[(int)halfNum].GetComponent<RectTransform>().rect.height * options[(int)halfNum].localScale.x / 2f;
            optionGroup.transform.localPosition = new Vector3(0, (-halfNum * yOffet + a - b) / 2f, 0);
        }
        //���������
        else
        {
            int temp = (optionNUm - 1) / 2;
            float b = options[temp].GetComponent<RectTransform>().rect.height * options[temp].localScale.x / 2f;
            optionGroup.transform.localPosition = new Vector3(0, (-temp * yOffet + a - b) / 2f, 0);
        }

        yield return null;
    }

    private void SetBorderColor(int i, Color c)//������ɫ�ķ���
    {
        border[i].color = c;
    }

    private void SetFristColor()//���õ�һ����ɫ
    {
        //���ñ߿���ɫ
        int first = GetFirst();
        SetBorderColor(first, firstColor);
    }
}
