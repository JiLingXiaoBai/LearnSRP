using UnityEngine;
using UnityEngine.Rendering;

public class CustomRenderPipeline : RenderPipeline
{
    private CustomRenderPipelineAsset m_Asset;

    public CustomRenderPipeline(CustomRenderPipelineAsset asset)
    {
        m_Asset = asset;
    }

    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        var cmd = new CommandBuffer();
        //清除上一帧绘制的东西
        cmd.ClearRenderTarget(true, true, m_Asset.clearColor);
        context.ExecuteCommandBuffer(cmd);
        cmd.Release();

        // 会有显示Scene视图的SceneCamera，点击Camera时显示Preview视图的PreviewCamera，以及场景中我们添加的Camera
        foreach (var camera in cameras)
        {
            //获取当前相机的剔除规则，进行剔除
            camera.TryGetCullingParameters(out var cullingParameters);
            var cullingResults = context.Cull(ref cullingParameters);
            //根据当前Camera，更新内置Shader的变量
            context.SetupCameraProperties(camera);

            //生成DrawingSettings
            ShaderTagId shaderTagId = new ShaderTagId("CustomLightModeTag");
            var sortingSettings = new SortingSettings(camera);
            DrawingSettings drawingSettings = new DrawingSettings(shaderTagId, sortingSettings);

            //生成FilteringSettings
            FilteringSettings filteringSettings = FilteringSettings.defaultValue;
            // var renderListParam = new RendererListParams(cullingResults, drawingSettings, filteringSettings);
            // RendererList rendererList = context.CreateRendererList(ref renderListParam);
            // var cmd2 = new CommandBuffer();
            // cmd2.DrawRendererList(rendererList);
            // cmd2.Release();
            context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
            if (camera.clearFlags == CameraClearFlags.Skybox && RenderSettings.skybox != null)
            {
                //绘制天空盒
                context.DrawSkybox(camera);
            }
            context.Submit();
        }
    }
}