using Unity.Entities;
using LiteNetLib;

[GenerateAuthoringComponent]
public struct LNLNetManager : IComponentData
{
    private NetManager _netClient;
}
