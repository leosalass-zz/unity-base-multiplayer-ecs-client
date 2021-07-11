using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;

using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;



public class LNLClientSystem : SystemBase, INetEventListener
{
    private NetManager _netManager;
    private NetDataWriter _writer;
    private NetPeer _server;

    private EntitySpawner _entitySpawner;

#if UNITY_EDITOR
    protected override void OnCreate() { Init(); }

    protected override void OnDestroy() { Shutdown(); }

    protected override void OnUpdate() { UpdateClient(); }
#endif

    public virtual void Init()
    {
        _netManager = new NetManager(this);
        _netManager.UnconnectedMessagesEnabled = true;
        _netManager.UpdateTime = 15;
        _netManager.Start();

        _writer = new NetDataWriter();

        _entitySpawner = new EntitySpawner();
    }

    public virtual void UpdateClient()
    {
        _netManager.PollEvents();

        var peer = _netManager.FirstPeer;
        if (IsConnectedToServer())
        {
        }
        else
        {
            _netManager.SendBroadcast(new byte[] { 1 }, 5000);
        }
    }

    private bool IsConnectedToServer()
    {
        var peer = _netManager.FirstPeer;
        if (peer != null && peer.ConnectionState == ConnectionState.Connected) return true;

        return false;
    }

    public virtual void Shutdown()
    {
        if (_netManager != null)
            _netManager.Stop();
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("[CLIENT] We connected to " + peer.EndPoint);
        _server = peer;

        float3 pos = new float3(2f, 0, 4f);
        _entitySpawner.SpawnPlayerCharacterEntity(pos);
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
    {
        Debug.Log("[CLIENT] We received error " + socketErrorCode);
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        if (messageType == UnconnectedMessageType.BasicMessage && _netManager.ConnectedPeersCount == 0 && reader.GetInt() == 1)
        {
            Debug.Log("[CLIENT] Received discovery response. Connecting to: " + remoteEndPoint);
            _netManager.Connect(remoteEndPoint, "sample_app");
        }
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {

    }

    public void OnConnectionRequest(ConnectionRequest request)
    {

    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log("[CLIENT] We disconnected because " + disconnectInfo.Reason);
        _entitySpawner.DestroyAndResetAllEntities();
    }

    public void SendChatMessageToServer(string chatMessage)
    {
        Debug.Log(chatMessage);
        if (IsConnectedToServer())
        {
            _writer.Reset();
            _writer.Put((int)MessageCode.CHAT_MESSAGE);
            _writer.Put(chatMessage);
            _server.Send(_writer, DeliveryMethod.ReliableOrdered);
        }

    }
}

