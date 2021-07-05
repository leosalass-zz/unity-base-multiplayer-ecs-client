using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Unity.Entities;
using LiteNetLib;
using LiteNetLib.Utils;


public class LNLClientSystem : SystemBase, INetEventListener
{
    //TODO: start creating the NetworkEntity with NetManager, NetDataWriter, NetPeer as components
    private NetManager _netClient;
    private NetDataWriter _dataWriter;
    private NetPeer serverPeer;

#if UNITY_EDITOR
    protected override void OnCreate() { Init(); }

    protected override void OnDestroy() { Shutdown(); }

    protected override void OnUpdate() { UpdateClient(); }
#endif

    //[SerializeField] private GameObject _clientBall;
    //[SerializeField] private GameObject _clientBallInterpolated;

    ///private float _newBallPosX;
    //private float _oldBallPosX;
    //private float _lerpTime;

    public virtual void Init()
    {
        _netClient = new NetManager(this);
        _netClient.UnconnectedMessagesEnabled = true;
        _netClient.UpdateTime = 15;
        _netClient.Start();

        _dataWriter = new NetDataWriter();
    }

    public virtual void UpdateClient()
    {
        _netClient.PollEvents();

        var peer = _netClient.FirstPeer;
        if (IsConnectedToServer())
        {
            /*//Fixed delta set to 0.05
            var pos = _clientBallInterpolated.transform.position;
            pos.x = Mathf.Lerp(_oldBallPosX, _newBallPosX, _lerpTime);
            _clientBallInterpolated.transform.position = pos;

            //Basic lerp
            _lerpTime += Time.deltaTime / Time.fixedDeltaTime;**/
        }
        else
        {
            _netClient.SendBroadcast(new byte[] { 1 }, 5000);
        }
    }

    private bool IsConnectedToServer()
    {
        var peer = _netClient.FirstPeer;
        if (peer != null && peer.ConnectionState == ConnectionState.Connected) return true;

        return false;
    }

    public virtual void Shutdown()
    {
        if (_netClient != null)
            _netClient.Stop();
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("[CLIENT] We connected to " + peer.EndPoint);
        serverPeer = peer;
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
    {
        Debug.Log("[CLIENT] We received error " + socketErrorCode);
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        //_newBallPosX = reader.GetFloat();

        /*var pos = _clientBall.transform.position;

        _oldBallPosX = pos.x;
        pos.x = _newBallPosX;

        _clientBall.transform.position = pos;

        _lerpTime = 0f;*/
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        if (messageType == UnconnectedMessageType.BasicMessage && _netClient.ConnectedPeersCount == 0 && reader.GetInt() == 1)
        {
            Debug.Log("[CLIENT] Received discovery response. Connecting to: " + remoteEndPoint);
            _netClient.Connect(remoteEndPoint, "sample_app");
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
    }

    public void SendChatMessageToServer(string chatMessage)
    {
        Debug.Log(chatMessage);
        if (IsConnectedToServer())
        {
            _dataWriter.Reset();
            _dataWriter.Put((int)MessageCode.CHAT_MESSAGE);
            _dataWriter.Put(chatMessage);
            serverPeer.Send(_dataWriter, DeliveryMethod.ReliableOrdered);
        }
        
    }
}

