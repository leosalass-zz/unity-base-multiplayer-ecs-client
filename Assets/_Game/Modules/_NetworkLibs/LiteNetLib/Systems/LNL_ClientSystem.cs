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

    #region System Methods
    //#if UNITY_EDITOR
    protected override void OnCreate() { Init(); }

    protected override void OnDestroy() { Shutdown(); }

    protected override void OnUpdate() { UpdateClient(); }
    //#endif
    #endregion

    #region System Methods Renamed
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

    public virtual void Shutdown()
    {
        if (_netManager != null)
            _netManager.Stop();
    }
    #endregion

    #region INetEventListener Methods
    private bool IsConnectedToServer()
    {
        var peer = _netManager.FirstPeer;
        if (peer != null && peer.ConnectionState == ConnectionState.Connected) return true;

        return false;
    }

    public void OnPeerConnected(NetPeer server)
    {
        _server = server;

        _writer.Reset();
        _writer.Put((int)MessageCode.SPAWN_PLAYER_CHARACTER_ENTITY);
        server.Send(_writer, DeliveryMethod.ReliableUnordered);

        Debug.Log("[CLIENT] We connected to " + server.EndPoint);
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
    {
        Debug.Log("[CLIENT] We received error " + socketErrorCode);
    }

    public void OnNetworkReceive(NetPeer server, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        NetworkMessagesHandler(server, reader, deliveryMethod);
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
    #endregion

    #region Incoming Network Messages
    private void NetworkMessagesHandler(NetPeer server, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        MessageCode code = (MessageCode)reader.GetInt();
        switch (code)
        {
            case MessageCode.CHAT_MESSAGE:
                ReceiveChatMessage(server, reader, deliveryMethod);
                break;

            case MessageCode.SPAWN_PLAYER_CHARACTER_ENTITY:
                ReceiveCreatePlayerCharacterMessage(server, reader, deliveryMethod);
                break;
        }
    }

    private void ReceiveChatMessage(NetPeer server, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        string message = LNL_ChatMessageSerializer.Read(ref reader);

        Debug.LogWarning("Chat Message received from: " + server.EndPoint + " with the connection id: " + server.Id);
        Debug.LogWarning("Message: " + message);
    }

    private void ReceiveCreatePlayerCharacterMessage(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        PlayerCharacterEntityMessage playerCharacterEntityMessage = LNL_PlayerCharacterEntityMessageSerializer.Read(ref reader);
        _entitySpawner.SpawnPlayerCharacterEntity(peer.Id, playerCharacterEntityMessage);
    }
    #endregion

    #region Outgoing Network Messages
    public void SendChatMessageToServer(Net_ChatMessage message)
    {
        Debug.Log("Message: " + message.Message);
        if (IsConnectedToServer())
        {
            LNL_ChatMessageSerializer.Write(message, _server, ref _writer);
            _server.Send(_writer, DeliveryMethod.ReliableOrdered);
        }
    }

    public void SendInputStatesToServer(Net_InputStateMessage message)
    {
        Debug.Log("Message: " + message.Code);
        if (IsConnectedToServer())
        {
            LNL_InputStateSerializer.Write(ref _writer, message);
            _server.Send(_writer, DeliveryMethod.ReliableOrdered);
        }
    }
    #endregion
}

