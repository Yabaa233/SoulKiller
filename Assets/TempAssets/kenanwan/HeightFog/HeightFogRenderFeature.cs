using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HeightFogRenderFeature : ScriptableRendererFeature
{
    HeightFogPass pass;
    public override void Create()
    {
        pass = new HeightFogPass(RenderPassEvent.BeforeRenderingPostProcessing);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        pass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(pass);
    }
}

public class HeightFogPass : ScriptableRenderPass
{
    static readonly string k_RenderTag = "HeightFogPass Effects";
    static readonly int MainTexId = Shader.PropertyToID("_MainTex");
    static readonly int TempTargetId = Shader.PropertyToID("_TempTarget");
    static readonly int FogNoiseId = Shader.PropertyToID("_FogNoiseTex");
    static readonly int FogStartHeightId = Shader.PropertyToID("_FogStartHeight");
    static readonly int FogEndHeightId = Shader.PropertyToID("_FogEndHeight");
    static readonly int FogIntensity = Shader.PropertyToID("_FogIntensity");
    static readonly int FogColorId = Shader.PropertyToID("_FogColor");
    static readonly int CameraForwardId = Shader.PropertyToID("_Forward");

    HeightFogVolume volume;
    Material material;
    RenderTargetIdentifier currentTarget;

    //构造函数 设置材质，shader
    public HeightFogPass(RenderPassEvent evt)
    {
        renderPassEvent = evt;
        var shader = Shader.Find("MS_Shader/HeightFog");
        if (shader == null)
        {
            Debug.LogError("Shader not found.");
            return;
        }
        material = CoreUtils.CreateEngineMaterial(shader);
    }
    
    //在相机渲染的时候调用执行的函数
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (material == null)
        {
            Debug.LogError("Material not created.");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled) return;

        var stack = VolumeManager.instance.stack;
        volume = stack.GetComponent<HeightFogVolume>();
        if (volume == null) { return; }
        if (!volume.IsActive()) { return; }
        var cmd = CommandBufferPool.Get(k_RenderTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    //在Excute中调用的函数，单独开出来 分配 shader属性
    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        ref var cameraData = ref renderingData.cameraData;
        var source = currentTarget;
        int destination = TempTargetId;

        var w = cameraData.camera.scaledPixelWidth;
        var h = cameraData.camera.scaledPixelHeight;
        material.SetTexture(FogNoiseId, volume.NoiseTex.value);
        material.SetFloat(FogStartHeightId, volume.FogStartHeight.value);
        material.SetFloat(FogEndHeightId, volume.FogEndHeight.value);
        material.SetFloat(FogIntensity, volume.FogIntensity.value);
        material.SetColor(FogColorId, volume.FogColor.value);
        material.SetVector(CameraForwardId,Camera.main.transform.forward);
        int shaderPass = 0;
        cmd.SetGlobalTexture(MainTexId, source);
        cmd.GetTemporaryRT(destination, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
        cmd.Blit(source, destination);
        cmd.Blit(destination, source, material, shaderPass);
    }
    
    public void Setup(in RenderTargetIdentifier currentTarget)
    {
        this.currentTarget = currentTarget;
    }
    
    //清理临时RT
    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        if (cmd == null)
        {
            throw new ArgumentNullException("cmd");
        }
        
        cmd.ReleaseTemporaryRT(TempTargetId);
    }
}