using Unity.Entities;

public class Net_ChatMessage : NetMessage
{    
    public string Message { set; get; }

    public Net_ChatMessage(string message)
    {
        Code = MessageCode.CHAT_MESSAGE;
        Message = message;
    }

    public override void Send()
    {
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<LNL_ClientSystem>().SendChatMessageToServer(this);
    }
}
