using Unity.Entities;

[GenerateAuthoringComponent]
public struct PendingInputs : IComponentData
{
    public int prediction;
    public int validation;
}