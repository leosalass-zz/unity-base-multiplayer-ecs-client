using Unity.Entities;

[GenerateAuthoringComponent]
[InternalBufferCapacity(100)]
public struct ServerInputStateBuffer : IBufferElementData
{
    public InputState inputState;
}