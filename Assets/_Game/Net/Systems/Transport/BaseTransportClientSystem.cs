using Unity.Burst;
using UnityEngine;
using UnityEngine.Assertions;

using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Networking.Transport;

public class BaseTransportClientSystem : SystemBase
{
    public NetworkDriver m_Driver;
    public NativeArray<NetworkConnection> m_Connection;
    public NativeArray<byte> m_Done;

    public JobHandle ClientJobHandle;

#if UNITY_EDITOR
    protected override void OnCreate() { Init(); }

    protected override void OnDestroy() { Shutdown(); }

    protected override void OnUpdate() { UpdateClient(); }
#endif

    public virtual void Init()
    {
        m_Driver = NetworkDriver.Create();

        m_Connection = new NativeArray<NetworkConnection>(1, Allocator.Persistent);
        m_Done = new NativeArray<byte>(1, Allocator.Persistent);
        var endpoint = NetworkEndPoint.LoopbackIpv4;
        endpoint.Port = 9000;
        m_Connection[0] = m_Driver.Connect(endpoint);

        Debug.Log("IsCreated: " + m_Connection[0].IsCreated);
    }
    public virtual void Shutdown()
    {
        Debug.LogError("disconected from the server");
        ClientJobHandle.Complete();
        m_Connection.Dispose();
        m_Driver.Dispose();
        m_Done.Dispose();
    }

    public virtual void UpdateClient()
    {
        ClientJobHandle.Complete();
        var job = new ClientUpdateJob
        {
            driver = m_Driver,
            connection = m_Connection,
            done = m_Done
        };
        ClientJobHandle = m_Driver.ScheduleUpdate();
        ClientJobHandle = job.Schedule(ClientJobHandle);
    }

    public void SendChatMessageToServer(string chatMessage)
    {
        ClientJobHandle.Complete();
        if (!m_Connection[0].IsCreated)
        {
            Debug.Log("Something went wrong sending the message");
            return;
        }

        DataStreamWriter writer;
        m_Driver.BeginSend(m_Connection[0], out writer);
        Net_ChatMessage net_ChatMessage = new Net_ChatMessage(chatMessage);
        net_ChatMessage.Serialize(ref writer);
        m_Driver.EndSend(writer);
    }
}

struct ClientUpdateJob : IJob
{
    public NetworkDriver driver;
    public NativeArray<NetworkConnection> connection;
    public NativeArray<byte> done;

    public void Execute()
    {
        if (!connection[0].IsCreated)
        {
            if (done[0] != 1)
                Debug.Log("Something went wrong during connect, IsCreated: " + connection[0].IsCreated + " done: " + done[0]);
            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd;

        while ((cmd = connection[0].PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We are now connected to the server");
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from server");
                connection[0] = default(NetworkConnection);
            }
        }
    }
}
