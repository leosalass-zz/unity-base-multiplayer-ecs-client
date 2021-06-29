using Unity.Entities;

[GenerateAuthoringComponent]
[InternalBufferCapacity(10)]
public struct LocalShadowPlayerBuffer : IBufferElementData
{
    public ShadowState shadowState;
}
