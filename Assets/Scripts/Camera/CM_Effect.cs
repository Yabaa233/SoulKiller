using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public enum shake_type
{
    random,
    simple,
    hor_random,
    hor_simple,
    ver_random,
    ver_simple
}

public class CM_Effect : singleton<CM_Effect>
{
    public List<Cinemachine.NoiseSettings> m_NoiseProfiles;
    public Cinemachine.CinemachineVirtualCamera cinemachine;
    public Cinemachine.CinemachineBasicMultiChannelPerlin config;

    public VolumeProfile volumeProfile;//対応するレンダリングコンポーネントを取得する
    public AnimationCurve clearCMCurve;//クリア時のカメラ移動曲線
    //バッファエリア
    private float bloomIntensity;
    private ColorParameter colorParameter;
    private ClampedFloatParameter clampedFloatParameter;
    private Coroutine damageCoroutine;
    private Vignette vignette;
    Color storeColor;
    float startIntensity;

    protected override void Awake()
    {
        base.Awake();
        cinemachine = GetComponent<Cinemachine.CinemachineVirtualCamera>();
        config = cinemachine.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }
    private void Start()
    {
        // GameManager.Instance.cinemachine = this.cinemachine;    //GameManagerに登録する
        volumeProfile = GameObject.Find("Volume").GetComponent<Volume>().profile;

        volumeProfile.TryGet<Vignette>(out vignette);
        storeColor = vignette.color.value;
        startIntensity = vignette.intensity.value;

    }
    //カメラシェイクAPI
    //パラメータ：int type：画面揺れタイプ 現在のタイプ↓
    //                  0:ランダム 1:シンプル 2:水平ランダム 3:水平シンプル 4:垂直ランダム 5:垂直シンプル
    //                 （ランダムは振動幅度が毎回ランダムで、シンプルは振動幅度が固定されていることを示します）
    //     float shake_time：振動時間
    //     オプションパラメータfloat Amp（0〜N）：振幅の揺れ（デフォルトは1）
    //     オプションパラメータfloat Fre（0〜N）：振動の頻度（デフォルトは1）
    public void CM_do_shake(shake_type type, float shake_time, float Amp = 1, float Fre = 1)
    {
        // var cinemachine = GetComponent<Cinemachine.CinemachineVirtualCamera>();
        // var config = cinemachine.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();

        StopCoroutine(shake_timer2(config, shake_time, Amp, Fre));

        config.m_AmplitudeGain = Amp;
        config.m_FrequencyGain = Fre;
        config.m_NoiseProfile = m_NoiseProfiles[((int)type)];

        StartCoroutine(shake_timer2(config, shake_time, Amp, Fre));
    }
    IEnumerator shake_timer(Cinemachine.CinemachineBasicMultiChannelPerlin config, float shake_time, float Amp, float Fre)
    {
        float Amp_lerp = Amp;//現在の振幅
        int frame = (int)(60 * shake_time);//総フレーム数
        float Amp_frame = Amp / frame;//各フレームの変化
        while (frame > 0)
        {
            yield return null;
            if (Amp_lerp > 0)
                Amp_lerp -= Amp_frame;
            else
                Amp_lerp = 0;
            config.m_AmplitudeGain = Amp_lerp;
            frame -= 1;
        }
        Amp_lerp = 0;
        config.m_NoiseProfile = null;
    }
    IEnumerator shake_timer2(Cinemachine.CinemachineBasicMultiChannelPerlin config, float shake_time, float Amp, float Fre)
    {
        float timef = 0.1f;//時間単位
        float Amp_lerp = Amp;//現在の振幅
        int frame = (int)(shake_time / timef);//合計実行回数
        float Amp_frame = Amp / frame;//毎回の変化
        while (frame > 0)
        {
            yield return new WaitForSeconds(timef);
            if (Amp_lerp > 0)
                Amp_lerp -= Amp_frame;
            else
                Amp_lerp = 0;
            config.m_AmplitudeGain = Amp_lerp;
            frame -= 1;
        }
        Amp_lerp = 0;
        config.m_NoiseProfile = null;
    }

    /// <summary>
    /// ブルーム効果を設定する
    /// </summary>
    /// <param name="intensity">強度値</param>
    /// <param name="needRevert">元に戻す必要があるか</param>
    /// <param name="keeptime">持続時間</param>
    public void SetBloomIntensity(float intensity, bool needRevert = false, float keeptime = 1)
    {
        Bloom bloom;
        volumeProfile.TryGet<Bloom>(out bloom);
        bloomIntensity = bloom.intensity.value;
        bloom.intensity.value = intensity;

        if (needRevert)
        {
            Invoke("RevertBloomIntensity", keeptime);
        }
    }

    private void RevertBloomIntensity()
    {
        Bloom bloom;
        volumeProfile.TryGet<Bloom>(out bloom);
        bloom.intensity.value = bloomIntensity;
    }

