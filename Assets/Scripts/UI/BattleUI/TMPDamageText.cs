using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TMPDamageText : MonoBehaviour
{
    public TextMeshProUGUI damageText;//伤害数字
    public RectTransform rectTransform;//矩形绘制框
    [Header("文字存在时间")]public float lifeTimer = 3;//存在多久
    [Header("文字上升速度")]private float upSpeed = 5;//上升速度
    [Header("文字最小上升速度")] public float upMinSpeed = 5;//最小上升速度
    [Header("文字最大上升速度")] public float upMaxSpeed = 5;//最大上升速度

    [Header("文字上升速度曲线")] public AnimationCurve upSpeedCurve ;//上升速度曲线
    [Header("文字缩放速度曲线")] public AnimationCurve upScaleCurve;//缩放速度曲线
    [Header("文字变透明速度曲线")] public AnimationCurve upAlphaCurve;//透明度变换速度曲线

    [Header("暴击时文字的缩放倍数")]public float scaleMultible = 2;//暴击时文字缩放倍
    [Header("初始大小")]public Vector3 startSize = new Vector3(0.5f,0.5f,0.5f);
    public TMP_ColorGradient criticalPreset;
    public TMP_ColorGradient normalPreset;
    public TMP_ColorGradient playerColorPreset;

    private Vector3 storePos;//存储刚出生时的位置
    private float biasY;//存储Y轴位移
    private float curTime=0;//储存动画曲线采样时间

    //private Material myMat;
    //private Vector3 myColor;
    //private bool isTimeStop=false;//是否正在顿帧
    void Awake()
    {
        damageText = gameObject.GetComponent<TextMeshProUGUI>();
        rectTransform = gameObject.GetComponent<RectTransform>();
        
    }

    private void Start() {
        // rectTransform.localScale = new Vector3(1f,1f,1f);
        // damageText.colorGradientPreset = normalPreset;
        // Invoke("RecycleObj",lifeTimer);
        //myMat = GetComponent<Material>();
        //myColor = new Vector3(myMat.color.r, myMat.color.g, myMat.color.b);
    }

    private void OnEnable() {
        rectTransform.localScale = startSize;
        damageText.colorGradientPreset = normalPreset;
        biasY = transform.position.y;
        damageText.alpha = 1;
        curTime = 0;
        rectTransform.localScale = startSize;
        Invoke("RecycleObj",lifeTimer);
        upSpeed =UnityEngine.Random.Range(upMinSpeed, upMaxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.Player_IsStop)
        {
        biasY += upSpeed*upSpeedCurve.Evaluate(curTime) * Time.deltaTime;
        //damageText.colorGradient..

        transform.position = new Vector3(storePos.x,biasY,storePos.z);
        curTime += Time.deltaTime;

            damageText.alpha =  upAlphaCurve.Evaluate(curTime);
        }
        else
        {
            biasY += 0;
            //damageText.colorGradient..

            transform.position = new Vector3(storePos.x, biasY, storePos.z);
            curTime += 0;
        }
    }

    public void SetDamage(float _damage,bool isCritical)
    {
        //damage = _damage;
        if (_damage < 50)
        {
            scaleMultible = 1.5f;
        }
        else if (_damage < 200)
        {
            scaleMultible = 2f;
        }
        else 
        {
            scaleMultible = 3f;
        }
        damageText.text = _damage.ToString("0");
        if(isCritical||_damage>100)
        {
            damageText.colorGradientPreset = criticalPreset;
            //rectTransform.localScale = scaleMultible * startSize;
            StartCoroutine(ChangeScale());
        }
    }

    private IEnumerator ChangeScale()
    {
        float allChangeScale = scaleMultible - 1;
        float nowChangeScale = 1;
        float time = 0;
        //print("暴击飘字缩放");
        while (time<1)
        {
            if (!GameManager.Instance.Player_IsStop)
            {
            rectTransform.localScale = nowChangeScale * startSize;
            float sampleRate = upScaleCurve.Evaluate(time);
            nowChangeScale = 1+sampleRate * allChangeScale;
            time += Time.deltaTime;

            }
            else
            {
                rectTransform.localScale = nowChangeScale * startSize;
                float sampleRate = upScaleCurve.Evaluate(time);
                nowChangeScale = 1 + sampleRate * allChangeScale;
                time += 0;
            }
            // print(sampleRate);

            yield return null;
        }
    }
    public void SetStorePos(Vector3 _store)//记录初始位置
    {
        storePos = _store;
    }

    public void SetColorGradiant(TMP_ColorGradient colorGradient)//设置颜色渐变
    {
        damageText.colorGradientPreset = colorGradient;
    }

    public void PlayerPreset(bool state)//使用玩家预设
    {
        //目前只有颜色更改
        if(state)
        {
            SetColorGradiant(playerColorPreset);
        }
    }


    public void RecycleObj()
    {
        ObjectPool.Instance.RecycleObj("DamageText", gameObject);
    }
}