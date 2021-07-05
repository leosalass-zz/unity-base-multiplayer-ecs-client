using Unity.Collections;
using Unity.Networking.Transport;

public class Net_ChatMessage : NetMessage
{
    // 0-8 MESSAGE CODE

    public string ChatMessage { set; get; }

    public Net_ChatMessage(string msg)
    {
        Code = MessageCode.CHAT_MESSAGE;
        ChatMessage = msg;
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteFixedString128(ChatMessage);
    }

    public override void Deserialize() { }
}
