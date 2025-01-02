using UnityEngine;
using UnityEngine.Rendering;

public class CameraRenderer
{
    private ScriptableRenderContext _context;
    private Camera _camera;

    private CullingResults _cullingResults;
    private const string BufferName = "Render Camera";
    
    private static readonly ShaderTagId UnlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");
    private readonly CommandBuffer _buffer = new CommandBuffer() { name = BufferName };


    public void Render(ScriptableRenderContext context, Camera camera)
    {
        _context = context;
        _camera = camera;

        if (!Cull())
            return;
        Setup();
        DrawVisibleGeometry();
        Submit();
    }

    private bool Cull()
    {
        if (!_camera.TryGetCullingParameters(out var cullingParameters)) return false;
        _cullingResults = _context.Cull(ref cullingParameters);
        return true;
    }


    private void Setup()
    {
        _context.SetupCameraProperties(_camera);
        _buffer.ClearRenderTarget(true, true, Color.clear);
        _buffer.BeginSample(BufferName);
        ExecuteBuffer();
    }

    private void DrawVisibleGeometry()
    {
        var sortingSettings = new SortingSettings(_camera){ criteria = SortingCriteria.CommonOpaque};
        var drawingSettings = new DrawingSettings(UnlitShaderTagId, sortingSettings);
        var filteringSettings = new FilteringSettings(RenderQueueRange.all);
        var renderListParam = new RendererListParams(_cullingResults, drawingSettings, filteringSettings);
        var renderList = _context.CreateRendererList(ref renderListParam);
        _buffer.DrawRendererList(renderList);
        var skyBoxRenderList = _context.CreateSkyboxRendererList(_camera);
        _buffer.DrawRendererList(skyBoxRenderList);
    }

    private void Submit()
    {
        _buffer.EndSample(BufferName);
        ExecuteBuffer();
        _context.Submit();
    }

    private void ExecuteBuffer()
    {
        _context.ExecuteCommandBuffer(_buffer);
        _buffer.Clear();
    }
}