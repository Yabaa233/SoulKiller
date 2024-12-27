using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable, VolumeComponentMenu("MSPost/HeightFog")]
public class HeightFogVolume : VolumeComponent, IPostProcessComponent
{
    [Tooltip("是否开启效果")]
    public BoolParameter EnableEffect = new BoolParameter(false);

    [Tooltip("雾效噪波")]
    public TextureParameter NoiseTex = new TextureParameter(null);
    
    [Tooltip("雾起始高度")]
    public FloatParameter FogStartHeight = new FloatParameter(0f);

    [Tooltip("雾高度")]
    public FloatParameter FogEndHeight = new FloatParameter(10f);

    [Range(0, 1), Tooltip("雾强度")]
    public FloatParameter FogIntensity = new FloatParameter(0.5f);

    [Tooltip("雾颜色")]
    public ColorParameter FogColor = new ColorParameter(Color.white);

    public bool IsActive() => EnableEffect == true;

    public bool IsTileCompatible() => false;

}