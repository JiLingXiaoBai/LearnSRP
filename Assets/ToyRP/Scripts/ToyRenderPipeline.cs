using UnityEngine;
using UnityEngine.Rendering;

public class ToyRenderPipeline : RenderPipeline
{
    private RenderTexture _gDepth;
    private RenderTexture[] _gBuffers = new RenderTexture[4];
    private RenderTargetIdentifier[] _gBufferIds = new RenderTargetIdentifier[4];

    public ToyRenderPipeline()
    {
        _gDepth = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.Depth,
            RenderTextureReadWrite.Linear);
        _gBuffers[0] = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32,
            RenderTextureReadWrite.Linear);
        _gBuffers[1] = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB2101010,
            RenderTextureReadWrite.Linear);
        _gBuffers[2] = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB64,
            RenderTextureReadWrite.Linear);
        _gBuffers[3] = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat,
            RenderTextureReadWrite.Linear);

        for (int i = 0; i < 4; i++)
        {
            _gBufferIds[i] = _gBuffers[i];
        }
    }
    
    private void LightPass(ScriptableRenderContext context, Camera camera)
    {
        var cmd = new CommandBuffer();
        cmd.name = "lightPass";
        var mat = new Material(Shader.Find("ToyRP/lightPass"));
        cmd.Blit(_gBufferIds[0], BuiltinRenderTextureType.CameraTarget, mat);
        context.ExecuteCommandBuffer(cmd);
        cmd.Release();
    }


    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        var camera = cameras[0];
        context.SetupCameraProperties(camera);

        var cmd = new CommandBuffer();
        cmd.name = "gBuffer";

        cmd.SetRenderTarget(_gBufferIds, _gDepth);
        cmd.SetGlobalTexture("_gDepth", _gDepth);
        for (int i = 0; i < 4; i++)
        {
            cmd.SetGlobalTexture("_GT" + i, _gBuffers[i]);
        }
        // 清屏
        cmd.ClearRenderTarget(true, true, Color.red);

        // 剔除
        camera.TryGetCullingParameters(out var cullingParameters);
        var cullResults = context.Cull(ref cullingParameters);

        // config settings
        var shaderTagId = new ShaderTagId("gBuffer");
        var sortingSettings = new SortingSettings(camera);
        var drawingSettings = new DrawingSettings(shaderTagId, sortingSettings);
        var filteringSettings = FilteringSettings.defaultValue;

        // draw
        var renderListParam = new RendererListParams(cullResults, drawingSettings, filteringSettings);
        var rendererList = context.CreateRendererList(ref renderListParam);
        cmd.DrawRendererList(rendererList);

        var skyBoxRenderList = context.CreateSkyboxRendererList(camera);
        cmd.DrawRendererList(skyBoxRenderList);
#if UNITY_EDITOR
        if (UnityEditor.Handles.ShouldRenderGizmos())
        {
            context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
            context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
        }
#endif
        context.ExecuteCommandBuffer(cmd);
        cmd.Release();
        context.Submit();
    }
}