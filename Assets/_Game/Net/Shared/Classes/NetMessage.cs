using Unity.Networking.Transport;

public class NetMessage
{
    public MessageCode Code { get; set; }

    public virtual void Serialize(ref DataStreamWriter writer) { }

    public virtual void Deserialize() { }
}
