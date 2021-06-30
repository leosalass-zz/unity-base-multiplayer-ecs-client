using Unity.Networking.Transport;

public class Net_ChatMessage : NetMessage
{
    public Net_ChatMessage()
    {
        Code = MessageCode.CHAT_MESSAGE;
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
    }

    public override void Deserialize() { }
}
