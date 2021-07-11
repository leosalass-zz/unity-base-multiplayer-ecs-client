using Unity.Entities;
using LiteNetLib;
using LiteNetLib.Utils;

[GenerateAuthoringComponent]
public struct LNLNetPeer : IComponentData {
    private NetPeer serverPeer;
}
