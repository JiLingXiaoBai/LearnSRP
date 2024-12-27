using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/ToyRenderPipelineAsset")]
public class ToyRenderPipelineAsset : RenderPipelineAsset<ToyRenderPipeline>
{
    protected override RenderPipeline CreatePipeline()
    {
        return new ToyRenderPipeline();
    }
}