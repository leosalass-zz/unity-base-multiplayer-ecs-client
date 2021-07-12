using Unity.Entities;

public class NetMessage
{
    public MessageCode Code { get; set; }

    public virtual void Send() { }
    public virtual void Send(DynamicBuffer<InputState> buffer) { }
}