    /// <summary>
    /// ColorAdjustingの色を調整する
    /// </summary>
    /// <param name="color">この色に調整する</param>
    /// <param name="needRevert">効果を元に戻す必要があるかどうか</param>
    /// <param name="keeptime">効果持続時間</param>
    public void SetColorAdjusting(float intensity, bool needRevert = false, float keeptime = 1)
    {
        ColorAdjustments colorAdjustments;
        volumeProfile.TryGet<ColorAdjustments>(out colorAdjustments);
        // colorParameter = colorAdjustments.colorFilter;
        // ColorParameter realColorParameter = new ColorParameter(color);
        // colorAdjustments.colorFilter = realColorParameter;
        colorAdjustments.postExposure.value = intensity;
        // colorAdjustments.colorFilter.Interp(color,endcolor,percent);

        if (needRevert)
        {
            Invoke("RevertColorAdjusting", keeptime);
        }
    }

    private void RevertColorAdjusting()
    {
        ColorAdjustments colorAdjustments;
        volumeProfile.TryGet<ColorAdjustments>(out colorAdjustments);
        colorAdjustments.colorFilter = colorParameter;
    }


    public void SetVignette(float _intensity, bool needRevert = false, float keeptime = 1)
    {
        Vignette vignette;
        volumeProfile.TryGet<Vignette>(out vignette);
        vignette.intensity.value = _intensity;

        if (needRevert)
        {
            Invoke("RevertVignette", keeptime);
        }
    }

    private void RevertVignette()
    {
        Vignette vignette;
        volumeProfile.TryGet<Vignette>(out vignette);
        vignette.intensity = clampedFloatParameter;
    }

    /// <summary>
    /// カメラをプレイヤーに追従させる設定
    /// </summary>
    /// <param name="playerTF"> フォローすべきプレイヤーオブジェクトのtf </param>
    public void SetFollwerPlayer(Transform playerTF)
    {
        CM_Effect.Instance.cinemachine.LookAt = playerTF;
        CM_Effect.Instance.cinemachine.Follow = playerTF;
    }


    ///<summary>



    ////// 画面の点滅を設定する



    ///</summary>
    /// <param name="color">点滅色</param>
    /// <param name="hurtSpeed">点滅速度</param>
    /// <param name="maxIntensity">点滅の最大値、0〜1</param>
    public void PlayerGetDamaged(Color color,float hurtSpeed,float maxIntensity)
    {
        
        if(maxIntensity > 0.99f)
        {
            maxIntensity = 0.99f;
        }
        vignette.color.value = color;
        vignette.intensity.value = startIntensity;
        // Debug.Log(maxIntensity);
        if(damageCoroutine == null)
        {
            damageCoroutine = StartCoroutine(VignetteDamageBack(hurtSpeed,maxIntensity,vignette));
        }
        else//ここでカバー操作を行います。
        {
            StopCoroutine(damageCoroutine);
            // vignette.color.value = color;
            vignette.intensity.value = startIntensity;
            damageCoroutine = StartCoroutine(VignetteDamageBack(hurtSpeed,maxIntensity,vignette));
        }
    }


    // IEnumerator VignetteDamageGo(float hurtSpeed,float maxIntensity,Vignette vignette)
    // {
    //     while(vignette.intensity.value <=maxIntensity)
    //     { 
    //         vignette.intensity.value += hurtSpeed * Time.deltaTime;
    //         yield return null;
    //     }
    //     yield break;
    // }

    IEnumerator VignetteDamageBack(float hurtSpeed,float maxIntensity,Vignette vignette)
    {
        if(damageCoroutine !=null)
        {
            yield return damageCoroutine;
        }
        while(vignette.intensity.value <=maxIntensity)
        { 
            vignette.intensity.value += hurtSpeed * Time.deltaTime;
            yield return null;
        }
        while(vignette.intensity.value>startIntensity)
        {
            vignette.intensity.value -= hurtSpeed * Time.deltaTime;
            // Debug.Log(vignette.intensity.value);
            yield return null;
        }
        vignette.color.value = storeColor;
        vignette.intensity.value = startIntensity;
        // Debug.Log("It has already been restored.");
        yield break;
    }

    /// <summary>
    /// カメラ移動
    /// </summary>
    /// <param name="targetSize"> 目標の直交サイズ </param>
    /// <param name="time"> 到着予定時間 </param>
    /// <returns></returns>
    public void CM_TransitionDim(float targetSize, float time)
    {
        StopCoroutine("CM_TransitionDim");
        StartCoroutine(IE_CM_TransitionDim(targetSize, time));
    }
    IEnumerator IE_CM_TransitionDim(float targetSize, float time)
    {
        float curTime = 0;
        float firstSize = cinemachine.m_Lens.OrthographicSize;
        float setp = (targetSize - firstSize) / time;
        while (curTime < time)
        {
            cinemachine.m_Lens.OrthographicSize += Time.deltaTime * setp * clearCMCurve.Evaluate(curTime / time);
            // Debug.Log("カメラの比率" + clearCMCurve.Evaluate(curTime / time));
            curTime += Time.deltaTime;
            yield return null;
        }
        yield break;
    }
}
