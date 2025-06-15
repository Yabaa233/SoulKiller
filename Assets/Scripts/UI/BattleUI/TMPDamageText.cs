using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TMPDamageText : MonoBehaviour
{
    public TextMeshProUGUI damageText;//ダメージ数値
    public RectTransform rectTransform;//長方形描画ボックス
    [Header("文字の存在時間")]public float lifeTimer = 3;//どのくらい存在していますか？
    [Header("テキストの上昇速度")]private float upSpeed = 5;//上昇速度
    [Header("テキストの最小上昇速度")] public float upMinSpeed = 5;//最小上昇速度
    [Header("テキストの最大上昇速度")] public float upMaxSpeed = 5;//最大上昇速度

    [Header("テキストの上昇速度曲線")] public AnimationCurve upSpeedCurve ;//上昇速度曲線
    [Header("テキストのズーム速度カーブ")] public AnimationCurve upScaleCurve;//ズーム速度カーブ
    [Header("テキストの透明度変化速度カーブ")] public AnimationCurve upAlphaCurve;//透明度の変化速度の曲線

    [Header("クリティカルヒット時のテキストの拡大倍率")]public float scaleMultible = 2;//クリティカルヒット時のテキストスケール倍数
    [Header("初期サイズ")]public Vector3 startSize = new Vector3(0.5f,0.5f,0.5f);
    public TMP_ColorGradient criticalPreset;
    public TMP_ColorGradient normalPreset;
    public TMP_ColorGradient playerColorPreset;

    private Vector3 storePos;//生まれた時の位置を保存する
    private float biasY;//Y軸の変位を保存します
    private float curTime=0;//アニメーションカーブのサンプリング時間を保存します

    //private Material myMat;
    //private Vector3 myColor;
    //private bool isTimeStop=false;//フレームが停止しているかどうか
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
        //print("クリティカルヒットのフローティングテキストのスケーリング");
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
    public void SetStorePos(Vector3 _store)//初期位置を記録する
    {
        storePos = _store;
    }

    public void SetColorGradiant(TMP_ColorGradient colorGradient)//グラデーション色を設定する
    {
        damageText.colorGradientPreset = colorGradient;
    }

    public void PlayerPreset(bool state)//プレイヤーのプリセットを使用する
    {
        //現在、色の変更のみが可能です。
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