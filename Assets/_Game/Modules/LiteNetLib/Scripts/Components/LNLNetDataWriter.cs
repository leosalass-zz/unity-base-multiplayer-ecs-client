using Unity.Entities;
using LiteNetLib;
using LiteNetLib.Utils;

[GenerateAuthoringComponent]
public struct LNLNetDataWriter : IComponentData
{
    private NetDataWriter _dataWriter;
}
