using UnityEngine;
using UnityEngine.Rendering;

public class HeightFogController : MonoBehaviour
{
    [Tooltip("是否开启效果")]
    public bool EnableEffect;

    [Tooltip("雾效噪波")]
    public Texture NoiseTex;
    
    [Tooltip("雾起始高度")]
    public float FogStartHeight = 1f;

    [Tooltip("雾高度")]
    public float FogEndHeight  = 0.2f;

    [Range(0, 1), Tooltip("雾强度")]
    public float FogIntensity = 1f;

    [Tooltip("雾颜色")]
    public Color FogColor = Color.white;
    
    public VolumeProfile volumeProfile;
    HeightFogVolume heightFog;

    void Update()
    {
        if (volumeProfile == null) return;
        if (heightFog == null) volumeProfile.TryGet<HeightFogVolume>(out heightFog);
        if (heightFog == null) return;

        heightFog.EnableEffect.value = EnableEffect;
        heightFog.NoiseTex.value = NoiseTex;
        heightFog.FogStartHeight.value = FogStartHeight;
        heightFog.FogEndHeight.value = FogEndHeight;
        heightFog.FogIntensity.value = FogIntensity;
        heightFog.FogColor.value = FogColor;
    }
}