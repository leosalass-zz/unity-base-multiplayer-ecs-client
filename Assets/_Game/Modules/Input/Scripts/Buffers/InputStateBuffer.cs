using Unity.Entities;

[GenerateAuthoringComponent]
[InternalBufferCapacity(10)]
public struct InputStateBuffer : IBufferElementData
{
    public InputState inputState;
}