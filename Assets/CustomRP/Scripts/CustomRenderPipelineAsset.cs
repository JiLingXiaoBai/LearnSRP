
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/CustomRenderPipelineAsset")]
public class CustomRenderPipelineAsset : RenderPipelineAsset<CustomRenderPipeline>
{
    protected override RenderPipeline CreatePipeline()
    {
        return new CustomRenderPipeline();
    }
}
