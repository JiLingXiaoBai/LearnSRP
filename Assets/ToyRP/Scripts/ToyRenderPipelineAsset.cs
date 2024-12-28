using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/ToyRenderPipelineAsset")]
public class ToyRenderPipelineAsset : RenderPipelineAsset<ToyRenderPipeline>
{
    public Cubemap diffuseIBL;
    public Cubemap specularIBL;
    public Texture brdfLut;
    
    protected override RenderPipeline CreatePipeline()
    {
        var rp = new ToyRenderPipeline();
        rp.diffuseIBL = diffuseIBL;
        rp.specularIBL = specularIBL;
        rp.brdfLut = brdfLut;
        return rp;
    }
}