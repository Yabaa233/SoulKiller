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

    public VolumeProfile volumeProfile;//得到对应的渲染组件
    public AnimationCurve clearCMCurve;//通关时相机移动曲线
    //缓存区
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
        // GameManager.Instance.cinemachine = this.cinemachine;    //向GameManger注册
        volumeProfile = GameObject.Find("Volume").GetComponent<Volume>().profile;

        volumeProfile.TryGet<Vignette>(out vignette);
        storeColor = vignette.color.value;
        startIntensity = vignette.intensity.value;

    }
    //镜头抖动API
    //参数:int type:抖屏类型 目前已有类型↓
    //                  0:随机 1:简单 2:水平随机 3:水平简单 4:竖直随机 5:竖直简单
    //                 （随机代表每一次抖动幅度随机，简单表示抖动幅度固定）
    //     float shake_time:抖动时间
    //     可选参数float Amp（0~N）:抖动的振幅（默认1）
    //     可选参数float Fre（0~N）:抖动的频率（默认1）
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
        float Amp_lerp = Amp;//当前振幅
        int frame = (int)(60 * shake_time);//总帧数
        float Amp_frame = Amp / frame;//每帧变化
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
        float timef = 0.1f;//时间单位
        float Amp_lerp = Amp;//当前振幅
        int frame = (int)(shake_time / timef);//总执行次数
        float Amp_frame = Amp / frame;//每次变化
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
    /// 将Bloom效果设置到
    /// </summary>
    /// <param name="intensity">强度值</param>
    /// <param name="needRevert">是否需要复原</param>
    /// <param name="keeptime">持续时间</param>
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
    /// 调整ColorAdjusting的颜色
    /// </summary>
    /// <param name="color">调整至该颜色</param>
    /// <param name="needRevert">是否需要还原效果</param>
    /// <param name="keeptime">效果持续时间</param>
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
    /// 设置相机跟随玩家
    /// </summary>
    /// <param name="playerTF"> 待跟随玩家物体的tf </param>
    public void SetFollwerPlayer(Transform playerTF)
    {
        CM_Effect.Instance.cinemachine.LookAt = playerTF;
        CM_Effect.Instance.cinemachine.Follow = playerTF;
    }


    /// <summary>
    /// 设置屏闪
    /// </summary>
    /// <param name="color">闪烁颜色</param>
    /// <param name="hurtSpeed">闪烁速度</param>
    /// <param name="maxIntensity">闪烁最大值，0~1</param>
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
        else//这里做覆盖操作
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
        // Debug.Log("已经还原");
        yield break;
    }

    /// <summary>
    /// 相机移动
    /// </summary>
    /// <param name="targetSize"> 目标正交尺寸 </param>
    /// <param name="time"> 期望到位时间 </param>
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
            // Debug.Log("镜头比例" + clearCMCurve.Evaluate(curTime / time));
            curTime += Time.deltaTime;
            yield return null;
        }
        yield break;
    }
}
