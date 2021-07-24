using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;

using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class LNL_ClientSystem : SystemBase, INetEventListener
{
    private NetManager _netManager;
    private NetDataWriter _writer;
    private NetPeer _server;

    private PlayerCharacterEntitySpawner _entitySpawner;

    //#if UNITY_EDITOR
    protected override void OnCreate() { Init(); }

    protected override void OnDestroy() { Shutdown(); }

    protected override void OnUpdate() { UpdateClient(); }
    //#endif

    public virtual void Init()
    {
        _netManager = new NetManager(this);
        _netManager.UnconnectedMessagesEnabled = true;
        _netManager.UpdateTime = 15;
        _netManager.Start();

        _writer = new NetDataWriter();

        _entitySpawner = new PlayerCharacterEntitySpawner();
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
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
    {
        Debug.Log("[CLIENT] We received error " + socketErrorCode);
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        MessageCode code = (MessageCode)reader.GetInt();
        
        if(code == MessageCode.CREATE_PLAYER_CHARACTER) {
            float x = (float)reader.GetFloat();
            float y = (float)reader.GetFloat();
            float z = (float)reader.GetFloat();
            float3 position = new float3(x, y, z);

            _entitySpawner.SpawnPlayerCharacterEntity(position, peer.Id);
        }
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

    public void SendChatMessageToServer(Net_ChatMessage message)
    {
        if (message.Message.Length == 0) return;

        Debug.Log("Mesagge: " + message.Message);

        if (IsConnectedToServer())
        {
            _writer.Reset();
            _writer.Put((int)message.Code);
            _writer.Put(message.Message);
            _server.Send(_writer, DeliveryMethod.ReliableOrdered);
        }

    }

    public void SendInputStatesToServer(Net_InputStateMessage message)
    {
        Debug.Log("Mesagge: " + message.Code);
        if (IsConnectedToServer())
        {
            LNL_InputStateSerializer.Write(ref _writer, message);
            _server.Send(_writer, DeliveryMethod.ReliableOrdered);
        }
    }
}

